using Flurl;
using Flurl.Http;
using Lottery.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lottery.Data
{
    public interface IThunderBallService
    {
        Task<List<Lottery>> GetThunderballAsync();
        Task<List<Lottery>> GetDrawbyDrawNumberAsync(string drawnumber);
        Task<string> UpdateDrawAsync();
        Task<Dictionary<int, int>> GetHotBallsAsync();
        Task<Dictionary<int, int>> GetDrawTotalsAsync();
        Task<Dictionary<int, int>> GetDeltasAsync();
        Task<Dictionary<int, TotalThunder>> DrawTotalByThunderBall();
        Task<List<Ticket>> GetTicketsAsync();
        Task<bool> SaveGuesses(List<Ticket> tickets);
        Task<List<Ticket>> GetLastGuessesAsync();
        Task<List<Ticket>> GetGuessesByDrawNumberAsync(string drawnumber);
        Task<DrawResult> GetDrawResultAsync(string drawnumber);
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

        public async Task<List<Lottery>> GetDrawbyDrawNumberAsync(string drawnumber)
        {
            //return await Task.FromResult(await repository.GetList());
            //return await repository.GetList();

            Url url = new($"https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/{drawnumber}");
            var data = await url.GetJsonAsync<List<Lottery>>();
            return data;
        }

        public async Task<string> UpdateDrawAsync()
        {
            //return await Task.FromResult(await repository.GetList());
            //return await repository.GetList();

            Url url = new($"https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/Update");
            var data = await url.GetStringAsync();
            return data;
        }

        public async Task<Dictionary<int, int>> GetHotBallsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/HotBalls");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<Dictionary<int, int>> GetDrawTotalsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/Totals");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<Dictionary<int, TotalThunder>> DrawTotalByThunderBall()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/TotalsByThunderBall");
            var data = await url.GetJsonAsync<Dictionary<int, TotalThunder>>();
            return data;
        }

        public async Task<Dictionary<int, int>> GetDeltasAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Draw/Deltas");
            var data = await url.GetJsonAsync<Dictionary<int, int>>();
            return data;
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Guess/Nextdraw");
            var data = await url.GetJsonAsync<List<Ticket>>();
            return data;
        }

        public async Task<bool> SaveGuesses(List<Ticket> tickets)
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Guess/Save");
            var res = await url.PostJsonAsync(tickets);
            if( res != null && res.StatusCode == 200) 
            { 
                return true; 
            }
            return false;
        }

        public async Task<List<Ticket>> GetLastGuessesAsync()
        {
            Url url = new("https://func-poc-lottery-vse-ne.azurewebsites.net/api/Guess/Last");
            var data = await url.GetJsonAsync<List<Ticket>>();
            return data;
        }

        public async Task<List<Ticket>> GetGuessesByDrawNumberAsync(string drawnumber)
        {
            Url url = new($"https://func-poc-lottery-vse-ne.azurewebsites.net/api/Guess/{drawnumber}");
            var data = await url.GetJsonAsync<List<Ticket>>();
            return data;
        }

        public async Task<DrawResult> GetDrawResultAsync(string drawnumber)
        {
            Url url = new($"https://func-poc-lottery-vse-ne.azurewebsites.net/api/Guess/Result/{drawnumber}");
            var data = await url.GetJsonAsync<DrawResult>();
            return data;
        }
    }

}