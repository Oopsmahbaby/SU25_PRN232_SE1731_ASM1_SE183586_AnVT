using Microsoft.EntityFrameworkCore;
using SmokeQuit.Repositories.AnVT.Basic;
using SmokeQuit.Repositories.AnVT.DBContext;
using SmokeQuit.Repositories.AnVT.ModelExtensions;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Repositories.AnVT
{
	public class BlogPostsAnVTRepository : GenericRepository<BlogPostsAnVt>
	{
		public BlogPostsAnVTRepository()
		{
			_context ??= new SU25_PRN232_SE1731_G6_SmokeQuitContext();
		}

		public BlogPostsAnVTRepository(SU25_PRN232_SE1731_G6_SmokeQuitContext context)
		{
			_context = context;
		}

		public async Task<List<BlogPostsAnVt>> GetAllWithAsync()
		{
			var blogPosts = await _context.BlogPostsAnVts
								.Include(x => x.User)
								.Include(x => x.Plan)
								.ToListAsync();

			return blogPosts ?? new List<BlogPostsAnVt>();
		}

		public async Task<PagedResult<BlogPostsAnVt>> GetAllWithPagingAsync(int pageNumber, int pageSize)
		{
			//var blogPosts = await _context.BlogPostsAnVts
			//							  .Include(x => x.User)
			//							  .Include(x => x.Plan)
			//							  .Skip((pageNumber - 1) * pageSize)
			//							  .Take(pageSize)
			//							  .ToListAsync();

			var query = _context.BlogPostsAnVts
								.Include(x => x.User)
								.Include(x => x.Plan);

			var totalCount = await query.CountAsync();
			var items = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<BlogPostsAnVt>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = pageNumber,
				PageSize = pageSize
			};
		}

		public async Task<BlogPostsAnVt> GetByIdAsync(int code)
		{
			var blogPost = await _context.BlogPostsAnVts.Include(x => x.User).Include(x => x.Plan).FirstOrDefaultAsync(x => x.BlogPostsAnVtid == code);
			return blogPost ?? new();
		}

		public async Task<List<BlogPostsAnVt>> SearchAsync(int? planId = null, string title = null, string category = null, string tags = null, bool? isPublic = null)
		{
			return await _context.BlogPostsAnVts
				.Include(x => x.User)
				.Include(x => x.Plan)
				.Where(x =>
					(!planId.HasValue || x.PlanId == planId.Value) &&
					(string.IsNullOrWhiteSpace(title) || x.Title.Contains(title)) &&
					(string.IsNullOrWhiteSpace(category) || x.Category.Contains(category)) &&
					(string.IsNullOrWhiteSpace(tags) || x.Tags.Contains(tags)) &&
					(!isPublic.HasValue || x.IsPublic == isPublic.Value)
				)
				.ToListAsync();

			/*var query = _context.BlogPostsAnVts
		.Include(x => x.User)
		.Include(x => x.Plan)
		.AsQueryable();

		if (planId.HasValue)
			query = query.Where(x => x.PlanId == planId.Value);

		if (!string.IsNullOrWhiteSpace(title))
			query = query.Where(x => x.Title.Contains(title));

		if (!string.IsNullOrWhiteSpace(category))
			query = query.Where(x => x.Category.Contains(category));

		if (!string.IsNullOrWhiteSpace(tags))
			query = query.Where(x => x.Tags.Contains(tags));

		if (isPublic.HasValue)
			query = query.Where(x => x.IsPublic == isPublic.Value);

		return await query.ToListAsync();*/
		}

		public async Task<PagedResult<BlogPostsAnVt>> SearchWithPagingAsync(BlogPostSearchRequest request)
		{
			var query = _context.BlogPostsAnVts
				.Include(x => x.User)
				.Include(x => x.Plan)
				.Where(x =>
					(!request.PlanId.HasValue || x.PlanId == request.PlanId.Value) &&
					(string.IsNullOrWhiteSpace(request.Title) || x.Title.Contains(request.Title)) &&
					(string.IsNullOrWhiteSpace(request.Category) || x.Category.Contains(request.Category)) &&
					(string.IsNullOrWhiteSpace(request.Tags) || x.Tags.Contains(request.Tags)) &&
					(!request.IsPublic.HasValue || x.IsPublic == request.IsPublic.Value)
				);

			var totalCount = await query.CountAsync();

			var items = await query
				.Skip((request.PageNumber - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return new PagedResult<BlogPostsAnVt>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			};
		}

	}
}
