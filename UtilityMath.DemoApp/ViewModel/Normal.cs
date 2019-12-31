using ReactiveUI;
using System.Reactive.Linq;

namespace UtilityMath.WpfApp.ViewModel
{
    public class Normal : ReactiveUI.ReactiveObject
    {
        private double mean;
        private double sd;
        private ObservableAsPropertyHelper<MathNet.Numerics.Distributions.Normal> output;

        public double Mean
        {
            get => mean;
            set => this.RaiseAndSetIfChanged(ref mean, value);
        }

        public double StandardDeviation
        {
            get => sd;
            set => this.RaiseAndSetIfChanged(ref sd, value);
        }

        public MathNet.Numerics.Distributions.Normal Output => output.Value;

        public Normal()
        {
            output = this.WhenAnyValue(_ => _.Mean)
                .CombineLatest(this.WhenAnyValue(_ => _.StandardDeviation), (a, b) => new MathNet.Numerics.Distributions.Normal(a, b))
                .ToProperty(this, _ => _.Output);
        }
    }
}