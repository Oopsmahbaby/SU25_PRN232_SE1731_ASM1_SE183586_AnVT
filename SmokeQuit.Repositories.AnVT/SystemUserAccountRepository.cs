using Microsoft.EntityFrameworkCore;
using SmokeQuit.Repositories.AnVT.Basic;
using SmokeQuit.Repositories.AnVT.DBContext;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Repositories.AnVT
{
	public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
	{
		public SystemUserAccountRepository()
		{
		}
		public SystemUserAccountRepository(SU25_PRN232_SE1731_G6_SmokeQuitContext context)
		{
			_context = context;
		}

		public async Task<SystemUserAccount> GetUserAccountAsync(string userName, string password)
		{
			var systemUserAccounts = await _context.SystemUserAccounts.FirstOrDefaultAsync(x => x.UserName == userName && x.Password == password && x.IsActive == true);
			return systemUserAccounts ?? new();
		}

		public async Task<List<SystemUserAccount>> GetAllUserAccounts()
		{
			return await _context.SystemUserAccounts.ToListAsync();
		}

	}
}
