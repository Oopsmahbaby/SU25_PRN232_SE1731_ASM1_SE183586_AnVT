using SmokeQuit.Repositories.AnVT;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Services.AnVT
{
	public interface IQuitPlansAnhDtn_AnVTService
	{
		Task<List<QuitPlansAnhDtn>> GetAllAsync();
		Task<QuitPlansAnhDtn?> GetByIdAsync(int id);
		Task<QuitPlansAnhDtn?> GetByUserIdAsync(int userId);
		Task<int> AddAsync(QuitPlansAnhDtn quitPlan);
		Task<int> UpdateAsync(QuitPlansAnhDtn quitPlan);
		Task<bool> DeleteAsync(int id);
	}

	public class QuitPlansAnhDtn_AnVTService : IQuitPlansAnhDtn_AnVTService
	{
		private readonly QuitPlansAnhDtn_AnVTRepository _quitPlansAnhDtn_AnVTRepository;
		public QuitPlansAnhDtn_AnVTService()
		{
			_quitPlansAnhDtn_AnVTRepository ??= new();
		}
		public QuitPlansAnhDtn_AnVTService(QuitPlansAnhDtn_AnVTRepository quitPlansAnhDtn_AnVTRepository)
		{
			_quitPlansAnhDtn_AnVTRepository = quitPlansAnhDtn_AnVTRepository;
		}
		public Task<List<QuitPlansAnhDtn>> GetAllAsync()
		{
			return _quitPlansAnhDtn_AnVTRepository.GetAllAsync();
		}

		public async Task<QuitPlansAnhDtn?> GetByIdAsync(int id)
		{
			return await _quitPlansAnhDtn_AnVTRepository.GetByIdAsync(id);
		}

		public async Task<QuitPlansAnhDtn?> GetByUserIdAsync(int userId)
		{
			var plans = await _quitPlansAnhDtn_AnVTRepository.GetAllAsync();
			return plans.FirstOrDefault(p => p.UserId == userId);
		}

		public async Task<int> AddAsync(QuitPlansAnhDtn quitPlan)
		{
			return await _quitPlansAnhDtn_AnVTRepository.CreateAsync(quitPlan);
		}

		public async Task<int> UpdateAsync(QuitPlansAnhDtn quitPlan)
		{
			return await _quitPlansAnhDtn_AnVTRepository.UpdateAsync(quitPlan);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var quitPlansAnhDtn = await _quitPlansAnhDtn_AnVTRepository.GetByIdAsync(id);
			return await _quitPlansAnhDtn_AnVTRepository.RemoveAsync(quitPlansAnhDtn);
		}
	}
}
