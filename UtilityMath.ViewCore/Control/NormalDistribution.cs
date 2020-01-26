using Accord.Math;
using OxyPlot;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityMath.ViewCore
{
    public class NormalDistribution : Control
    {
        public PlotModel PlotModel
        {
            get { return (PlotModel)GetValue(PlotModelProperty); }
            set { SetValue(PlotModelProperty, value); }
        }

        public static readonly DependencyProperty PlotModelProperty = DependencyProperty.Register("PlotModel", typeof(PlotModel), typeof(NormalDistribution), new PropertyMetadata(null));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NormalDistribution), new PropertyMetadata(false));

        public double Mean
        {
            get { return (double)GetValue(MeanProperty); }
            set { SetValue(MeanProperty, value); }
        }

        public static readonly DependencyProperty MeanProperty = DependencyProperty.Register("Mean", typeof(double), typeof(NormalDistribution), new PropertyMetadata(0d, Changed));

        public double StandardDeviation
        {
            get { return (double)GetValue(StandardDeviationProperty); }
            set { SetValue(StandardDeviationProperty, value); }
        }

        public static readonly DependencyProperty StandardDeviationProperty = DependencyProperty.Register("StandardDeviation", typeof(double), typeof(NormalDistribution), new PropertyMetadata(1d, Changed));
        private PlotModelService pmservice;

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as NormalDistribution);
            if (control.StandardDeviation > 0)
                control.Recalculate(control.Mean, control.StandardDeviation);
        }

        static NormalDistribution()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalDistribution), new FrameworkPropertyMetadata(typeof(NormalDistribution)));
        }

        public NormalDistribution()
        {
            pmservice = new PlotModelService();
            Recalculate(Mean, StandardDeviation);
        }

        private void Recalculate(double mean, double stdDev)
        {
            Accord.DoubleRange range = new Accord.DoubleRange(-100, 100);
            Task.Run(() =>
            {
                Accord.Statistics.Distributions.Univariate.NormalDistribution normalDistribution = new Accord.Statistics.Distributions.Univariate.NormalDistribution(mean, stdDev);
                double[] x = Accord.Math.Vector.Range(-100, 100, 0.1);
                double[] y = x.Apply(normalDistribution.ProbabilityDensityFunction);
                return pmservice.Create(range, "", x, y, false);
            }).ContinueWith(a => this.Dispatcher.InvokeAsync(async () => PlotModel = await a, System.Windows.Threading.DispatcherPriority.Background));
        }
    }
}