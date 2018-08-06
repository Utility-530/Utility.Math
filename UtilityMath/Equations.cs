using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{



    //public static class EquationGenerator
    //{

    //    private static Func<int, int, Func<int, double>>[] f = new Func<int, int, Func<int, double>>[]
    //    {
    //    (sigma, factor) => (i) =>{ double sigma = rand.NextDouble(); return sigma + factor * Math.Sin(i * 3.14 * 10 / 180); },
    //    (sigma, factor) => (i) =>{   double sigma = rand.NextDouble(); return sigma + factor * Math.Cos(i * 3.14 * 10 / 180); },
    //    (maxdeviation, factor) => (i) => {   double sigma = rand.NextDouble(); return sigma + factor; }

    //    };



    //}







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