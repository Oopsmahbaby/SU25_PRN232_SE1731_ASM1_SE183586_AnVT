using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repositories.AnVT.DTOs
{
	public class BlogPostsAnVtDto
	{
		public int UserId { get; set; }

		public int? PlanId { get; set; }

		public string? Title { get; set; }

		public string? Content { get; set; }

		public string? Category { get; set; }

		public string? Tags { get; set; }

		public bool IsPublic { get; set; }

		public int? ViewsCount { get; set; }

		public int? LikesCount { get; set; }

		public int? CommentsCount { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
