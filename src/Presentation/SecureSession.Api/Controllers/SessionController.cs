using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSession.Api.CQRS.v1.Commands.CreateSession;
using SecureSession.Api.CQRS.v1.Queries.GetPublicKey;
using System.ComponentModel.DataAnnotations;

namespace SecureSession.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetPublicKey")]
        public async Task<IActionResult> GetPublicKey(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetPublicKeyQuery(), cancellationToken);

            return Ok(result);
        }

        [HttpPost("CreateSession")]
        public async Task<IActionResult> CreateSession([FromBody, Required] CreateSessionCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
