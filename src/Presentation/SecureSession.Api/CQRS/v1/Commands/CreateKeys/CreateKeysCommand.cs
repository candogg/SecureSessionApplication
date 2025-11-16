using MediatR;
using SecureSession.Api.Items;

namespace SecureSession.Api.CQRS.v1.Commands.CreateKeys
{
    public record CreateKeysCommand(bool WillReplace) : IRequest<ResponseItem<object>>;
}
