using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using ReactiveUI;

namespace NormalMath.Wpf.Demo
{
    public class MainViewModel : ReactiveObject
    {

        public MainViewModel()
        {
            SelectionViewModel
                .WhenAnyValue(a => a.SelectedValue)
                .Where(a => a != null)
                .Select(method => (a: method, method.GetParameters().Count()))
                .CombineLatest(I1.WhenAnyValue(_ => _.Output), I2.WhenAnyValue(a => a.Output), (a, b, c) => NewMethod(b, c, v => a.a.Invoke(null, v), a.Item2))
                .SelectMany(a => a.ToObservable())
                .ObserveOnDispatcher()
                .Subscribe(normal =>
                {
                    O.Mean = normal.Mean;
                    O.StandardDeviation = normal.StdDev;
                });

            static Task<Normal> NewMethod(Normal normal, Normal normal2, Func<object[], object> method, int parametersCount)
            {
                return Task.Run(() => (Normal)method.Invoke(parametersCount == 2 ? new[] { normal, normal2 } : new object[] { normal, normal2, 1000 }));
            }
        }

        public MethodSelectionViewModel SelectionViewModel { get; } = new MethodSelectionViewModel();

        public ViewModel.NormalViewModel I1 { get; } = new ViewModel.NormalViewModel { Mean = 2, StandardDeviation = 3 };

        public ViewModel.NormalViewModel I2 { get; } = new ViewModel.NormalViewModel { Mean = 2, StandardDeviation = 3 };

        public ViewModel.NormalViewModel O { get; } = new ViewModel.NormalViewModel();

    }


    public class MethodSelectionViewModel : ReactiveObject
    {
        private MethodInfo selectedValue;

        public MethodInfo SelectedValue { get => selectedValue; set => this.RaiseAndSetIfChanged(ref selectedValue, value); }

        public Dictionary<string, MethodInfo> MethodsDictionary =>
            typeof(UtilityMath.Statistics.GaussianCalculator).GetMethodsBySignature(typeof(Normal), typeof(Normal), typeof(Normal))
            .Concat(typeof(UtilityMath.Statistics.GaussianCalculator).GetMethodsBySignature(typeof(Normal), typeof(Normal), typeof(Normal), typeof(int)))
            .ToDictionary(a => a.Name, a => a);
    }
}