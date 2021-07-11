using ReactiveUI;
using System.Reactive.Linq;

namespace NormalMath.Wpf.Demo.ViewModel
{
    public class NormalViewModel : ReactiveObject
    {
        private double mean, standardDeviation;
        private readonly ObservableAsPropertyHelper<MathNet.Numerics.Distributions.Normal> output;

        public NormalViewModel()
        {
            output = this
                .WhenAnyValue(_ => _.Mean)
                .CombineLatest(this.WhenAnyValue(_ => _.StandardDeviation), (a, b) => new MathNet.Numerics.Distributions.Normal(a, b))
                .ToProperty(this, _ => _.Output);
        }

        public double Mean
        {
            get => mean;
            set => this.RaiseAndSetIfChanged(ref mean, value);
        }

        public double StandardDeviation
        {
            get => standardDeviation;
            set => this.RaiseAndSetIfChanged(ref standardDeviation, value);
        }

        public MathNet.Numerics.Distributions.Normal Output => output.Value;
    }
}