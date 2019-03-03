using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath.Statistics
{
    //Consider LinqStatistics Package
    public static class Statistics
    {

        public static IEnumerable<double> RollingVariance(this IEnumerable<double> source)
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
                yield return M2 / (n - 1);
            }

        }



        public static double StdDev(IEnumerable<double> values)
        {

            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                return Math.Sqrt((sum) / (values.Count() - 1));
            }
            else
                return 0;
        }



        public static double WeightedAverage<T>(IEnumerable<T> records, Func<T, double> value, Func<T, double> weight, double control = 0)
        {
            double weightedValueSum = records.Sum(x => (value(x) - control) * weight(x));
            double weightSum = records.Sum(x => weight(x));

            if (weightSum != 0)
                return weightedValueSum / weightSum;
            else
                throw new DivideByZeroException("Divide by zero exception calculating weighted average");
        }

    }



    public struct Sample
    {
        public int Size { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
    }






}


