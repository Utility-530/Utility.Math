using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SharpLearning.Extension.Interface;


//namespace Statistics.Analysis
//{

//    public class SharpLearningRegressionService:IModelBuilder
//    {
//        protected IList<double> _input;
//        protected IList<double> _targets;
//        protected ILearnerBuilder _learnerbuilderservice;
//        protected static int _fold = 10;

//        public SharpLearningRegressionService(IList<double> input, IList<double> targets, ILearnerBuilder learnerbuilderservice)
//        {
//            _input = input;
//            _targets = targets;
//            _learnerbuilderservice = learnerbuilderservice;
//        }

//        //public SharpLearning.Containers.CertaintyPrediction[] GetOutput(double[] d) =>

//        // _learnerbuilderservice.Build(d)
//        //    .Learn(GetF64(_input), _targets.ToArray())
//        //    .Predict(GetF64(_input))
//        //    .Select(_ => new SharpLearning.Containers.CertaintyPrediction(_, 1))
//        //    .ToArray();

//        public SharpLearning.Common.Interfaces.IPredictor<double> Build(double[] d) =>

//_learnerbuilderservice.Build(d)
//.Learn(GetF64(_input), _targets.ToArray());
////.Predict(GetF64(_input))
////.Select(_ => new SharpLearning.Containers.CertaintyPrediction(_, 1))
////.ToArray();

//        public virtual double GetError(double[] d)
//        {

//            var xvalues = _input;
//            var combined = xvalues.Zip(_targets, (a, b) => Tuple.Create(a, b));
//            //var values2 = values.Select(_ => Tuple.Create((double)_.Key.Ticks, _.Value)).ToList();

//            var folds = combined.KFolds(_fold);

//            List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>> inp = new List<KeyValuePair<IEnumerable<Tuple<double, double>>, double>>();
//            double errorsum = 0;
//            foreach (var fold in folds)
//            {
//                var trainx = fold.Train.Select(_ => _.Item1).ToArray();
//                var trainy = fold.Train.Select(_ => _.Item2).ToArray();
//                var testx = fold.Test.Select(_ => _.Item1).ToArray();
//                var testy = fold.Test.Select(_ => _.Item2).ToArray();

//                var predictions = _learnerbuilderservice.Build(d)
//                     .Learn(GetF64(trainx), trainy)
//                     .Predict(GetF64(testx));

//                errorsum += new SharpLearning.Metrics.Regression.MeanSquaredErrorRegressionMetric().Error(testy, predictions);

//            }

//            return errorsum;


//        }

//        public SharpLearning.Optimization.OptimizerResult GetOptimizerResult(double[] d) => new SharpLearning.Optimization.OptimizerResult(d, GetError(d));

//        public static SharpLearning.Containers.Matrices.F64Matrix GetF64(IEnumerable<double> input) => new SharpLearning.Containers.Matrices.F64Matrix(input.ToArray(), input.Count(), 1);

//    }


//}
