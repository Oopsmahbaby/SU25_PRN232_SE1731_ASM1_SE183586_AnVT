using SmokeQuit.Repositories.AnVT;
using SmokeQuit.Repositories.AnVT.Models;

namespace SmokeQuit.Services.AnVT
{
	public interface IQuitPlansAnhDtn_AnVTService
	{
			Task<List<QuitPlansAnhDtn>> GetAllAsync();
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
	}
}
