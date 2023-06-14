using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper;
using UtilityHelper.NonGeneric;

namespace UtilityMath.WpfApp
{
    public partial class HistogramControl : Control
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(IEnumerable), typeof(HistogramControl), new PropertyMetadata(null, DataChanged));

        public static readonly DependencyProperty BinSizeProperty = DependencyProperty.Register("BinSize", typeof(double), typeof(HistogramControl), new PropertyMetadata(0.5, BinSizeChanged));

        public static readonly DependencyProperty BinCountProperty = DependencyProperty.Register("BinCount", typeof(double), typeof(HistogramControl), new PropertyMetadata(22d, BinCountChanged));

        public static readonly DependencyProperty ObservationProperty = DependencyProperty.Register("Observation", typeof(string), typeof(HistogramControl), new PropertyMetadata("", OnPropertyChangedObservation));

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(string), typeof(HistogramControl), new PropertyMetadata("", OnPropertyChangedTarget));

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(IEnumerable<Tuple<double, double>>), typeof(HistogramControl), new PropertyMetadata(null));

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
            (d as HistogramControl).PointsObservable.OnNext(1);
        }

        private static void BinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).PointsObservable.OnNext(2);
        }

        private static void DataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as HistogramControl).PointsObservable.OnNext(3);
        }

        protected System.Reactive.Subjects.ISubject<int> PointsObservable { get; set; } = new System.Reactive.Subjects.Subject<int>();
        protected System.Reactive.Subjects.ISubject<object> ObservationObservable = new System.Reactive.Subjects.Subject<object>();
        protected System.Reactive.Subjects.ISubject<object> TargetObservable = new System.Reactive.Subjects.Subject<object>();

        public HistogramControl() : base()

        {
            TargetObservable.StartWith("Target").Where(_ => _ != null).DistinctUntilChanged().CombineLatest(
             ObservationObservable.StartWith("Observation").Where(_ => _ != null).DistinctUntilChanged(),
             PointsObservable.StartWith(1), (c, d, e) => new { t = (string)c, o = (string)d, p = e })
             .Subscribe(_ =>
             {
                 switch (_.p)
                 {
                     case (1):
                         OnPropertyChanged(HistogramMethod.Count, _.t, _.o); break;
                     case (2):
                         OnPropertyChanged(HistogramMethod.Size, _.t, _.o); break;
                     case (3):
                         OnPropertyChanged(hmethod, _.t, _.o); break;
                     default:
                         throw new ArgumentOutOfRangeException();
                 }
             });
        }

        private async void OnPropertyChanged(HistogramMethod method, string target, string observation)
        {
            hmethod = method;
            var data = Data;

            var io = await Task.Run(() => GetInputOutput(data, observation, target));
            if (io != null)
            {
                await Update(io.Item1.Zip(io.Item2, (a, b) => Tuple.Create(a, b)).ToArray(), BinCount, BinSize, method)
                    .ContinueWith(async (xx) =>
                await this.Dispatcher.InvokeAsync(async () =>
                {
                    var points = await xx;
                    if (points != null)
                    {
                        this.SetValue(PointsProperty, points);
                    }
                }, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken)));
                //subject.OnNext(data.ToList());
            }
        }

        /*  static*/
        private HistogramMethod hmethod = HistogramMethod.Size;

        private static void OnShowEstimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static async Task<dynamic> Update(IEnumerable<Tuple<double, double>> data, double bincount, double binsize, HistogramMethod method)
        {
            if (data != null)
                if (data.Count() > 0)
                {
                    return await Task.Run(() =>
                    {
                        var his = GetHistogram(data, method, Convert.ToInt16(bincount), (double)binsize);
                        return his.SelectMany(_ => new[] { Tuple.Create(_.Key.Item1, _.Value), Tuple.Create(_.Key.Item2, _.Value) }).ToList();
                    });
                }

            return null;
        }

        public static Tuple<double[], double[]> GetInputOutput(IEnumerable data, string observation, string target)
        {
            if (data == null || data.Count() == 0)
                return default(Tuple<double[], double[]>);

            var contains = data.First().GetType().GetProperties().Select(_ => _.Name).Contains(observation);
            if (contains)
            {
                var observations = UtilityHelper.PropertyHelper.GetPropValues<double>(data, observation).ToArray();
                var targets = UtilityHelper.PropertyHelper.GetPropValues<double>(data, target).ToArray();

                return Tuple.Create(observations.ToArray(), targets.ToArray());
            }
            return default(Tuple<double[], double[]>);
        }

        private static Dictionary<Tuple<double, double>, double> GetHistogram(IEnumerable<Tuple<double, double>> data, HistogramMethod method, int bincount, double binsize)
        {
            switch (method)
            {
                case (HistogramMethod.Count):
                    return UtilityMath.Histogram.ToHistogramByBinCount((data), bincount).ToDictionary(_ => _.Key, _ => _.Value);

                case (HistogramMethod.Size):
                    return UtilityMath.Histogram.ToHistogram((data), binsize);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum HistogramMethod
        {
            Size, Count
        }
    }
}