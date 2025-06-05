using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokeQuit.Repositories.AnVT.ModelExtensions
{
	public class PagedResult<T>
	{
		public List<T> Items { get; set; } = new();
		public int TotalCount { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
	}
}
