using dotnet_core_api_with_jwt.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace dotnet_core_api_with_jwt.Middleware
{
    public class ExceptionHandling
    {
        private readonly ILogger<LogController> _logger;
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;

        public ExceptionHandling(ILogger<LogController> logger, IConfiguration configuration, RequestDelegate next)
        {
            _logger = logger;
            _configuration = configuration;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = JsonConvert.SerializeObject(new { error = ex.Message });

            string[] fullpath = context.Request.Path.ToString().Split('/');
            string controller = "null";
            string action = "null";
            if (fullpath.Length == 5)
            {
                controller = fullpath[3];
                action = fullpath[4];
            }

            LOG_WEB_API_ERROR data = new LOG_WEB_API_ERROR()
            {
                LOG_DATE = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US")),
                CLIENT_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(),
                SERVER_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(),
                ERROR_MESSAGE = result,
                LOG_LEVEL = "Error",
                SERVICE_NAME = string.Format("{0}/{1}", controller, action),
                SERVICE_TYPE = "I"
            };
            Logger(data);

            return context.Response.WriteAsync(result);
        }

        private void Logger(LOG_WEB_API_ERROR data)
        {
            LogController log = new LogController(_logger, _configuration);
            log.ExceptionLogfile(data);
        }
    }
}
