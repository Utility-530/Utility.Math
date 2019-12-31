using System;
using System.Collections.Generic;

namespace UtilityMath.Statistics.Correlation
{
    //https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/Correlation.cs

    public class RollingPearson
    {
        private int n = 0;
        private double r = 0.0;

        private double meanA = 0;
        private double meanB = 0;
        private double varA = 0;
        private double varB = 0;

        private List<double> _dataA;
        private List<double> _dataB;
        private double correlation;

        public double Correlation { get { if (count == 0 || count == 1) return 0; else return correlation; } }

        private int count;

        public RollingPearson(List<double> dataA, List<double> dataB)
        {
            _dataA = dataA;
            _dataB = dataB;
        }

        public RollingPearson()
        {
            _dataA = new List<double>();
            _dataB = new List<double>();
        }

        public double Update(double a, double b)
        {
            _dataA.Add(a);
            _dataB.Add(b);
            count++;

            //correlation= BatchCorrelation.Pearson(dataA, dataB, ref n, ref r, ref meanA, ref meanB, ref varA, ref varB);
            double currentA = a;
            double currentB = b;

            double deltaA = currentA - meanA;
            double scaleDeltaA = deltaA / ++n;

            double deltaB = currentB - meanB;
            double scaleDeltaB = deltaB / n;

            meanA += scaleDeltaA;
            meanB += scaleDeltaB;

            varA += scaleDeltaA * deltaA * (n - 1);
            varB += scaleDeltaB * deltaB * (n - 1);
            r += (deltaA * deltaB * (n - 1)) / n;

            correlation = r / Math.Sqrt(varA * varB);
            return Correlation;
        }
    }
}