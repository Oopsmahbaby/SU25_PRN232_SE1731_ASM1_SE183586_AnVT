using SmokeQuit.Repositories.AnVT;
using SmokeQuit.Repositories.AnVT.DTOs;
using SmokeQuit.Repositories.AnVT.ModelExtensions;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Services.AnVT
{
	public interface IBlogPostsAnVTService
	{
		Task<List<BlogPostsAnVt>> GetAllAsync();
		Task<PagedResult<BlogPostsAnVt>> GetAllWithPagingAsync(int pageNumber, int pageSize);
		Task<BlogPostsAnVt> GetByIdAsync(int code);
		Task<List<BlogPostsAnVt>> SearchAsync(int? planId = null, string title = null, string category = null, string tags = null, bool? isPublic = null);
		Task<PagedResult<BlogPostsAnVt>> SearchWithPagingAsync(
			int pageNumber,
			int pageSize,
			int? planId = null,
			string title = null,
			string category = null,
			string tags = null,
			bool? isPublic = null);
		Task<int> CreateAsync(BlogPostsAnVtDto blogPost);
		Task<int> UpdateAsync(BlogPostsAnVt blogPost);
		Task<bool> DeleteAsync(int code);
	}
	public class BlogPostsAnVTService : IBlogPostsAnVTService
	{
		private readonly BlogPostsAnVTRepository _blogPostsAnVTRepository;

		public BlogPostsAnVTService()
		{
			_blogPostsAnVTRepository ??= new();
		}

		public BlogPostsAnVTService(BlogPostsAnVTRepository blogPostsAnVTRepository)
		{
			_blogPostsAnVTRepository = blogPostsAnVTRepository;
		}

		public Task<int> CreateAsync(BlogPostsAnVtDto blogPost)
		{
			var entities = new BlogPostsAnVt
			{
				UserId = blogPost.UserId,
				PlanId = blogPost.PlanId,
				Title = blogPost.Title,
				Content = blogPost.Content,
				Category = blogPost.Category,
				Tags = blogPost.Tags,
				IsPublic = blogPost.IsPublic,
			};
			return _blogPostsAnVTRepository.CreateAsync(entities);
		}

		public async Task<bool> DeleteAsync(int code)
		{
			var blogPost = await _blogPostsAnVTRepository.GetByIdAsync(code);
			if (blogPost != null)
			{
				return await _blogPostsAnVTRepository.RemoveAsync(blogPost);
			}

			return false;
		}

		public async Task<PagedResult<BlogPostsAnVt>> GetAllWithPagingAsync(int pageNumber, int pageSize)
		{
			return await _blogPostsAnVTRepository.GetAllWithPagingAsync(pageNumber, pageSize);
		}

		public async Task<List<BlogPostsAnVt>> GetAllAsync()
		{
			return await _blogPostsAnVTRepository.GetAllWithAsync();
		}

		public async Task<BlogPostsAnVt> GetByIdAsync(int code)
		{
			return await _blogPostsAnVTRepository.GetByIdAsync(code);
		}

		public async Task<List<BlogPostsAnVt>> SearchAsync(int? planId = null, string title = null, string category = null, string tags = null, bool? isPublic = null)
		{
			return await _blogPostsAnVTRepository.SearchAsync(planId, title, category, tags, isPublic);
		}

		public async Task<int> UpdateAsync(BlogPostsAnVt blogPost)
		{
			return await _blogPostsAnVTRepository.UpdateAsync(blogPost);
		}

		public async Task<PagedResult<BlogPostsAnVt>> SearchWithPagingAsync(
			int pageNumber,
			int pageSize,
			int? planId = null,
			string title = null,
			string category = null,
			string tags = null,
			bool? isPublic = null)
		{
			return await _blogPostsAnVTRepository.SearchWithPagingAsync(pageNumber, pageSize, planId, title, category, tags, isPublic);
		}
	}
}
