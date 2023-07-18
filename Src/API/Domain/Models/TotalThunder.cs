using System.Collections.Generic;
using System.Linq;

namespace Domain.Models
{
    public class TotalThunder
    {
        public TotalThunder(int count, int thunderball)
        {
            Count = count;
            Thunderball = new List<int>();
            Thunderball.Add(thunderball);
        }

        public int Count { get; set; }
        public List<int> Thunderball { get; set; }

        public void Update(int thunderball)
        {
            this.Count++;
            var item = Thunderball.Count(item => item == thunderball);
            if(item == 0)
            {
                Thunderball.Add(thunderball);
            }

        }
    }
}
