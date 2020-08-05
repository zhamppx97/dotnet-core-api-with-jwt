using System;

namespace dotnet_core_api_with_jwt
{
    public class LOG_WEB_API
    {
        public long LogID { get; set; }
        public string Application { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Machine { get; set; }
        public string MachineIpAddress { get; set; }
        public string ApiPath { get; set; }
        public string RequestURLParams { get; set; }
        public string RequestIpAddress { get; set; }
        public string RequestContentType { get; set; }
        public string RequestContentBody { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseContentBody { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseTimestamp { get; set; }
        public string ChannelType { get; set; }
    }
}
