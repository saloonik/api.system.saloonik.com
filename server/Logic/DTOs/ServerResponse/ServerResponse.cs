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
        public static ServerResponse CreateValidationFailedResponse(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
        {
            var errorList = string.Join(", ", errors.Select(e => e.ErrorMessage));
            return ServerResponse.CreateErrorResponse(
                "Niepoprawne dane formularza",
                StatusCodes.Status400BadRequest,
                $"Wprowadzone dane są nieprawidłowe: {errorList}"
            );
        }

        public static ServerResponse CreateBadRequestResponse(string message) =>
            ServerResponse.CreateErrorResponse("Nieprawidłowe żądanie", StatusCodes.Status400BadRequest, message);

        public static ServerResponse CreateUnauthorizedResponse(string message) =>
            ServerResponse.CreateErrorResponse("Nieautoryzowany dostęp", StatusCodes.Status401Unauthorized, message);

        public static ServerResponse CreateConflictResponse(string message) =>
            ServerResponse.CreateErrorResponse("Konflikt danych", StatusCodes.Status409Conflict, message);

        public static ServerResponse CreateInternalErrorResponse(string message) =>
            ServerResponse.CreateErrorResponse("Błąd wewnętrzny serwera", StatusCodes.Status500InternalServerError, message);
    }
}
