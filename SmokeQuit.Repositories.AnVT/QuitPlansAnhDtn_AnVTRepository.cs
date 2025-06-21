using Microsoft.EntityFrameworkCore;
using SmokeQuit.Repositories.AnVT.Basic;
using SmokeQuit.Repositories.AnVT.DBContext;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Repositories.AnVT
{
	public class QuitPlansAnhDtn_AnVTRepository : GenericRepository<QuitPlansAnhDtn>
	{
		public QuitPlansAnhDtn_AnVTRepository()
		{
			
		}

		public QuitPlansAnhDtn_AnVTRepository(SU25_PRN232_SE1731_G6_SmokeQuitContext context)
		{
			_context = context;
		}

		//public async Task<List<QuitPlansAnhDtn>> GetAllAsync()
		//{
		//	var quitPlans = await _context.QuitPlansAnhDtns.Include(x => x.User).ToListAsync();
		//	return quitPlans ?? new();
		//}
	}
}
