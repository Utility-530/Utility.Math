using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{

    public static class NoisyKernelEquation
    {


        public static Func<int, int, Func<int, double>>  Sin(Random rand)
        {
            return (a, b) => NoisyIteration.Sin(a, b, rand);

        }

        public static Func<int, int, Func<int, double>> Cos(Random rand)
        {
            return (a, b) => NoisyIteration.Cos(a, b, rand);

        }
        public static Func<int, int, Func<int, double>> Exp(Random rand)
        {
            return (a, b) => NoisyIteration.Exp(a, b, rand);

        }

    }

    public static class NoisyBKernelEquation
    {

        public static Func<int, int, Func<int, double, double>> Brownian( Random rand)
        {
            return (a, b) => NoisyIteration.Brownian(a, b, rand);

        }
    }
    public static class NoisyIteration
    {


        public static Func<int, double> Sin(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Sin(i ) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));

        }

        public static Func<int, double> Cos(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Cos(i  ) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));

        }
        public static Func<int, double> Exp(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Exp(i) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));

        }

        //http://scipy-cookbook.readthedocs.io/items/BrownianMotion.html
        public static Func<int, double, double> Brownian(double factor, double sigma, Random rand)
        {
            return (i, a) => a + factor * MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma);

        }
    }
}
