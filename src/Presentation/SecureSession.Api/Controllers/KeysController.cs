using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSession.Api.CQRS.v1.Commands.CreateKeys;

namespace SecureSession.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController(IMediator mediator) : ControllerBase
    {
        [HttpPut("CreateKeys")]
        public async Task<IActionResult> CreateKeys(CreateKeysCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
