using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecureSession.Api.CQRS.v1.Queries.Test;
using System.ComponentModel.DataAnnotations;

namespace SecureSession.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController(IMediator mediator) : ControllerBase
    {
        [HttpPost("Test")]
        public async Task<IActionResult> Test([FromBody, Required] TestQuery query)
        {
            var result = await mediator.Send(query);

            return Ok(result);
        }
    }
}
