using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UtilityMath.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var vals = System.Linq.Enumerable.Range(0, 40).Select(_ => System.Tuple.Create((((double)(_ - 2)) / 10), NORMSDIST((((double)(_ - 2)) / 10)))).ToArray();

            var h = UtilityMath.Histogram.ToHistogramByBinCount(vals, 10)/*.Select(_ => Tuple.Create((_.Key.Item1 + _.Key.Item2) / 2, _.Value))*/.ToList();

            var ss = h.SelectMany(_ => Enumerable.Range((int)_.Key.Item1, (int)_.Key.Item2).Select(s => System.Tuple.Create(s, _.Value)));

            ConsolePlotting.Program.Plot(ss.Select(_ => _.Item1).ToArray(), ss.Select(_ => _.Item2).ToArray());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var vals = System.Linq.Enumerable.Range(0, 40).Select(_ => System.Tuple.Create((((double)(_ - 2)) / 10), NORMSDIST((((double)(_ - 2)) / 10)))).ToArray();

            var h = UtilityMath.Histogram.ToHistogram(vals, 10)/*.Select(_ => Tuple.Create((_.Key.Item1 + _.Key.Item2) / 2, _.Value))*/.ToList();

            var ss = h.SelectMany(_ => Enumerable.Range((int)_.Key.Item1, (int)_.Key.Item2).Select(s => System.Tuple.Create(s, _.Value)));

            ConsolePlotting.Program.Plot(ss.Select(_ => _.Item1).ToArray(), ss.Select(_ => _.Item2).ToArray());
        }

        private static double NORMSDIST(double Zscore)
        {
            double Z = -(Zscore * Zscore) / 2;
            double normDist = (1 / System.Math.Sqrt(2 * System.Math.PI)) * (System.Math.Exp(Z));

            return normDist;
        }
    }
}