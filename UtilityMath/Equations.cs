using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{

    public static class NoisyIterations
    {


        public static IEnumerable<double> Sin(double factor, double sigma, Random rand)
        {
            int i = 0;
            while (true)
            {
                yield return factor * (Math.Sin(i) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));
                i++;
            }
        }

        public static IEnumerable<double> Cos(double factor, double sigma, Random rand)
        {

            int i = 0;
            while (true)
            {
                yield return factor * (Math.Cos(i) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));
                i++;
            }

        }
        public static IEnumerable<double> Exp(double factor, double sigma, Random rand)
        {
            int i = 0;
            while (true)
            {
                yield return factor * (Math.Exp(i) + MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma));
                i++;
            }


        }

        //http://scipy-cookbook.readthedocs.io/items/BrownianMotion.html
        public static IEnumerable<double> Brownian(double factor, double sigma, Random rand)
        {
            double value = factor * MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma);
            while (true)
            {
                yield return value += factor * MathNet.Numerics.Distributions.Normal.Sample(rand, 0, sigma);
            }

        }
    }

    //public static class NoisyEquation
    //{


    //    public static Func<int, double> Sin(double factor, double sigma, Random rand)
    //    {
    //        return (i) => factor * (Math.Sin(i * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

    //    }

    //    public static Func<int, double> Cos(double factor, double sigma, Random rand)
    //    {
    //        return (i) => factor * (Math.Cos(i * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

    //    }
    //    public static Func<int, double> Exp(double factor, double sigma, Random rand)
    //    {
    //        return (i) => factor * (Math.Exp(i) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

    //    }
    //    public static Func<int,double, double> Brownian(double factor, double sigma,Random rand)
    //    {
    //        return (i,a) => a + factor * MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma);

    //    }
    //}
}