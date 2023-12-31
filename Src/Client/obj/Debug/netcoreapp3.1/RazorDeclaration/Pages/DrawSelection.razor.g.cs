// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Lottery.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Lottery;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\_Imports.razor"
using Lottery.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\Pages\DrawSelection.razor"
using Lottery.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Components.RouteAttribute("/drawselection")]
    public partial class DrawSelection : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 91 "C:\Users\NEWPORTG\source\Azure\FunctionMonkey\FMLottery\Lottery\Pages\DrawSelection.razor"
 
    List<int[]> tickets = new List<int[]>();
    List<KeyValuePair<int, int>> hot = new List<KeyValuePair<int, int>>();
    IList<ThunderBall> items = new List<ThunderBall>();
    private bool IsSortedAscending;
    private string CurrentSortColumn;
    string hotBalls;
    string hotBallsLinear;
    string drawTotals;
    string deltas;

    protected override async Task OnInitializedAsync()
    {
        items = await ThunderBallService.GetThunderballAsync();

        var ltball = new List<Thunderball>();
        foreach (ThunderBall entity in items)
        {
            // Access the entity properties
            string partitionKey = entity.PartitionKey;
            string rowKey = entity.RowKey;
            // Access other properties of the entity

            ltball.Add(new Thunderball(entity.DrawNumber, entity.Ball1, entity.Ball2, entity.Ball3, entity.Ball4, entity.Ball5, entity.BallSet, entity.DrawDate, entity.Thunderball));
        }

        var td = new ThunderDraws(ltball);

        // Draw selection should be :-
        // 1, Not have consecutive numbers
        // 2, Not have cold numbers
        // 2, Not been drawn before
        // 3, Total should be within the middle of the Total distribution

        tickets = td.GenerateTickets();

        hotBalls = HotBalls(td);
        hotBallsLinear = HotBallsLinear(td);
        drawTotals = DrawTotals(td);
        deltas = Deltas(td);

    }

    public string HotBalls(ThunderDraws td)
    {
        string hotBalls;
        var dic = td.HotBalls();
        hot = dic.OrderBy(x => x.Key).ToList();

        // Whats the lowest hot value
        // Whats the highest hit value
        var val = dic.OrderBy(x => x.Value).ToList();
        var low = val[0].Value;
        var high = val[val.Count - 1].Value;

        hotBalls = "<table class='table-bordered'><tbody>";
        for (int i = 0, j = 0; i < 39 && j < 5; i++, j++)
        {
            if (j == 0)
                hotBalls += "<tr>";

            hotBalls += $"<td bgcolor=#ff{MapNumberToRange(hot[i].Value, low, high, 0, 255)}00>{(hot[i].Key)}</td>";
            if (j == 4)
            {
                hotBalls += "</tr>";
                j = -1;
            }
        }
        hotBalls += "</tr></tbody></table>";

        return hotBalls;
    }

    public string HotBallsLinear(ThunderDraws td)
    {
        string hotBalls;
        var dic = td.HotBalls();
        hot = dic.OrderBy(x => x.Key).ToList();

        // Whats the lowest hot value
        // Whats the highest hit value
        var val = dic.OrderBy(x => x.Value).ToList();
        var low = val[0].Value;
        var high = val[val.Count - 1].Value;

        hotBalls = "<table class='table-bordered'><tbody><tr>";
        foreach( var item in val)
        {
            hotBalls += $"<td bgcolor=#ff{MapNumberToRange(item.Value, low, high, 0, 255)}00>{(item.Key)}</td>";
        }
        hotBalls += "</tr></tbody></table>";

        return hotBalls;
    }

    public string DrawTotals(ThunderDraws td)
    {
        string drawTotals;
        var dic = td.DrawTotals();

        var ord = dic.OrderBy(x => x.Key).ToList();
        var middle = ord.Count() /5 ;

        drawTotals = "<table class='table-bordered'><thead><tr><td>Count</td><td>Totals</td></tr></thead><tbody>";

        foreach (var entry in ord)
        {
            int count = entry.Value;
            int number = entry.Key;
            string distribution = new string('*', count);

            var idx = ord.IndexOf(entry);
            if (idx > middle && idx < middle * 4)
                drawTotals += $"<tr bgcolor=#00ff00><td>{number}</td><td>{distribution}</td></tr>";
            else
                drawTotals += $"<tr><td>{number}</td><td>{distribution}</td></tr>";
        }

        //var groupedEntries = dic.GroupBy(entry => entry.Value).OrderBy(x => x.Key);  // Group the dictionary entries by count
        //foreach (var group in groupedEntries)
        //{
        //    int count = group.Key;
        //    var numbers = group.Select(entry => entry.Key);
        //    string numbersString = string.Join(",", numbers);

        //    drawTotals += $"<tr><td>{count}</td><td>{numbersString}</td></tr>";
        //}

        drawTotals += "</tbody></table>";
        return drawTotals;

    }

    private string MapNumberToRange(int number, int inputMin, int inputMax, int outputMin, int outputMax)
    {
        // Ensure the number is within the input range
        number = Math.Max(inputMin, Math.Min(inputMax, number));

        // Calculate the size of the input and output ranges
        int inputRangeSize = inputMax - inputMin;
        int outputRangeSize = outputMax - outputMin;

        // Calculate the gap ratio between the input and output ranges
        double gapRatio = (double)outputRangeSize / inputRangeSize;

        // Calculate the mapped value
        //byte mappedValue = (byte)((number - inputMin) * gapRatio + outputMin);
        byte mappedValue = (byte)((inputMax - number) * gapRatio + outputMin);

        string hexValue = mappedValue.ToString("X2");

        return hexValue;
    }

    private string GetSortStyle(string columnName)
    {
        if (CurrentSortColumn != columnName)
        {
            return string.Empty;
        }
        if (IsSortedAscending)
        {
            return "fa-sort-up";
        }
        else
        {
            return "fa-sort-down";
        }
    }

    private void SortTable(string columnName)
    {
        //Sorting against a column that is not currently sorted against.
        if (columnName != CurrentSortColumn)
        {
            //We need to force order by ascending on the new column
            //This line uses reflection and will probably perform inefficiently in a production environment.
            items = items.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            CurrentSortColumn = columnName;
            IsSortedAscending = true;

        }
        else //Sorting against same column but in different direction
        {
            if (IsSortedAscending)
            {
                items = items.OrderByDescending(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            }
            else
            {
                items = items.OrderBy(x => x.GetType().GetProperty(columnName).GetValue(x, null)).ToList();
            }

            //Toggle this boolean
            IsSortedAscending = !IsSortedAscending;
        }
    }

    public string Deltas(ThunderDraws td)
    {
        string deltas;
        var dic = td.Deltas();
        var ord = dic.OrderBy(x => x.Key).ToList();
        var totalcount = 0;
        foreach (var entry in ord)
        {
            totalcount += entry.Value;
        }

        deltas = "<table class='table-bordered'><thead><tr><td>Count</td><td>Percent</td><td>Totals</td></tr></thead><tbody>";


        foreach (var entry in ord)
        {
            int count = entry.Value;
            int number = entry.Key;
            decimal percentage = (decimal)Math.Round((double)(100 * count) / totalcount);

            string distribution = new string('*', count);

            deltas += $"<tr><td>{number}</td><td>{percentage}</td><td>{distribution}</td></tr>";
        }

        deltas += "</tbody></table>";
        return deltas;
    }

    public class ThunderDraws
    {
        int totalNumbers = 39; // Total numbers in the lottery
        int numbersToPick = 5; // Numbers to pick for each ticket
        int ticketsToGenerate = 9; // Number of tickets to generate

        List<Thunderball> previousdraws { get; set; }
        List<int[]> tickets = new List<int[]>();
        Dictionary<int, int> drawtotals = new Dictionary<int, int>();
        Dictionary<int, int> deltas = new Dictionary<int, int>();

        public ThunderDraws(List<Thunderball> draws)
        {
            previousdraws = draws;
        }

        public Dictionary<int, int> HotBalls()
        {
            Dictionary<int, int> hot = new Dictionary<int, int>();

            foreach (var draw in previousdraws)
            {
                foreach (var ball in draw.Balls)
                {
                    if (hot.ContainsKey(ball))
                    {
                        hot[ball]++;
                    }
                    else
                    {
                        hot[ball] = 1;
                    }
                }
            }

            return hot;
        }

        public List<int[]> GenerateTickets()
        {
            List<int[]> tickets = new List<int[]>();

            //var dic = DrawTotals();
            //var ord = dic.OrderBy(x => x.Key).ToList();
            //var dislower = ord.Count() / 5;
            //var dishigher = dislower*4;

            // Generate all possible combinations of numbers
            IEnumerable<IEnumerable<int>> combinations = Enumerable.Range(1, totalNumbers)
                .DifferentCombinations(numbersToPick);

            Random random = new Random();

            // Randomly select tickets from the combinations
            for (int i = 0; i < ticketsToGenerate; i++)
            {
                int[] ticket;
                bool cons;
                bool drawn;
                do
                {
                    ticket = combinations.ElementAt(random.Next(combinations.Count())).ToArray();

                    cons = HasConsecutiveNumbers(ticket);
                    drawn = HasBeenDrawn(ticket);

                    if (cons == true)
                        Console.WriteLine("Rejected - " + string.Join(", ", ticket));
                    if (drawn == true)
                        Console.WriteLine("Previously Drawn - " + string.Join(", ", ticket));

                } while (cons && !drawn);

                // Add ThunderBall
                Random rnd = new Random();
                int[] tic = new int[7];
                for (int j = 0; j < ticket.Length; j++)
                {
                    tic[j] = ticket[j];
                }
                tic[5] = rnd.Next(1, 14);
                tic[6] = tic[0] + tic[1] + tic[2] + tic[3] + tic[4] + tic[5];


                //if (tic[6] > dislower && tic[6] < dishigher)
                //{
                //    tickets.Add(tic);
                //}
                //else
                //{
                //    i--; // lets go round again
                //}

                tickets.Add(tic);
            }

            return tickets;
        }

        private bool HasBeenDrawn(int[] ticket)
        {
            for (int i = 0; i < previousdraws.Count; i++)
            {
                return previousdraws[i].HasBeenDrawn(ticket);
            }
            return false;
        }

        private bool HasConsecutiveNumbers(int[] numbers)
        {
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                if (numbers[i] == numbers[i + 1] - 1)
                {
                    return true;
                }
            }

            return false;
        }

        public Dictionary<int, int> DrawTotals()
        {
            if (drawtotals.Count != 0)
                return drawtotals;

            foreach (var draw in previousdraws)
            {
                if (drawtotals.ContainsKey(draw.DrawTotal()))
                {
                    drawtotals[draw.DrawTotal()]++;
                }
                else
                {
                    drawtotals[draw.DrawTotal()] = 1;
                }
            }
            return drawtotals;
        }

        public Dictionary<int, int> Deltas()
        {
            if (deltas.Count != 0)
                return deltas;

            foreach (var draw in previousdraws)
            {
                foreach (var ball in draw.Delta)
                {
                    if (deltas.ContainsKey(ball))
                    {
                        deltas[ball]++;
                    }
                    else
                    {
                        deltas[ball] = 1;
                    }
                }
            }

            return deltas;
        }
    }

    public class Thunderball
    {
        public Thunderball(string drawNumber, String ball1, String ball2, String ball3, String ball4, String ball5, String ballset, String drawdate, String tball)
        {
            DrawNumber = drawNumber;
            Balls[0] = int.Parse(ball1);
            Balls[1] = int.Parse(ball2);
            Balls[2] = int.Parse(ball3);
            Balls[3] = int.Parse(ball4);
            Balls[4] = int.Parse(ball5);

            SortedBalls = Balls;
            Array.Sort(SortedBalls);
  
            Delta[0] = Balls[1] - Balls[0];
            Delta[1] = Balls[2] - Balls[1];
            Delta[2] = Balls[3] - Balls[2];
            Delta[3] = Balls[4] - Balls[3];

            BallSet = ballset;
            DrawDate = drawdate;
            ThunderBall = int.Parse(tball);
        }

        public string DrawNumber { get; set; }
        public int[] Balls { get; set; } = new int[5];
        public int[] SortedBalls { get; set; } = new int[5];
        public int[] Delta { get; set; } = new int[4];
        public string BallSet { get; set; } = string.Empty;
        public string DrawDate { get; set; } = string.Empty;
        public int ThunderBall { get; set; }

        public int DrawTotal() { return Balls[0] + Balls[1] + Balls[2] + Balls[3] + Balls[4] + ThunderBall; }

        public bool HasBeenDrawn(int[] ticket)
        {
            if (ticket.Contains<int>(Balls[0]) && ticket.Contains<int>(Balls[1]) && ticket.Contains<int>(Balls[2]) && ticket.Contains<int>(Balls[3]) && ticket.Contains<int>(Balls[4]))
            {
                return true;
            }
            return false;
        }

        public string Print()
        {
            return string.Join(", ", Balls);
        }

        public string PrintDelta()
        {
            return string.Join(", ", Delta);
        }

    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IThunderBallService ThunderBallService { get; set; }
    }
}
#pragma warning restore 1591
