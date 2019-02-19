﻿using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath
{



    public static class NormalExtension
    {

        public static Normal Negative(this Normal sbw)
        {

            return new Normal(-sbw.Mean, sbw.StdDev);
        }



        public static Normal Multiply(this Normal norm1, Normal norm2)
        {

            var v_1 = (1 / norm1.Variance + 1 / norm2.Variance);
            var m_v = (norm1.Mean / norm1.Variance) + (norm2.Mean / norm2.Variance);

            return new Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }



        public static Normal Multiply(this Normal norm, Tuple<double, double> tup)
        {
            // inverser of variance
            var v_1 = (1 / norm.Variance + 1 / tup.Item2);
            // mean over variance
            var m_v = (norm.Mean / norm.Variance) + (tup.Item1 / tup.Item2);

            // mean equals mean over variance over inverse of variance 
            return new Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }


        public static Normal Multiply(IEnumerable<Tuple<double, double>> tups)
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
            return new Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }


        public static Normal Divide(this Normal norm1, Normal norm2)
        {
            var v_1 = (1 / norm1.Variance + 1 / norm2.Variance);
            var m_v = (norm1.Mean / norm1.Variance) - (norm2.Mean / norm2.Variance);

            return new Normal(m_v / v_1, Math.Sqrt(1 / v_1));

        }



        public static Normal Add(this Normal norm1, Normal norm2)
        {
            var v = (norm1.Variance + norm2.Variance);
            var m = (norm1.Mean) + (norm2.Mean);

            return new Normal(m, Math.Sqrt(v));

        }


        public static Normal Minus(this Normal norm1, Normal norm2)
        {
            var v = (norm1.Variance + norm2.Variance);
            var m = (norm1.Mean) - (norm2.Mean);

            return new Normal(m, Math.Sqrt(v));

        }


        public static Normal Divide(this Normal norm1, double factor)
        {
            var s = (norm1.StdDev / (factor));
            var m = (norm1.Mean / factor);

            return new Normal(m, s);

        }


        public static Normal Multiply(this Normal norm1, double factor)
        {
            var s = (norm1.StdDev * (factor));
            var m = (norm1.Mean * factor);

            return new Normal(m, s);

        }



        public static Normal MonteCarloMultiply(Normal norm1, Normal norm2,int size)
        {
            return MonteCarloOperation(norm1, norm2, size, (a, b) => a * b);
        }


        public static Normal MonteCarloDivide(Normal norm1, Normal norm2, int size)
        {
            return MonteCarloOperation(norm1, norm2, size, (a, b) => a / b);
        }

        public static Normal MonteCarloAdd(Normal norm1, Normal norm2, int size)
        {
            return MonteCarloOperation(norm1, norm2, size, (a, b) => a + b);
        }

        public static Normal MonteCarloSubstract(Normal norm1, Normal norm2, int size)
        {
            return MonteCarloOperation(norm1, norm2, size, (a, b) => a - b);
        }

        public static Normal MonteCarloOperation(Normal norm1, Normal norm2, int size,Func<double,double,double > operation )
        {
            double[] samples1 = new double[size];
            norm1.Samples(samples1);
            double[] samples2 = new double[size];
            norm2.Samples(samples2);

            var values = (from i in Enumerable.Range(0, size) select operation(samples1[i], samples2[i])).ToArray();

            double mean = values.Sum() / size;

            double std = Math.Sqrt(values.Select(_ => Math.Pow(_ - mean, 2)).Sum() / size);

            return new Normal(mean, std);

        }

    }


}

