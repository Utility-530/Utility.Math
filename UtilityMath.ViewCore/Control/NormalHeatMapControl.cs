using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace UtilityMath.View
{

    public class NormalHeatMapControl : Control
    {
        private ISubject<Rectangle> rect1Subject = new Subject<Rectangle>();
        private ISubject<double> StandardDeviationSubject = new Subject<double>();
        private ISubject<double> MinXSubject = new Subject<double>();
        private ISubject<double> MaxXSubject = new Subject<double>();
        private ISubject<double> MeanSubject = new Subject<double>();
        private ISubject<double> MaxYSubject = new Subject<double>();

        //public static readonly DependencyProperty RatioProperty = DependencyProperty.Register("Ratio", typeof(int), typeof(NormalHeatMapControl), new PropertyMetadata(50, Changed));
        public static readonly DependencyProperty StandardDeviationProperty = DependencyProperty.Register("StandardDeviation", typeof(double), typeof(NormalHeatMapControl), new PropertyMetadata(1d, StandardDeviationChanged));
        public static readonly DependencyProperty MeanProperty = DependencyProperty.Register("Mean", typeof(double), typeof(NormalHeatMapControl), new PropertyMetadata(0d, MeanChanged));
        public static readonly DependencyProperty MinXProperty = DependencyProperty.Register("MinX", typeof(double), typeof(NormalHeatMapControl), new PropertyMetadata(-10d, MinXChanged));
        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register("MaxX", typeof(double), typeof(NormalHeatMapControl), new PropertyMetadata(10d, MaxXChanged));
        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register("MaxY", typeof(double), typeof(NormalHeatMapControl), new PropertyMetadata(5d, MaxYChanged));

        private static void StandardDeviationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalHeatMapControl).StandardDeviationSubject.OnNext((double)e.NewValue);
        }

        private static void MinXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalHeatMapControl).MinXSubject.OnNext((double)e.NewValue);
        }

        private static void MaxXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalHeatMapControl).MaxXSubject.OnNext((double)e.NewValue);
        }

        private static void MeanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalHeatMapControl).MeanSubject.OnNext((double)e.NewValue);
        }

        private static void MaxYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NormalHeatMapControl).MaxYSubject.OnNext((double)e.NewValue);
        }

        public double StandardDeviation
        {
            get { return (double)GetValue(StandardDeviationProperty); }
            set { SetValue(StandardDeviationProperty, value); }
        }

        public double Mean
        {
            get { return (double)GetValue(MeanProperty); }
            set { SetValue(MeanProperty, value); }
        }

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }


        static NormalHeatMapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalHeatMapControl), new FrameworkPropertyMetadata(typeof(NormalHeatMapControl)));
        }

        public override void OnApplyTemplate()
        {
            rect1Subject.OnNext(this.GetTemplateChild("Rectangle1") as Rectangle);

            base.OnApplyTemplate();
        }

        public NormalHeatMapControl()
        {
            var changes = MeanSubject
                .CombineLatest(StandardDeviationSubject, MinXSubject, MaxXSubject, MaxYSubject, (a, b, c, d, e) => Calculate(a, b, c, d, e));


            var minMax = MeanSubject
                .CombineLatest(StandardDeviationSubject, (a, b) => CalculateMinMax(a, b));

            var esee = MeanSubject.CombineLatest(minMax, (a, b) => Calculate2(a, b.min, b.max));

            var esdse = MeanSubject
    .CombineLatest(MinXSubject, MaxXSubject, (a, b, c) => Calculate2(a, b, c));


            var changes2 = MeanSubject
         .CombineLatest(StandardDeviationSubject, minMax, (a, b, c) =>
         {
             return Calculate(a, b, c.min, c.max, 1 );
         });



            rect1Subject
                .CombineLatest(changes.Merge(changes2), esdse.Merge(esee), (a, b, c) => (a, b, c))
                .Subscribe(c =>
                {
                    var (lower, middle, upper) = c.b;
                    GradientStopAnimationExample(c.a, c.c, lower, middle, upper);
                });
        }


        static (double lower, double middle, double upper) Calculate(double x, double sd, double minX, double maxX, double maxY)
        {
            //var normal = new Normal(y, sd); 
            var middleValue = Normal.PDF(x, sd, x) / maxY;
            var lowerValue = Normal.PDF(x, sd, minX) / maxY;
            var upperValue = Normal.PDF(x, sd, maxX) / maxY;

            return (lowerValue, middleValue, upperValue);
        }

        static double Calculate2(double x, double minX, double maxX)
        {
            //var normal = new Normal(y, sd); 
            var middleRatio = (x - minX) / (maxX - minX);

            return middleRatio;
        }

        static (double min, double max) CalculateMinMax(double x, double sd)
        {
            var minX = x > 0 ? -(x + 2 * sd) : x - 2 * sd;
            var maxX = -minX;


            return (minX, maxX);
        }

        public void GradientStopAnimationExample(Rectangle rectangle, double newMean, double gradient1, double gradient2, double gradient3)
        {
            //NameScope.SetNameScope(rectangle, new NameScope());

            var r = (rectangle.Fill as LinearGradientBrush);
            var z = r.GradientStops[1];
            var zmin = r.GradientStops[0];
            var zmax = r.GradientStops[2];

            DoubleAnimation offsetAnimation = new DoubleAnimation
            {
                To = newMean,
                Duration = TimeSpan.FromSeconds(1.5)
            };

            ColorAnimation colorAnimation1 = new ColorAnimation
            {
                To = Color.FromArgb(ToColor(gradient1), 0, 0, 0),
                Duration = TimeSpan.FromSeconds(1.5)
            };

            ColorAnimation colorAnimation2 = new ColorAnimation
            {
                To = Color.FromArgb(ToColor(gradient2), 0, 0, 0),
                Duration = TimeSpan.FromSeconds(1.5)
            };

            ColorAnimation colorAnimation3 = new ColorAnimation
            {
                To = Color.FromArgb(ToColor(gradient3), 0, 0, 0),
                Duration = TimeSpan.FromSeconds(1.5)
            };


            z.BeginAnimation(GradientStop.OffsetProperty, offsetAnimation);
            zmin.BeginAnimation(GradientStop.ColorProperty, colorAnimation1);
            z.BeginAnimation(GradientStop.ColorProperty, colorAnimation2);
            zmax.BeginAnimation(GradientStop.ColorProperty, colorAnimation3);

            static byte ToColor(double ratio)
            {
                return (byte)(ratio * 256);
            }
        }
    }
}
