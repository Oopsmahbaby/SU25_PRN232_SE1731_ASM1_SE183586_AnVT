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
		public async Task<IEnumerable<QuitPlansAnhDtn>> Get()
		{
			return await _quitPlansAnhDtn_AnVTService.GetAllAsync();
		}

		// GET api/<QuitPlansAnhDtn_AnVTController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<QuitPlansAnhDtn>> Get(int id)
		{
			var plan = await _quitPlansAnhDtn_AnVTService.GetByIdAsync(id);
			if (plan == null)
			{
				return NotFound();
			}
			return Ok(plan);
		}

		// POST api/<QuitPlansAnhDtn_AnVTController>
		[HttpPost]
		public async Task<ActionResult<int>> Post([FromBody] QuitPlansAnhDtn quitPlan)
		{
			if (quitPlan == null)
				return BadRequest();

			var createdId = await _quitPlansAnhDtn_AnVTService.AddAsync(quitPlan);
			return Ok(createdId);
		}

		// PUT api/<QuitPlansAnhDtnController>/5
		[HttpPut]
		public async Task<int> Put(QuitPlansAnhDtn quitPlans)
		{
			if (ModelState.IsValid)
			{
				return await _quitPlansAnhDtn_AnVTService.UpdateAsync(quitPlans);
			}

			return 0;
		}

		// DELETE api/<QuitPlansAnhDtn_AnVTController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _quitPlansAnhDtn_AnVTService.DeleteAsync(id);
			if (!success)
				return NotFound();

			return Ok(success);
		}

		// GET api/<QuitPlansAnhDtn_AnVTController>/user/5
		[HttpGet("user/{userId}")]
		public async Task<ActionResult<QuitPlansAnhDtn>> GetByUserId(int userId)
		{
			var plan = await _quitPlansAnhDtn_AnVTService.GetByUserIdAsync(userId);
			if (plan == null)
				return NotFound();

			return Ok(plan);
		}


	}
}
