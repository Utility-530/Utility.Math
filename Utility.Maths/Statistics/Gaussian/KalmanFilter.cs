using MoreLinq;
using Savage.Range;
using System;
using System.Collections.Generic;
using System.Linq;
using Deviation = System.Double;

using Number = System.Double;
using X = System.Double;

using Y = MathNet.Numerics.Distributions.Normal;

namespace UtilityMath.Statistics.Gaussian
{
    public static class KalmanFilter
    {
        public static IEnumerable<NormalPoint> Filter(IEnumerable<NormalPoint> measuredValues, Kernel kernel, NormalPoint latest)
        {
            foreach (var (x, y) in measuredValues)
            {
                yield return latest = kernel.Update(x, latest.X, latest.Y);
                latest = new NormalPoint(x, latest.Y.Multiply(y));
            }
        }

        public static IEnumerable<Number> Filter(IEnumerable<Number> measuredValues, Deviation sourceSigma, Deviation measuredSigma, NormalPoint latest = default)
        {
            var values = measuredValues.Select((a, i) => new NormalPoint(i, new Y(a, measuredSigma)));
            var kernel = new BasicKernel { Deviation = sourceSigma };
            return Filter(values, kernel, latest.Y == default ? values.First() : latest)
                .Select(a => a.Y.Mean);
        }

        public static IEnumerable<(Deviation sigma, IEnumerable<Number> filteredValues)> FilterInRange(IEnumerable<Number> measuredValues, Range<Deviation> sourceSigma, Deviation measuredSigma, int count, NormalPoint latest = default)
        {
            var diff = (sourceSigma.Ceiling - sourceSigma.Floor) / (1d * count);

            for (int index = 0; index < count; index++)
            {
                var values = measuredValues.Select((a, i) => new NormalPoint(i, new Y(a, measuredSigma)));
                var sigma = index * diff;
                var kernel = new BasicKernel { Deviation = index * diff };
                latest = latest.Y == default ? values.First() : latest;
                yield return (sigma, Filter(values, kernel, latest).Select(a => a.Y.Mean));
            }
        }

        public static IEnumerable<(int index, Number min, Number max, Number mean)> Weighted<T>(IEnumerable<T> weightValues, Func<ICollection<Number>, Number> weightFunc, Func<T, ICollection<Number>> valuesFunc)
        {
            return weightValues
                .SelectMany(a =>
                {
                    var values = valuesFunc(a);
                    var weight = weightFunc(values);
                    return valuesFunc(a).Select((value, i) => (value, i, weight));
                })
                .GroupBy(a => a.i)
                .Select(a => (a.Key, a.Min(v => v.value), a.Max(v => v.value), a.WeightedAverage(aa => aa.value, aa => aa.weight))); ;
        }
    }

    public abstract class FunctionKernel : Kernel
    {
        public Deviation Deviation { get; set; } = 1;

        public abstract Number Execute(Number x);

        public override NormalPoint Update(X x1, X x0, Y y0)
        {
            return new NormalPoint(x1, new Y(Execute(x1) - Execute(x0), (x1 - x0) * Deviation).Add(y0));
        }
    }

    public class BasicKernel : FunctionKernel
    {
        public override Number Execute(Number x) => 0;
    }

    public class XSquaredKernal : FunctionKernel
    {
        public override Number Execute(Number x) => Math.Pow(x, 2);
    }

    public abstract class Kernel
    {
        public abstract NormalPoint Update(X x1, X x0, Y y0);
    }

    /// <summary>
    /// Represents the results of running a test with the Kalman filter
    /// </summary>
    public class Results
    {
        public Number[] ActualValues;
        public Number[] MeasuredValues;
        public Number[] FilteredValues;
        public double Error;
    }

    public struct NormalPoint
    {
        public NormalPoint(X x, Y y)
        {
            X = x;
            Y = y;
        }

        public X X { get; }
        public Y Y { get; }

        internal void Deconstruct(out X x, out Y y)
        {
            x = X; y = Y;
        }
    }
}