using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statistics.Analysis
{
    //public static class StatisticalHelper
    //{
    //    public static double StdDev(IEnumerable<double> values)
    //    {

    //        if (values.Count() > 0)
    //        {
    //            double avg = values.Average();
    //            double sum = values.Sum(d => Math.Pow(d - avg, 2));
    //            return Math.Sqrt((sum) / (values.Count() - 1));
    //        }
    //        else
    //            return 0;
    //    }




    //    public static double WeightedAverage<T>(IEnumerable<T> records, Func<T, double> value, Func<T, double> weight, double control = 0)
    //    {
    //        double weightedValueSum = records.Sum(x => (value(x) - control) * weight(x));
    //        double weightSum = records.Sum(x => weight(x));

    //        if (weightSum != 0)
    //            return weightedValueSum / weightSum;
    //        else
    //            throw new DivideByZeroException("Divide by zero exception calculating weighted average");
    //    }



    //}
}
