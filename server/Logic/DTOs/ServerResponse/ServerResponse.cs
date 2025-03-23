namespace beautysalon.Logic.DTOs.ServerResponse
{
    public class ServerResponse()
    {
        public bool IsSuccess { get; set; }
        public required string ResultTitle { get; set; }
        public required string ResultDescription { get; set; }
        public required int StatusCode { get; set; }
        public required string StatusMessage { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public static ServerResponse CreateErrorResponse (string message, int statusCode, string description = null)
        {
            return new ServerResponse
            {
                IsSuccess = false,
                ResultTitle = message,
                StatusCode = statusCode,
                StatusMessage = message,
                ResultDescription = description ?? message
            };
        }
    }
}
