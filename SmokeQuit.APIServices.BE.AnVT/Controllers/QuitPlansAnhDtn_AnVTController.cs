using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokeQuit.Repositories.AnVT.Models;
using SmokeQuit.Services.AnVT;

namespace SmokeQuit.APIServices.BE.AnVT.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuitPlansAnhDtn_AnVTController : ControllerBase
	{
		private readonly IQuitPlansAnhDtn_AnVTService _quitPlansAnhDtn_AnVTService;
		public QuitPlansAnhDtn_AnVTController(IQuitPlansAnhDtn_AnVTService quitPlansAnhDtn_AnVTService)
		{
			_quitPlansAnhDtn_AnVTService = quitPlansAnhDtn_AnVTService;
		}
		// GET: api/<QuitPlansAnhDtn_AnVTController>
		[HttpGet]
		public async Task<ActionResult<List<QuitPlansAnhDtn>>> Get()
		{
			var plans = await _quitPlansAnhDtn_AnVTService.GetAllAsync();
			return Ok(plans);
		}
	}
}
