using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityMath
{
    public class NoisyValues
    {
        private readonly double factor;
        private readonly double sigma;
        private readonly Random rand;

        public NoisyValues(double factor, double sigma, Random rand)
        {
            this.factor = factor;
            this.sigma = sigma;
            this.rand = rand;
        }

        public IEnumerable<(int x, double y, double noisyY)> Sin(int count)
        {
            return Values(NoisyIteration.Sin(factor, sigma, rand), count);
        }

        public IEnumerable<(int x, double y, double noisyY)> Cos(int count)
        {
            return Values(NoisyIteration.Sin(factor, sigma, rand), count);
        }

        public IEnumerable<(int x, double y, double noisyY)> Exp(int count)
        {
            return Values(NoisyIteration.Sin(factor, sigma, rand), count);
        }

        public IEnumerable<(int x, double y)> Browian(int count)
        {
            return Values(NoisyIteration.Brownian(factor, sigma, rand), count);
        }

        private static IEnumerable<(int x, double y, double noisyY)> Values(Func<int, (double, double)> func, int count)
        {
            return Enumerable.Range(0, count).Select(a => (a, func(a))).Select(a => (a.a, a.Item2.Item1, a.Item2.Item2));
        }

        private static IEnumerable<(int x, double y)> Values(Func<int, double> func, int count)
        {
            return Enumerable.Range(0, count).Select(a => (a, func(a))).Select(a => (a.a, a.Item2));
        }
    }

    public static class NoisyBKernelEquation
    {
        public static Func<double, double, Func<int, double>> Brownian(Random rand)
        {
            return (a, b) => NoisyIteration.Brownian(a, b, rand);
        }
    }

    public static class NoisyIteration
    {
        public static Func<int, (double, double)> Sin(double factor, double sigma, Random rand)
        {
            return (i) => (factor * Math.Sin(i / Math.PI), factor * (Math.Sin(i / Math.PI) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma)));
        }

        public static Func<int, (double, double)> Cos(double factor, double sigma, Random rand)
        {
            return (i) => (factor * Math.Cos(i / Math.PI), factor * (Math.Cos(i / Math.PI) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma)));
        }

        public static Func<int, (double, double)> Exp(double factor, double sigma, Random rand)
        {
            return (i) => (factor * Math.Exp(i), factor * (Math.Exp(i) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma)));
        }

        //http://scipy-cookbook.readthedocs.io/items/BrownianMotion.html
        public static Func<int, double> Brownian(double factor, double sigma, Random rand)
        {
            var sd = new Iterator(a => a + factor * MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));
            return sd.Next;
        }
    }

    internal class Iterator
    {
        private readonly Func<double, double> func;
        private Dictionary<int, double> dictionary = new Dictionary<int, double>();

        public Iterator(Func<double, double> func)
        {
            this.func = func;
        }

        public double Next(int index)
        {
            var last = dictionary.LastOrDefault();
            while (dictionary.ContainsKey(index) == false)
            {
                dictionary.Add(last.Key + 1, func(last.Value));
                last = dictionary.LastOrDefault();
            }
            return last.Value;
        }
    }
}