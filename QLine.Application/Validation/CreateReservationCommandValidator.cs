using FluentValidation;
using QLine.Application.Features.Reservations.Commands;

namespace QLine.Application.Validation
{
    public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
    {
        public CreateReservationCommandValidator()
        {
            RuleFor(x => x.ServicePointId).NotEmpty();
            RuleFor(x => x.ServiceId).NotEmpty();
            RuleFor(x => x.StartTime).NotEqual(default(DateTimeOffset));
        }
    }
}
