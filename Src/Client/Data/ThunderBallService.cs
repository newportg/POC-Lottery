using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Data
{
    public interface IThunderBallService
    {
        Task<List<ThunderBall>> GetThunderballAsync();

        Task<ThunderBall[]> GetListAsync();
    }

    public class ThunderBallService : IThunderBallService
    {
        private readonly IAzureTableStorage<ThunderBall> repository;
        public ThunderBallService(IAzureTableStorage<ThunderBall> repository)
        {
            this.repository = repository;
        }

        public async Task<List<ThunderBall>> GetThunderballAsync()
        {
            //return await Task.FromResult(await repository.GetList());
            return await repository.GetList();
        }

        public async Task<ThunderBall[]> GetListAsync()
        {
            var a = await repository.GetList();
            return a.ToArray();
        }
    }
}