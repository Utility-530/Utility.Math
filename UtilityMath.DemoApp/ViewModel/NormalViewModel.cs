using MathNet.Numerics.Distributions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityMath.WpfApp
{
    public class NormalViewModel : ReactiveObject
    {
        System.Reflection.MethodInfo selectedValue;

        public ViewModel.Normal I1 { get; } = new ViewModel.Normal { Mean = 2, StandardDeviation = 3 };

        public ViewModel.Normal I2 { get; } = new ViewModel.Normal { Mean = 2, StandardDeviation = 3 };

        public ViewModel.Normal O1 { get; } = new ViewModel.Normal();

        public System.Reflection.MethodInfo SelectedValue { get => selectedValue; set => this.RaiseAndSetIfChanged(ref selectedValue, value); }

        public Dictionary<string, System.Reflection.MethodInfo> Dictionary =>
            typeof(UtilityMath.NormalExtension).GetMethodsBySignature(typeof(Normal), typeof(Normal), typeof(Normal))
            .Concat(typeof(UtilityMath.NormalExtension).GetMethodsBySignature(typeof(Normal), typeof(Normal), typeof(Normal), typeof(int)))
            .ToDictionary(_ => _.Name, _ => _);


        public NormalViewModel()
        {
            I1.WhenAnyValue(_ => _.Output).CombineLatest(I2.WhenAnyValue(_ => _.Output), this.WhenAnyValue(_ => _.SelectedValue).Where(_ => _ != null), (a, b, c) =>
            Task.Run(() => (c.GetParameters().Count() == 2) ? c.Invoke(null, new[] { a, b }) : c.Invoke(null, new object[] { a, b, 1000 })))
                .Subscribe(async _ =>
                {
                    var normal = (Normal)await _;
                    O1.Mean = normal.Mean;
                    O1.StandardDeviation = normal.StdDev;
                });
        }
    }

}
