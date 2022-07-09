using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using OxyPlot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UtilityMath;
using UtilityMath.Statistics.Gaussian;
using est = FilterMath.WPF.Demo.Estimate<int>;

namespace FilterMath.WPF.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class KalmanAggregateFilterViewModel
    {
        public KalmanAggregateFilterViewModel()
        {
            var values = new NoisyValues(1, 0.3, new Random()).Cos(40).ToArray();
            var filtered = KalmanFilter.FilterInRange(values.Select(a => a.noisyY), new Savage.Range.Range<double>(0.1, 0.9), 0.2, 10);
            var weighted = KalmanFilter.Weighted(filtered,
                a => a.Join(values.Select(a => a.y), a => true, a => true, (a, b) => Math.Abs(a - b)).Sum(),
                a => a.filteredValues.ToArray())
                .Select(a => new est(a.index, a.mean, a.min, a.max))
                .ToArray();

            Values = values.Select(a => new { a.x, a.y, a.noisyY }).ToArray();

            Model = MakePlotModel(values, weighted);

            Series = new[]{
                LiveChartsHelper.ToSmoothLineSeries(values.Select(a => a.y)),
                LiveChartsHelper.ToLineSeries(values.Select(a => a.noisyY)),
                LiveChartsHelper.ToLineSeries(weighted.Select(a=>a.Value))
            };

            static PlotModel MakePlotModel((int x, double y, double noisyY)[] values, est[] weighted)
            {
                var model = new PlotModel { };
                model.Series.Add(new OxyPlot.Series.LineSeries { ItemsSource = values.Select((a, i) => new DataPoint(i, a.y)) });
                model.Series.Add(new OxyPlot.Series.LineSeries { ItemsSource = values.Select((a, i) => new DataPoint(i, a.noisyY)) });
                model.Series.Add(new OxyPlot.Series.AreaSeries
                {
                    ItemsSource = weighted,
                    DataFieldX = nameof(est.Fixed),
                    DataFieldY = nameof(est.LowerLimit),
                    DataFieldX2 = nameof(est.Fixed),
                    DataFieldY2 = nameof(est.UpperLimit),
                    Fill = OxyColor.FromArgb(128, 255, 255, 0),
                    Color = OxyColors.Black
                });

                model.Series.Add(new OxyPlot.Series.LineSeries { ItemsSource = weighted.Select((a, i) => new DataPoint(i, a.Value)) });
                return model;
            }
        }

        public IEnumerable Values { get; }
        public ISeries[] Series { get; }
        public PlotModel Model { get; }

        public string Key => nameof(KalmanAggregateFilterViewModel);
    }

    public partial class KalmanFilterViewModel
    {
        public KalmanFilterViewModel()
        {
            var values = new NoisyValues(1, 0.3, new System.Random()).Cos(20).ToArray();
            var filtered01 = KalmanFilter.Filter(values.Select(a => a.noisyY), 0.1, 0.2);
            var filtered05 = KalmanFilter.Filter(values.Select(a => a.noisyY), 0.5, 0.2);
            var filtered09 = KalmanFilter.Filter(values.Select(a => a.noisyY), 0.9, 0.2);

            Series = new[]{
                LiveChartsHelper.ToSmoothLineSeries(values.Select(a => a.y)),
                LiveChartsHelper.ToLineSeries(values.Select(a => a.noisyY)),
                LiveChartsHelper.ToLineSeries(filtered01),
                LiveChartsHelper.ToLineSeries(filtered05),
                LiveChartsHelper.ToLineSeries(filtered09)
            };
        }

        public ISeries[] Series { get; }

        public string Key => nameof(KalmanFilterViewModel);
    }

    public static class LiveChartsHelper
    {
        public static LineSeries<ObservablePoint> ToLineSeries(this IEnumerable<double> filtered)
        {
            return new LineSeries<ObservablePoint>
            {
                Values = filtered.Select((a, i) => new ObservablePoint(i, a)).ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0
            };
        }

        public static LineSeries<ObservablePoint> ToSmoothLineSeries(this IEnumerable<double> filtered)
        {
            return new LineSeries<ObservablePoint>
            {
                Values = filtered.Select((a, i) => new ObservablePoint(i, a)).ToArray(),
                Fill = null,
                GeometrySize = 0,
            };
        }

        public static ScatterSeries<ObservablePoint> ToScatterSeries(this IEnumerable<double> filtered)
        {
            return new ScatterSeries<ObservablePoint>
            {
                Values = filtered.Select((a, i) => new ObservablePoint(i, a)).ToArray(),
            };
        }
    }

    public struct Estimate<TIndex>
    {
        public Estimate(TIndex @fixed, double value, double deviation = 0)
        {
            Fixed = @fixed;
            Value = value;
            Deviation = deviation;
            UpperLimit = value + Math.Sqrt(deviation);
            LowerLimit = value - Math.Sqrt(deviation);
        }

        public Estimate(TIndex @fixed, double value, double lowerLimit, double upperLimit)
        {
            Fixed = @fixed;
            Value = value;
            Deviation = (lowerLimit + upperLimit) / 2 - value;
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
        }

        public double Value { get; }
        public TIndex Fixed { get; }
        public double Deviation { get; }
        public double UpperLimit { get; }
        public double LowerLimit { get; }

        public struct Distribution
        {
            public Distribution(double value, double deviation = 0)
            {
                Value = value;
                Deviation = deviation;
                UpperLimit = value + Math.Sqrt(deviation);
                LowerLimit = value - Math.Sqrt(deviation);
            }

            public Distribution(double value, double lowerLimit, double upperLimit)
            {
                Value = value;
                Deviation = (lowerLimit + upperLimit) / 2 - value;
                UpperLimit = upperLimit;
                LowerLimit = lowerLimit;
            }

            public double Value { get; }
            public double Deviation { get; }
            public double UpperLimit { get; }
            public double LowerLimit { get; }
        }
    }
}