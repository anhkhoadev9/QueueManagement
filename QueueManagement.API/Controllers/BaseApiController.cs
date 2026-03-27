using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IMediator _mediator;
        public BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
