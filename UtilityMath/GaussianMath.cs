using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{



    public static class NormalExtension
    {

        public static MathNet.Numerics.Distributions.Normal Negative(this MathNet.Numerics.Distributions.Normal sbw)
        {

            return new MathNet.Numerics.Distributions.Normal(-sbw.Mean, sbw.StdDev);
        }



        public static MathNet.Numerics.Distributions.Normal Multiply(this MathNet.Numerics.Distributions.Normal norm1, MathNet.Numerics.Distributions.Normal norm2)
        {

            var v_1 = (1 / norm1.Variance + 1 / norm2.Variance);
            var m_v = (norm1.Mean / norm1.Variance) + (norm2.Mean / norm2.Variance);

            return new MathNet.Numerics.Distributions.Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }



        public static MathNet.Numerics.Distributions.Normal Multiply(this MathNet.Numerics.Distributions.Normal norm, Tuple<double, double> tup)
        {
            // inverser of variance
            var v_1 = (1 / norm.Variance + 1 / tup.Item2);
            // mean over variance
            var m_v = (norm.Mean / norm.Variance) + (tup.Item1 / tup.Item2);

            // mean equals mean over variance over inverse of variance 
            return new MathNet.Numerics.Distributions.Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }


        public static MathNet.Numerics.Distributions.Normal Multiply(IEnumerable<Tuple<double, double>> tups)
        {
            // inverser of variance
            var x = tups.GetEnumerator();
            double v_1=0, m_v=0;
            while (x.MoveNext())
            {
                v_1 += 1 / x.Current.Item2;
            // mean over variance
                m_v += x.Current.Item1 / x.Current.Item2;
            }
            // mean equals mean over variance over inverse of variance 
            return new MathNet.Numerics.Distributions.Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }


        public static MathNet.Numerics.Distributions.Normal Divide(this MathNet.Numerics.Distributions.Normal norm1, MathNet.Numerics.Distributions.Normal norm2)
        {
            var v_1 = (1 / norm1.Variance + 1 / norm2.Variance);
            var m_v = (norm1.Mean / norm1.Variance) - (norm2.Mean / norm2.Variance);

            return new MathNet.Numerics.Distributions.Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }



        public static MathNet.Numerics.Distributions.Normal Add(this MathNet.Numerics.Distributions.Normal norm1, MathNet.Numerics.Distributions.Normal norm2)
        {
            var v = (norm1.Variance + norm2.Variance);
            var m = (norm1.Mean) + (norm2.Mean);

            return new MathNet.Numerics.Distributions.Normal(m, Math.Sqrt(v));

        }


        public static MathNet.Numerics.Distributions.Normal Minus(this MathNet.Numerics.Distributions.Normal norm1, MathNet.Numerics.Distributions.Normal norm2)
        {
            var v = (norm1.Variance + norm2.Variance);
            var m = (norm1.Mean) - (norm2.Mean);

            return new MathNet.Numerics.Distributions.Normal(m, Math.Sqrt(v));

        }


        public static MathNet.Numerics.Distributions.Normal Divide(this MathNet.Numerics.Distributions.Normal norm1, double factor)
        {
            var s = (norm1.StdDev / (factor));
            var m = (norm1.Mean / factor);

            return new MathNet.Numerics.Distributions.Normal(m, s);

        }


        public static MathNet.Numerics.Distributions.Normal Multiply(this MathNet.Numerics.Distributions.Normal norm1, double factor)
        {
            var s = (norm1.StdDev * (factor));
            var m = (norm1.Mean * factor);

            return new MathNet.Numerics.Distributions.Normal(m, s);

        }


    }


}

