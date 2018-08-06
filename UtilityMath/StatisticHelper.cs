using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{



    public static class Statistics
    {

        public static double Variance(this IEnumerable<double> source)
        {
            int n = 0;
            double mean = 0;
            double M2 = 0;

            foreach (double x in source)
            {
                n = n + 1;
                double delta = x - mean;
                mean = mean + delta / n;
                M2 += delta * (x - mean);
            }
            return M2 / (n - 1);
        }



        public static IEnumerable<Tuple<decimal, decimal>> ToHistogram(this IEnumerable<Tuple<decimal, decimal>> dt, double binSize, int binCount, int min = 0)
        {



            //if (binSize*binCount+min)
            var ranges = Enumerable.Range(min, min + binCount).Select(x => min + (double)x * binSize);

            //var ranges = Enumerable.Range(1, 200).Select(x => (double)x / 10);


            var combi = dt
               .GroupBy(i => ranges.FirstOrDefault(r => r > Convert.ToDouble(i.Item1)))
               .OrderByDescending(rt => rt.Key)
               .Reverse()
               .Select(g => new Tuple<decimal, decimal>((decimal)g.Key, (decimal)g.Average(km => Convert.ToDouble(km.Item2))));



            return combi;



        }

    }









}


