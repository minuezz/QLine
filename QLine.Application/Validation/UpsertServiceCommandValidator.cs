using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using QLine.Application.Features.Admin.Services.Commands;

namespace QLine.Application.Validation
{
    public sealed class UpsertServiceCommandValidator : AbstractValidator<UpsertServiceCommand>
    {
        public UpsertServiceCommandValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.ServicePointId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.DurationMin).GreaterThan(0);
            RuleFor(x => x.BufferMin).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxPerDay).GreaterThanOrEqualTo(0);
        }
    }
}
