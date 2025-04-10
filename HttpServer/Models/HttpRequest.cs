namespace HttpServer.Models
{
    class HttpRequest
    {
        public HttpMethodEnum Method { get; set; }
        public Uri? Uri { get; set; }
        public string Version { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; }
        public bool isValid = true;
    }
}
