using System;
using System.Collections.Generic;
using System.Linq;

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

        public static double StandardDeviation(IEnumerable<double> values)
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

        public static Tuple<double, double> WeightedAverage(this Tuple<double, double> norm, Tuple<double, double> tup)
        {
            // inverser of variance
            var v_1 = (norm.Item2 + tup.Item2);
            // mean over variance
            var m_v = (norm.Item1 * norm.Item2) + (tup.Item1 * tup.Item2);

            // mean equals mean over variance over inverse of variance
            return new Tuple<double, double>(m_v / v_1, v_1);
        }

        //

        public static Tuple<double, double> WeightedAverage(this IEnumerable<Tuple<double, double>> tups, bool meanAtIndex0 = true)
        {
            // inverser of variance
            var x = tups.GetEnumerator();
            double v_1 = 0, m_v = 0;

            while (x.MoveNext())
            {
                v_1 += (meanAtIndex0) ?
                    x.Current.Item2 : x.Current.Item1;
                // mean over variance
                m_v += x.Current.Item1 * x.Current.Item2;
            }

            // mean equals mean over variance over inverse of variance
            return new Tuple<double, double>(m_v / v_1, v_1);
        }
    }
}