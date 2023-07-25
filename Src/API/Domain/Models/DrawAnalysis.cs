using System;
using System.Collections.Generic;
using System.Numerics;

namespace Domain.Models
{
    public class DrawAnalysis
    {
        List<Lottery> Draws { get; set; }
        public int[,] RGspread { get; set; }
        public int[,] RGguess { get; set; }

        public DrawAnalysis(List<Lottery> draws)
        {
            if(draws == null)
                throw new ArgumentNullException(nameof(draws));
            this.Draws = draws;

            RGspreadCalc();
            RGGuessCalc();
        }


        private void RGspreadCalc()
        {
            if( Draws == null) { return; }

            int[,] cnt = new int[5, 5];
            foreach (var draw in Draws)
            {
                for (int i = 0; i < 5; i++)
                {
                    var val = draw.RenatoGianellaPattern[i];
                    if (val == 0) cnt[i, 0]++;
                    else if (val == 1) cnt[i, 1]++;
                    else if (val == 2) cnt[i, 2]++;
                    else if (val == 3) cnt[i, 3]++;
                    else if (val == 4) cnt[i, 4]++;
                }
            }

            RGspread = cnt;
            return;
        }

        private void RGGuessCalc()
        {
            // We make 9 guesses
            // So if the RG spread is 8 numbers per column
            // column 0 being 1,2,3,4,5,6,7,8
            // column 1 being 9,10,11,12,13,14,15,16 etc..
            //
            // Take the RGSpread per column, say colum zero is 60%, 30%, 5%, 4%, 1%
            // Any percentage < 10% ignore.
            // Then select proportionately that number of balls from that column

            if (RGspread == null || Draws == null) { return; }

            int[,] cnt = new int[5,5];

            // Get the total number of Draws
            var total = Draws.Count;

            // Loop through the RGspread matrix
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    // Get the value at the current position in the RGspread matrix
                    var zero = this.RGspread[i, j];

                    // Calculate the percentage for the current value and distribute it to the cnt matrix
                    decimal percentage = (zero * 100) / total;
                    var val = (int)Math.Round((decimal)percentage);

                    // If the calculated value is greater than 10, update the corresponding position in the cnt matrix
                    if (val > 10)
                    {
                        cnt[i, j] = val;
                    }
                }
            }

            // Work out the number of Guess rows
            for (int i = 0; i < 5; i++)
            {
                // Get individual counts for each category
                var one = cnt[i, 0];
                var two = cnt[i, 1];
                var thr = cnt[i, 2];
                var fou = cnt[i, 3];
                var fiv = cnt[i, 4];

                // Calculate the total count
                var totalg = one + two + thr + fou + fiv;
                var totalRows = 0;

                // Calculate the percentage for each category and distribute it to nine rows
                for (int j = 0; j < 5; j++)
                {
                    decimal percentage = (cnt[i, j] * 100) / totalg;
                    cnt[i, j] = (int)Math.Ceiling((decimal)9 / 100 * percentage);

                    totalRows += cnt[i, j];
                    if( totalRows > 9)
                    {
                        cnt[i, j]--;
                        totalRows--;
                    }

                }
            }
            RGguess = cnt;
            return;
        }
    }
}
