using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityMath
{
    public static class Normaliser
    {
        public static IList<double> Normalise01(IList<double> target)
        {
            var max = target.Max();
            var min = target.Min();
            double diff = max - min;

            return target.Select(_ => (_ - min) / (diff)).ToList();
        }

        public static IList<double> Normalise(IList<double> target)
        {
            double sum = target.Sum();

            return target.Select(_ => (_ / sum) + Double.Epsilon).ToList();
            // avoid round-off to zero
        }

        public static IList<double[]> Normalise(IList<double[]> target)
        {
            double[] sum = new double[target.First().Length];

            for (int i = 0; i < target.First().Length; i++)
            {
                sum[i] = target.Select(_ => _[i]).Sum();
            }

            return target.Select(_ => _.Select((__, i) => (__ / sum[i]) + Double.Epsilon).ToArray()).ToList();
        }

        public static IList<Matrix<double>> Normalise(IList<Matrix<double>> target)
        {
            var sum = target.Aggregate((a, b) => a + b);

            return target.Select(_ => _.PointwiseDivide(sum) + Double.Epsilon).ToList();
        }

        public static double[] CalculateSoftMax(IEnumerable<double> values)
        {
            var ex = values.Sum(_ => System.Math.Exp(_));
            return values.Select(_ => System.Math.Exp(_) / ex).ToArray();
        }

        public static double[] Normalise(double[] target)
        {
            double sum = target.Sum();

            return target.Select(_ => (_ / sum) + Double.Epsilon).ToArray();
            // avoid round-off to zero
        }
    }
}