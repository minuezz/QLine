using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using QLine.Application.Features.Admin.ServicePoints.Commands;

namespace QLine.Application.Validation
{
    public sealed class UpsertServicePointCommandValidator : AbstractValidator<UpsertServicePointCommand>
    {
        public UpsertServicePointCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.OpenHoursJson).NotNull();
        }
    }
}
