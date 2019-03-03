using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath.Statistics.Correlation
{
    //https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/Correlation.cs

    public class RollingPearson
    {
        int n = 0;
        double r = 0.0;

        double meanA = 0;
        double meanB = 0;
        double varA = 0;
        double varB = 0;

        List<double> _dataA;
        List<double> _dataB;
        double correlation;

        public double Correlation { get { if (count == 0 || count == 1) return 0; else return correlation; } }

        int count;


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

            correlation = r/ Math.Sqrt(varA * varB);
            return Correlation;
        }




    }



}
