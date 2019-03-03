//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace StatisticalAnalysis
//{



//    public class GaussianProcessOptimisationService : ISharpLearningPredictionService
//    {

//        public SharpLearning.Containers.CertaintyPrediction[] PredictWithCertainty(IList<double> input, IList<double> targets)
//        {
//            var parameters = new SharpLearning.Optimization.ParameterBounds[] { new SharpLearning.Optimization.ParameterBounds(
//                min: 0, max: 0.1,
//                transform: SharpLearning.Optimization.Transform.Linear,
//                parameterType: SharpLearning.Optimization.ParameterType.Continuous),
//            new SharpLearning.Optimization.ParameterBounds(
//                min: 0, max: 0.1,
//                transform: SharpLearning.Optimization.Transform.Linear,
//                parameterType: SharpLearning.Optimization.ParameterType.Continuous)
//            }; // iterations


//            var optimizer = new SharpLearning.Optimization.RandomSearchOptimizer(parameters, iterations: 10, runParallel: true);
//            //var optimizer = new SharpLearning.Optimization.ParticleSwarmOptimizer(parameters, maxIterations: 100);
//            var best = optimizer.Optimize((d) => new GaussianProcessRegressionService(input, targets).GetOptimizerResult(d));

//            return new GaussianProcessRegressionService(input, targets).GetOutput(best.First().ParameterSet);
//        }


//    }





//    public class GaussianProcessRegressionService : FilterRegressionService
//    {

//        public GaussianProcessRegressionService(IList<double> input, IList<double> targets) : base(input, targets, new GaussianProcess.Wrap.GaussianProcessWrapperBuilder())
//        {


//        }

//    }




//    public class FilterRegressionService
//    {
//        private IList<double> _input;
//        private IList<double> _targets;
//        private Model.IFilterWrapperBuilder _filterbuilderservice;

//        public FilterRegressionService(IList<double> input, IList<double> targets, Filter.Model.IFilterWrapperBuilder filterbuilderservice)
//        {
//            _input = input;
//            _targets = targets;
//            _filterbuilderservice = filterbuilderservice;
//        }

//        public SharpLearning.Containers.CertaintyPrediction[] GetOutput(double[] d) =>

//         _filterbuilderservice.Build(d[0], d[1])
//            .BatchRun(_input.Zip(_targets, (a, b) => new KeyValuePair<DateTime, double>(new DateTime() + TimeSpan.FromSeconds(a), b)))
//            .Select(_ => new SharpLearning.Containers.CertaintyPrediction(_.Value[0].Item1, _.Value[0].Item2))
//            .ToArray();



//        public double GetError(double[] d) =>
//            new SharpLearning.Metrics.Regression.MeanSquaredErrorRegressionMetric()
//                .Error(_input.ToArray(), GetOutput(d).Select(_ => _.Prediction).ToArray());



//        public SharpLearning.Optimization.OptimizerResult GetOptimizerResult(double[] d) => new SharpLearning.Optimization.OptimizerResult(d, GetError(d));


//    }
//}
