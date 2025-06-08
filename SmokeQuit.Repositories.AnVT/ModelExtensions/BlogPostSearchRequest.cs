namespace SmokeQuit.Repositories.AnVT.ModelExtensions
{
	public class BlogPostSearchRequest
	{
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int? PlanId { get; set; }
		public string? Title { get; set; }
		public string? Category { get; set; }
		public string? Tags { get; set; }
		public bool? IsPublic { get; set; }
	}
}
