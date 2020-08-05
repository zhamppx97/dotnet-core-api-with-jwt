using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using System;

namespace dotnet_core_api_with_jwt.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogger<LogController> _logger;
        private readonly IConfiguration _configuration;

        public LogController(ILogger<LogController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void TracingLogfile(LOG_WEB_API data)
        {
            try
            {
                NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
                logger = LogManager.GetLogger("LogfileRequestResponse");
                NLog.MappedDiagnosticsContext.Set("APPNAME", "api");
                NLog.MappedDiagnosticsContext.Set("Application", data.Application);
                NLog.MappedDiagnosticsContext.Set("ControllerName", data.ControllerName);
                NLog.MappedDiagnosticsContext.Set("ActionName", data.ActionName);
                //NLog.MappedDiagnosticsContext.Set("UserID", data.UserID);
                NLog.MappedDiagnosticsContext.Set("Machine", data.Machine);
                NLog.MappedDiagnosticsContext.Set("MachineIpAddress", data.MachineIpAddress);
                NLog.MappedDiagnosticsContext.Set("RequestIpAddress", data.RequestIpAddress);
                NLog.MappedDiagnosticsContext.Set("RequestTimestamp", data.RequestTimestamp);
                NLog.MappedDiagnosticsContext.Set("ResponseTimestamp", data.ResponseTimestamp);
                NLog.MappedDiagnosticsContext.Set("RequestHeaders", data.RequestHeaders);
                NLog.MappedDiagnosticsContext.Set("RequestUri", data.RequestUri);
                NLog.MappedDiagnosticsContext.Set("ApiPath", data.ApiPath);
                NLog.MappedDiagnosticsContext.Set("RequestContentType", data.RequestContentType);
                NLog.MappedDiagnosticsContext.Set("RequestMethod", data.RequestMethod);
                NLog.MappedDiagnosticsContext.Set("RequestURLParams", data.RequestURLParams);
                NLog.MappedDiagnosticsContext.Set("RequestContentBody", data.RequestContentBody);
                NLog.MappedDiagnosticsContext.Set("ResponseContentType", data.ResponseContentType);
                NLog.MappedDiagnosticsContext.Set("ResponseContentBody", data.ResponseContentBody);
                NLog.MappedDiagnosticsContext.Set("ResponseStatusCode", data.ResponseStatusCode);
                NLog.MappedDiagnosticsContext.Set("ResponseHeaders", data.ResponseHeaders);
                NLog.MappedDiagnosticsContext.Set("ChannelType", data.ChannelType);
                logger.Log(NLog.LogLevel.Trace, "");
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }

        public void ExceptionLogfile(LOG_WEB_API_ERROR data)
        {
            try
            {
                NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
                logger = LogManager.GetLogger("LogfileException");
                NLog.MappedDiagnosticsContext.Set("APPNAME", "api");
                NLog.MappedDiagnosticsContext.Set("LOG_LEVEL", data.LOG_LEVEL);
                NLog.MappedDiagnosticsContext.Set("SERVICE_NAME", data.SERVICE_NAME);
                NLog.MappedDiagnosticsContext.Set("SERVICE_TYPE", data.SERVICE_TYPE);
                NLog.MappedDiagnosticsContext.Set("ERROR_MESSAGE", data.ERROR_MESSAGE);
                NLog.MappedDiagnosticsContext.Set("SERVER_IP", data.SERVER_IP);
                NLog.MappedDiagnosticsContext.Set("CLIENT_IP ", data.CLIENT_IP);
                NLog.MappedDiagnosticsContext.Set("LOG_DATE", data.LOG_DATE);
                logger.Log(NLog.LogLevel.Trace, "");
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
        }
    }
}
