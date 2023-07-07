using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Data
{
    public interface IThunderBallService
    {
        Task<List<Lottery>> GetThunderballAsync();
        Task<Dictionary<int, int>> GetHotBallsAsync();
        Task<Dictionary<int, int>> GetDrawTotalsAsync();
        Task<Dictionary<int, int>> GetDeltasAsync();
        Task<List<Ticket>> GetTicketsAsync();
        Task<bool> SaveGuesses(List<Ticket> tickets);
    }

    public class ThunderBallService : IThunderBallService
    {
        public ThunderBallService()
        {

        }

        public async Task<List<Lottery>> GetThunderballAsync()
        {
            //return await Task.FromResult(await repository.GetList());
            //return await repository.GetList();

            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw");
            var data = await url.GetJsonAsync<List<Lottery>>();
            return data;
        }

        public async Task<Dictionary<int, int>> GetHotBallsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/HotBalls");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<Dictionary<int, int>> GetDrawTotalsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/DrawTotals");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<Dictionary<int, int>> GetDeltasAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Deltas");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/GetTickets");
            var data = await url.GetJsonAsync<List<Ticket>>();
            return data;
        }

        public async Task<bool> SaveGuesses(List<Ticket> tickets)
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/SaveTickets");
            var res = await url.PostJsonAsync(tickets);
            if( res != null && res.StatusCode == 200) 
            { 
                return true; 
            }
            return false;
        }
    }

}