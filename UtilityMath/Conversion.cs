using MathNet.Numerics.Distributions;
using SharpLearning.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityMath
{
    public static class Conversion
    {

        public static CertaintyPrediction ToCertaintyPrediction(this Normal normal)
        {
            return new CertaintyPrediction(normal.Mean, normal.StdDev);

        }

        public static Normal ToNormal(this CertaintyPrediction cp)
        {
            return new Normal(cp.Prediction, cp.Variance);

        }
    }
}
