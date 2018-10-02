using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{



    public static class NoisyEquation
    {


        public static Func<int, double> Sin(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Sin(i * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

        }

        public static Func<int, double> Cos(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Cos(i * 3.14 / 180) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

        }
        public static Func<int, double> Exp(double factor, double sigma, Random rand)
        {
            return (i) => factor * (Math.Exp(i) + MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma));

        }
        public static Func<int,double, double> Brownian(double factor, double sigma,Random rand)
        {
            return (i,a) => a + factor * MathNet.Numerics.Distributions.Normal.Sample(rand,0, sigma);

        }
    }
}