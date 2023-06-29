using Lottery.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lottery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThunderBallController : Controller
    {
        private readonly IThunderBallService service;
        public ThunderBallController(IThunderBallService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<List<Lottery.Data.Lottery>> Get()
        {
            var model = await service.GetThunderballAsync();
            return model.ToList();
        }
    }
}