using dotnet_core_api_with_jwt.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_core_api_with_jwt.Middleware
{
    public class ApiLogHandling
    {
        private readonly ILogger<LogController> _logger;
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly LOG_WEB_API DATA = new LOG_WEB_API();

        public ApiLogHandling(ILogger<LogController> logger,IConfiguration configuration, RequestDelegate next)
        {
            _logger = logger;
            _configuration = configuration;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var request = await FormatRequest(context, context.Request);
            // Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;
            // Create a new memory stream...
            using var responseBody = new MemoryStream();
            // ...and use that for the temporary response body
            context.Response.Body = responseBody;
            // Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);
            
            // Format the response from the server
            var response = await FormatResponse(context, context.Response);
            var responseHeaders = new StringBuilder();
            responseHeaders.Append(response);
            foreach (var header in context.Response.Headers)
            {
                responseHeaders.Append("\"" + header.Key + "\"" + ":" + "\"" + header.Value + "\"" + ",");
                if (header.Key.Equals("Content-Type")) { DATA.ResponseContentType = header.Value; }
            }
            if (DATA.ResponseContentType == null) { DATA.ResponseContentType = ""; }
            if (responseHeaders.Length == 0) { DATA.ResponseHeaders = ""; }
            else { DATA.ResponseHeaders = "{" + responseHeaders.ToString().TrimEnd(',') + "}"; }

            // Save log to chosen datastore
            Logger(DATA);

            // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpContext context, HttpRequest request)
        {
            var requestHeaders = new StringBuilder();
            foreach (var header in context.Request.Headers)
            {
                requestHeaders.Append("\"" + header.Key + "\"" + ":" + "\"" + header.Value + "\"" + ",");
                if (header.Key.Equals("Content-Type")) { DATA.RequestContentType = header.Value; }
            }
            if (DATA.RequestContentType == null) { DATA.RequestContentType = ""; }
            if (requestHeaders.Length == 0) { DATA.RequestHeaders = ""; }
            else { DATA.RequestHeaders = "{" + requestHeaders.ToString().TrimEnd(',') + "}"; }

            // Leave the body open so the next middleware can read it.
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            var requestContentBody = await reader.ReadToEndAsync();
            // Do some processing with body…
            var formattedRequest = $"{request.Scheme}://" + $"{request.Host}{request.Path}{request.QueryString}{requestContentBody}";
            var requestUri = $"{request.Scheme}://" + $"{request.Host}{request.Path}";
            
            // Reset the request body stream position so the next middleware can read it
            request.Body.Position = 0;

            string[] fullpath = request.Path.ToString().Split('/');
            string controller = "";
            string action = "";
            if (fullpath.Length == 4)
            {
                controller = fullpath[2];
                action = fullpath[3];
            }

            DATA.ChannelType = "A";
            DATA.Application = "api";
            DATA.Machine = Environment.MachineName;
            DATA.MachineIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            DATA.ControllerName = controller;
            DATA.ActionName = action;
            DATA.ApiPath = request.Path;

            DATA.RequestContentBody = requestContentBody;
            DATA.RequestIpAddress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            DATA.RequestMethod = request.Method;
            DATA.RequestTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US"));
            DATA.RequestUri = requestUri;
            DATA.RequestURLParams = $"{request.QueryString}";

            return formattedRequest;
        }

        private async Task<string> FormatResponse(HttpContext context, HttpResponse response)
        {
            // We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);
            // ...and copy it into a string
            string responseContentBody = await new StreamReader(response.Body).ReadToEndAsync();
            // We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            DATA.ResponseContentBody = responseContentBody;
            DATA.ResponseStatusCode = response.StatusCode;
            DATA.ResponseTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US"));

            // Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}:{responseContentBody}";
        }

        private void Logger(LOG_WEB_API data)
        {
            LogController log = new LogController(_logger, _configuration);
            if (data.ControllerName != "" && data.ActionName != "")
            {
                log.TracingLogfile(data);
            }
        }
    }
}
