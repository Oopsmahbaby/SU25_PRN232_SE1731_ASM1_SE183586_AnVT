using SmokeQuit.Repositories.AnVT;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Services.AnVT
{
	public class SystemUserAccountService
	{
		private readonly SystemUserAccountRepository _systemUserAccountRepository;
		public SystemUserAccountService()
		{
			_systemUserAccountRepository ??= new();
		}
		public SystemUserAccountService(SystemUserAccountRepository systemUserAccountRepository)
		{
			_systemUserAccountRepository = systemUserAccountRepository;
		}
		public async Task<SystemUserAccount> GetUserAccountAsync(string userName, string password)
		{
			return await _systemUserAccountRepository.GetUserAccountAsync(userName, password);
		}

		public async Task<List<SystemUserAccount>> GetAllUserAccounts()
		{
			return await _systemUserAccountRepository.GetAllUserAccounts();
		}
	}
}
