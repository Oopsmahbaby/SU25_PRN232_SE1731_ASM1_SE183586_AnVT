using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SmokeQuit.Repositories.AnVT.DTOs;
using SmokeQuit.Repositories.AnVT.ModelExtensions;
using SmokeQuit.Repositories.AnVT.Models;
using SmokeQuit.Services.AnVT;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmokeQuit.APIServices.BE.AnVT.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class BlogPostsAnVtsController : ControllerBase
	{
		private readonly IBlogPostsAnVTService _blogPostsAnVTService;

		public BlogPostsAnVtsController(IBlogPostsAnVTService blogPostsAnVTService)
		{
			_blogPostsAnVTService = blogPostsAnVTService;
		}

		// GET: api/<BlogPostsAnVtController>
		[Authorize(Roles = "1, 2")]
		[HttpGet("{pageNumber}/{pageSize}")]
		public async Task<ActionResult<PagedResult<BlogPostsAnVt>>> GetAllWithPagingAsync(int pageNumber, int pageSize)
		{
			var blogPosts = await _blogPostsAnVTService.GetAllWithPagingAsync(pageNumber, pageSize);
			return Ok(blogPosts);
		}

		//[Authorize(Roles = "1, 2")]
		[HttpGet]
		public async Task<ActionResult<List<BlogPostsAnVt>>> Get()
		{
			var blogPosts = await _blogPostsAnVTService.GetAllAsync();
			return Ok(blogPosts);
		}

		/// <summary>
		/// Filter using oData
		/// </summary>
		/// <returns></returns>
		[HttpGet("searchByOData")]
		[EnableQuery]
		public async Task<ActionResult<List<BlogPostsAnVt>>> Search()
		{
			var blogPosts = await _blogPostsAnVTService.GetAllAsync();
			return Ok(blogPosts);
		}

		// GET api/<BlogPostsAnVtController>/5
		[Authorize(Roles = "1, 2")]
		[HttpGet("{id}")]
		public async Task<BlogPostsAnVt> Get(int id)
		{
			return await _blogPostsAnVTService.GetByIdAsync(id);
		}

		// POST api/<BlogPostsAnVtController>
		[Authorize(Roles = "1, 2")]
		[HttpPost]
		public async Task<int> Post([FromBody] BlogPostsAnVtDto value)
		{
			if (!ModelState.IsValid)
			{
				return ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault() switch
				{
					string errorMessage => throw new ArgumentException(errorMessage),
					_ => throw new ArgumentException("Invalid model state.")
				};
			}
			return await _blogPostsAnVTService.CreateAsync(value);
		}

		// PUT api/<BlogPostsAnVtController>/5
		[Authorize(Roles = "1, 2")]
		[HttpPut("{id}")]
		public async Task<int> Put([FromBody] BlogPostsAnVt value)
		{
			return await _blogPostsAnVTService.UpdateAsync(value);
		}

		// DELETE api/<BlogPostsAnVtController>/5
		//[Authorize(Roles = "1")]
		[HttpDelete("{id}")]
		public async Task<bool> Delete(int id)
		{
			return await _blogPostsAnVTService.DeleteAsync(id);
		}

		// GET api/<BlogPostsAnVtController>/search
		[Authorize(Roles = "1, 2")]
		[HttpGet("search")]
		public async Task<List<BlogPostsAnVt>> Search(int? planId = null, string title = null, string category = null, string tags = null, bool? isPublic = null)
		{
			return await _blogPostsAnVTService.SearchAsync(planId, title, category, tags, isPublic);
		}

		// GET api/<BlogPostsAnVtController>/searchByUserId
		//[Authorize(Roles = "1, 2")]
		[HttpPost("searchWithPaging")]
		public async Task<ActionResult<PagedResult<BlogPostsAnVt>>> SearchWithPagingAsync([FromBody] BlogPostSearchRequest request)
		{
			var result = await _blogPostsAnVTService.SearchWithPagingAsync(request);

			return Ok(result);
		}
	}
}
