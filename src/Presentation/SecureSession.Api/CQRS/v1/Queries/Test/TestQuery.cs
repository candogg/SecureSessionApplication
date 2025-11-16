using MediatR;
using SecureSession.Api.Dto;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Queries.Test
{
    public record TestQuery(CryptedDto Request) : IRequest<ResponseItem<CryptedDto>>;
}
