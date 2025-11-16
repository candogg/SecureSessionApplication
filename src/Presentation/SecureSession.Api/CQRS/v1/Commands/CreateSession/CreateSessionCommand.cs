using MediatR;
using SecureSession.Api.Dto;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Commands.CreateSession
{
    public record CreateSessionCommand(SessionExchangeDto Request) : IRequest<ResponseItem<SessionExchangeDto>>;
}
