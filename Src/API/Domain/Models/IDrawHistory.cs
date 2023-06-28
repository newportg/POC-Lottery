using System.Collections.Generic;

namespace Domain.Models
{
    public interface IDrawHistory
    {
        public List<LotteryDto> ThunderBall();
        public List<LotteryDto> Lotto();
    }
}
