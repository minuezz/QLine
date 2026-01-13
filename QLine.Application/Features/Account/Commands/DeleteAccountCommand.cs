using MediatR;

namespace QLine.Application.Features.Account.Commands
{
    public sealed record DeleteAccountCommand(Guid UserId, string Password) : IRequest;
}
