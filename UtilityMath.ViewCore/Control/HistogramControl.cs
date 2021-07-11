using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityHelper;
using UtilityHelper.NonGeneric;
using System.Reactive.Subjects;

namespace UtilityMath.View
{
    public partial class HistogramControl : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IEnumerable), typeof(HistogramControl), new PropertyMetadata(null, DataChanged));
        public static readonly DependencyProperty BinSizeProperty = DependencyProperty.Register("BinSize", typeof(double), typeof(HistogramControl), new PropertyMetadata(0.5, BinSizeChanged));
        public static readonly DependencyProperty BinCountProperty = DependencyProperty.Register("BinCount", typeof(double), typeof(HistogramControl), new PropertyMetadata(22d, BinCountChanged));
        public static readonly DependencyProperty ObservationProperty = DependencyProperty.Register("Observation", typeof(string), typeof(HistogramControl), new PropertyMetadata("Observation", OnPropertyChangedObservation));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(string), typeof(HistogramControl), new PropertyMetadata("Target", OnPropertyChangedTarget));
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(IEnumerable<Coordinate>), typeof(HistogramControl), new PropertyMetadata(null));

        protected ISubject<HistogramMethod> MethodObservable = new Subject<HistogramMethod>();
        protected ISubject<object> ObservationObservable = new Subject<object>();
        protected ISubject<object> TargetObservable = new Subject<object>();
        protected ISubject<IEnumerable> DataObservable = new Subject<IEnumerable>();
        private TextBlock textBlock;

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public string Observation
        {
            get { return (string)GetValue(ObservationProperty); }
            set { SetValue(ObservationProperty, value); }
        }

        public IEnumerable Data
        {
            get { return (IEnumerable)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public double BinSize
        {
            get { return (double)GetValue(BinSizeProperty); }
            set { SetValue(BinSizeProperty, value); }
        }

        public double BinCount
        {
            get { return (double)GetValue(BinCountProperty); }
            set { SetValue(BinCountProperty, value); }
        }


        public IEnumerable<Coordinate> Points
        {
            get { return (IEnumerable<Coordinate>)GetValue(BinCountProperty); }
        }

        static HistogramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HistogramControl), new FrameworkPropertyMetadata(typeof(HistogramControl)));
        }

        private static void OnPropertyChangedObservation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).ObservationObservable.OnNext(e.NewValue);
        }

        private static void OnPropertyChangedTarget(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).TargetObservable.OnNext(e.NewValue);
        }

        private static void BinCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).MethodObservable.OnNext(HistogramMethod.Count);
        }

        private static void BinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).MethodObservable.OnNext(HistogramMethod.Size);
        }

        private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).DataObservable.OnNext((IEnumerable)e.NewValue);
        }


        public HistogramControl() : base()

        {
            TargetObservable.StartWith(Target).Where(_ => _ != null).DistinctUntilChanged()
                .CombineLatest(ObservationObservable.StartWith(Observation).Where(_ => _ != null).DistinctUntilChanged(),
             MethodObservable.StartWith(HistogramMethod.Size), DataObservable,
             (t, o, m, d) => ((string)t, (string)o, m, d))
             .Subscribe(async a =>
             {
                 var (target, observation, method, data) = a;

                 var io = await Task.Run(() =>
                 {
                     Exception exception = null;
                     (double, double)[] values = null;
                     try
                     {
                         var (input, output) = GetInputOutput(data, observation, target);
                         values = input.Zip(output).ToArray();
                     }
                     catch (Exception ex)
                     {
                         exception = ex;
                     }

                     return (values, exception);
                 });

                 if (io.exception != null)
                 {
                     if (textBlock != null)
                         textBlock.Text = io.exception.Message;
                     return;
                 }
                 if (textBlock != null)
                     textBlock.Text = "";
                 if (io.values == default)
                     return;


                 await Update(io.values, BinCount, BinSize, method)
                 .ContinueWith(async points =>
                 await this.Dispatcher.InvokeAsync(async () =>
                 {
                     this.SetValue(PointsProperty, await points);

                 }, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken)));
             });
        }

        public override void OnApplyTemplate()
        {
            textBlock = this.GetTemplateChild("ErrorTextBlock") as TextBlock;
            base.OnApplyTemplate();
        }

        private static void OnShowEstimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static async Task<Coordinate[]> Update((double, double)[] data, double bincount, double binsize, HistogramMethod method)
        {
            if (data != null)
                if (data.Count() > 0)
                {
                    return await Task.Run(() =>
                    {
                        var his = GetHistogram(data, method, Convert.ToInt16(bincount), (double)binsize);
                        return his.SelectMany(_ => new[] { new Coordinate(_.Key.Item1, _.Value), new Coordinate(_.Key.Item2, _.Value) }).ToArray();
                    });
                }

            return null;
        }

        private static (double[] input, double[] output) GetInputOutput(IEnumerable data, string observation, string target)
        {
            if (data == null)
                throw new Exception($"Data is null");
            if (data.Count() == 0)
                return default((double[], double[]));

            if (data.First().GetType().GetProperties().Select(p => p.Name).Contains(observation))
            {
                var observations = PropertyHelper.GetPropertyValues<double>(data, observation).ToArray();
                var targets = PropertyHelper.GetPropertyValues<double>(data, target).ToArray();

                return (observations.ToArray(), targets.ToArray());
            }
            throw new Exception($"Data doesn't contain {observation}");
        }

        private static Dictionary<Tuple<double, double>, double> GetHistogram((double, double)[] data, HistogramMethod method, int bincount, double binsize)
        {
            switch (method)
            {
                case (HistogramMethod.Count):
                    return Histogram.ToHistogramByBinCount(data, bincount).ToDictionary(a => a.Key, a => a.Value);

                case (HistogramMethod.Size):
                    return Histogram.ToHistogram(data, binsize);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected enum HistogramMethod
        {
            Size, Count
        }
    }

    public struct Coordinate
    {
        public Coordinate(double target, double observation) : this()
        {
            X = target;
            Y = observation;
        }
        public double X { get; }

        public double Y { get; }
    }

    public class StringFomatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                string formatterString = parameter.ToString();

                if (!string.IsNullOrEmpty(formatterString))
                {
                    return string.Format(culture, formatterString, value);
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}