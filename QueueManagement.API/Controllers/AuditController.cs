using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Application.Features.Audit;
using System;
using System.Threading.Tasks;

namespace QueueManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy báo cáo tổng quan theo ngày (mặc định là hôm nay)
        /// </summary>
        [HttpGet("daily-report")]
        public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date = null)
        {
            var result = await _mediator.Send(new GetDailyReportQuery(date));
            return Ok(result);
        }
    }
}
