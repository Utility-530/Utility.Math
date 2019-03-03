using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Error;
using MoreLinq;

namespace UtilityMath.Statistics.Error
{
    public class Calculator
    {
        public static IEnumerable<KeyValuePair<DateTime, double>> GetError(IEnumerable<KeyValuePair<DateTime, double>> a, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> b)
        {
            return Error.Calculator.GetError(a, b, c => c.Value, d => d.Value[0].Item1, Error.Residual.RMSE).Zip(a, (c, d) => new KeyValuePair<DateTime, double>(d.Key, c));

        }
        public static double GetErrorSum(IEnumerable<KeyValuePair<DateTime, double>> a, IEnumerable<KeyValuePair<DateTime, Tuple<double, double>[]>> b)
        {
            return Error.Calculator.GetErrorSum(a, b, c => c.Value, d => d.Value[0].Item1, Error.Residual.RMSE);

        }

        public static double GetErrorSum(IEnumerable<double> est, IEnumerable<double> meas, Residual lf)
        {

            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(f.a, f.b));


        }

        public static IEnumerable<double> GetError(IEnumerable<double> est, IEnumerable<double> meas, Residual lf)
        {

            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Scan(0d, (e, f) => e + lossFunction(f.a, f.b));


        }

        public static double GetErrorSum<T, R>(IEnumerable<T> est, IEnumerable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        {
            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        }

        public static IEnumerable<double> GetError<T, R>(IEnumerable<T> est, IEnumerable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        {
            var lossFunction = Helper.ChooseLossFunction(lf);

            return est.Zip(meas, (a, b) => new { a, b }).Scan(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        }

        class Helper
        {

            public static Func<double, double, double> ChooseLossFunction(Residual lf)
            {

                switch (lf)
                {

                    case (Residual.MAE):
                        return LossFunctions.MeanAbsoluteError;
                    case (Residual.MSE):
                        return LossFunctions.MeanSquareError;
                    case (Residual.RMSE):
                        return LossFunctions.RootMeanSquareError;
                    case (Residual.Hinge):
                        return LossFunctions.HingeError;
                    case (Residual.CrossEntropy):
                        return LossFunctions.CrossEntropyError;
                    default:
                        return LossFunctions.MeanSquareError;

                }

            }

        }
        //public static double GetErrorSum<T, R>(IObservable<T> est, IObservable<R> meas, Func<T, double> estf, Func<R, double> measf, Residual lf)
        //{
        //    var lossFunction = Helper.ChooseLossFunction(lf);

        //    return est.Zip(meas, (a, b) => new { a, b }).Aggregate(0d, (e, f) => e + lossFunction(estf(f.a), measf(f.b)));

        //}
    }
}
