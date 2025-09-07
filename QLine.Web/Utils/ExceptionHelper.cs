using FluentValidation;

namespace QLine.Web.Utils
{
    public static class ExceptionHelper
    {
        public static (string message, List<string> details) ToUserMessage(this Exception ex)
        {
            if (ex is ValidationException vex)
            {
                var details = vex.Errors.Select(e =>
                    string.IsNullOrWhiteSpace(e.PropertyName) ? e.ErrorMessage : $"{e.PropertyName}: {e.ErrorMessage}"
                ).ToList();
                return ("Check provided data.", details);
            }

            return (ex.Message, new List<string>());
        }
    }
}
