using Accord;
using Accord.Math;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityMath.View
{
    public class NormalDistributionControl : Control
    {
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NormalDistributionControl), new PropertyMetadata(false));

        public static readonly DependencyProperty PlotModelProperty = DependencyProperty.Register("PlotModel", typeof(PlotModel), typeof(NormalDistributionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StandardDeviationProperty = DependencyProperty.Register("StandardDeviation", typeof(double), typeof(NormalDistributionControl), new PropertyMetadata(1d, Changed, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            return Math.Max((double)baseValue, 0.001);
        }

        public static readonly DependencyProperty MeanProperty = DependencyProperty.Register("Mean", typeof(double), typeof(NormalDistributionControl), new PropertyMetadata(0d, Changed));

        public PlotModel PlotModel
        {
            get { return (PlotModel)GetValue(PlotModelProperty); }
            set { SetValue(PlotModelProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public double Mean
        {
            get { return (double)GetValue(MeanProperty); }
            set { SetValue(MeanProperty, value); }
        }

        public double StandardDeviation
        {
            get { return (double)GetValue(StandardDeviationProperty); }
            set { SetValue(StandardDeviationProperty, value); }
        }


        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as NormalDistributionControl);
            if (control.StandardDeviation > 0)
                control.Recalculate(control.Mean, control.StandardDeviation);
        }

        static NormalDistributionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalDistributionControl), new FrameworkPropertyMetadata(typeof(NormalDistributionControl)));
        }

        public NormalDistributionControl()
        {
            Recalculate(Mean, StandardDeviation);
        }

        private void Recalculate(double mean, double stdDev)
        {
            Task.Run(() =>
            {
                Accord.Statistics.Distributions.Univariate.NormalDistribution normalDistribution = new Accord.Statistics.Distributions.Univariate.NormalDistribution(mean, stdDev);
                var limit = Math.Abs(normalDistribution.Mean + 2 * normalDistribution.StandardDeviation);
                DoubleRange range = new Accord.DoubleRange(-limit, limit);
                double[] x = Accord.Math.Vector.Range(-limit, limit, 0.1);
                double[] y = x.Apply(normalDistribution.ProbabilityDensityFunction);
                return PlotModelHelper.Create(range, "", x, y, false);
            }).ContinueWith(a => this.Dispatcher.InvokeAsync(async () => PlotModel = await a, System.Windows.Threading.DispatcherPriority.Background));
        }


        public class PlotModelHelper
        {
            public static PlotModel Create(DoubleRange? range, string title, double[] x, double[] y, bool discrete)
            {
                var plotModel = new PlotModel();
                plotModel.Series.Clear();
                plotModel.Axes.Clear();

                double ymin = y.FirstOrDefault(a => !Double.IsNaN(a) && !Double.IsInfinity(a));
                double ymax = ymin;

                for (int i = 0; i < y.Length; i++)
                {
                    if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                        continue;

                    if (y[i] > ymax)
                        ymax = y[i];
                    if (y[i] < ymin)
                        ymin = y[i];
                }

                double maxGrace = ymax * 0.1;
                double minGrace = ymin * 0.1;

                if (!discrete)
                {
                    var xAxis = new OxyPlot.Axes.LinearAxis()
                    {
                        Position = AxisPosition.Bottom,
                        Minimum = range.Value.Min,
                        Maximum = range.Value.Max,
                        Key = "xAxis",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        IntervalLength = 80
                    };

                    var yAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = ymin - minGrace,
                        Maximum = ymax + maxGrace,
                        Key = "yAxis",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = title
                    };

                    plotModel.Axes.Add(xAxis);
                    plotModel.Axes.Add(yAxis);

                    var lineSeries = new LineSeries
                    {
                        YAxisKey = yAxis.Key,
                        XAxisKey = xAxis.Key,
                        StrokeThickness = 2,
                        MarkerSize = 3,
                        MarkerStroke = OxyColor.FromRgb(0, 0, 0),
                        MarkerType = MarkerType.None,

                    };

                    for (int i = 0; i < x.Length; i++)
                    {
                        if (Double.IsNaN(y[i]) || Double.IsInfinity(y[i]))
                            continue;

                        lineSeries.Points.Add(new DataPoint(x[i], y[i]));
                    }

                    plotModel.Series.Add(lineSeries);
                }
                else
                {
                    var xAxis = new OxyPlot.Axes.CategoryAxis()
                    {
                        Position = AxisPosition.Bottom,
                        Key = "xAxis",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                    };

                    var yAxis = new LinearAxis()
                    {
                        Position = AxisPosition.Left,
                        Minimum = ymin - minGrace,
                        Maximum = ymax + maxGrace,
                        Key = "yAxis",
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Title = title
                    };

                    plotModel.Axes.Add(xAxis);
                    plotModel.Axes.Add(yAxis);

                    var boxSeries = new ColumnSeries
                    {
                        YAxisKey = yAxis.Key,
                        XAxisKey = xAxis.Key,
                        StrokeThickness = 2,
                        ColumnWidth = 1,
                    };

                    for (int i = 0; i < x.Length; i++)
                    {
                        xAxis.Labels.Add(x[i].ToString("G2"));
                        var item = new ColumnItem(y[i]);
                        boxSeries.Items.Add(item);
                    }

                    plotModel.Series.Add(boxSeries);
                }

                //var formattable = instance as IFormattable;
                //if (formattable != null)
                //{
                //    plotModel.Title = formattable.ToString("G3", CultureInfo.CurrentUICulture);
                //}
                //else
                //{
                //    plotModel.Title = instance.ToString();
                //}

                plotModel.TitlePadding = 2;
                plotModel.TitleFontSize = 15;
                plotModel.TitleFontWeight = 1;
                plotModel.TitlePadding = 2;

                return plotModel;
            }
        }
    }

    public class NormalDistributionInputControl : Control
    {

        public static readonly DependencyProperty StandardDeviationProperty = DependencyProperty.Register("StandardDeviation", typeof(double), typeof(NormalDistributionInputControl), new PropertyMetadata(1d, Changed));

        public static readonly DependencyProperty MeanProperty = DependencyProperty.Register("Mean", typeof(double), typeof(NormalDistributionInputControl), new PropertyMetadata(0d, Changed));

        static NormalDistributionInputControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalDistributionInputControl), new FrameworkPropertyMetadata(typeof(NormalDistributionInputControl)));
        }


        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public double Mean
        {
            get { return (double)GetValue(MeanProperty); }
            set { SetValue(MeanProperty, value); }
        }

        public double StandardDeviation
        {
            get { return (double)GetValue(StandardDeviationProperty); }
            set { SetValue(StandardDeviationProperty, value); }
        }

    }

    public class NumberInputControl : Control
    {

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(NumberInputControl), new PropertyMetadata(0d));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(NumberInputControl), new PropertyMetadata(0d, Changed, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            return Math.Round((double)baseValue, 1);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(NumberInputControl), new PropertyMetadata(string.Empty));

        static NumberInputControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberInputControl), new FrameworkPropertyMetadata(typeof(NumberInputControl)));
        }
        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
    }
}