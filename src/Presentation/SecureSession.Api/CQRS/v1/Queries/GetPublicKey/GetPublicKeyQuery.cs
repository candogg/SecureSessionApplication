using MediatR;
using SecureSession.Api.Dto;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Queries.GetPublicKey
{
    public record GetPublicKeyQuery : IRequest<ResponseItem<SessionExchangeDto>>;
}
