using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using In_Extremis.Editor.Euler;
using In_Extremis.Editor.Gauss;

namespace In_Extremis.Editor
{
    public class ComplexPlane : Canvas
    {
        public double Radius {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public double RadiusSquared {
            get { return Radius * Radius; }
        }
        public static readonly DependencyProperty RadiusProperty;

        static ComplexPlane()
        {
            var radiusMetadata = new FrameworkPropertyMetadata(OnRadiusChanged);
            RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(ComplexPlane), radiusMetadata);
        }
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ComplexPlane)d).Draw();
        }

        public ComplexPlane()
            : base()
        {
            Loaded += ComplexPlane_Loaded;
        }

        public ObservableCollection<PrimeFactors> Factors { get; private set; }
        private Dictionary<int, Point[]> Lattice;

        private double scale = 20;
        private SolidColorBrush background;
        private Pen pen;
        private Pen point_pen;

        private bool loaded = false;
        private void ComplexPlane_Loaded(object sender, RoutedEventArgs e)
        {
            Factors = new ObservableCollection<PrimeFactors>(Gaussian.Factors.Select(x => x.Value));
            Lattice = Gaussian.Lattice(scale);
            background = Brushes.Transparent;
            //var gradient = new LinearGradientBrush(Colors.DarkBlue, Colors.DarkMagenta, 45);
            var gradient = new LinearGradientBrush(new GradientStopCollection(new[] { new GradientStop(Colors.DarkMagenta, 0.0), new GradientStop(Colors.DarkBlue, .25), new GradientStop(Colors.DarkMagenta, 0.75) }));
            var gradient2 = new LinearGradientBrush(new GradientStopCollection(new[] { new GradientStop(Colors.DarkOrchid, 0.0), new GradientStop(Colors.MediumPurple, 0.45) }));
            pen = new Pen(gradient, 1);
            point_pen = new Pen(gradient2, scale);

            Draw();

            DoubleAnimation radiusAnimation = new DoubleAnimation();
            radiusAnimation.From = 0;
            radiusAnimation.To = Radius;
            radiusAnimation.AutoReverse = false;
            radiusAnimation.Duration = TimeSpan.FromMinutes(.25);
            this.BeginAnimation(ComplexPlane.RadiusProperty, radiusAnimation);
            loaded = true;
        }

        DrawingVisual circle_visual;
        double last_remainder = 0;
        int last_i = 0;
        public void Draw()
        {
            if (loaded)
            {
                if (circle_visual != null) {
                    DeleteVisual(circle_visual);
                }

                DrawingVisual visual;

                var i_radius = (int)Radius;
                var remainder = Radius - i_radius;
                var difference_from_one = 1 - remainder;
                var point_radius = 1;

                var circle_radius = Radius * scale;

                visual = new DrawingVisual();
                circle_visual = new DrawingVisual();

                using (DrawingContext dc = circle_visual.RenderOpen())
                {
                    dc.DrawEllipse(background, pen, new Point(0, 0), circle_radius, circle_radius);
                }
                this.AddVisual(circle_visual);

                if (last_remainder == 0 || last_remainder > remainder)
                {
                    using (DrawingContext dc = visual.RenderOpen())
                    {
                        var start = last_i * last_i;
                        var end = i_radius * i_radius;
                        for (int i = start; i < end; i++)
                        {
                            var l = Lattice[i];
                            foreach (var p in l)
                            {
                                dc.DrawEllipse(background, point_pen, p, point_radius, point_radius);
                            }
                        }
                    }
                    this.AddVisual(visual);
                    last_i = i_radius;
                }
                last_remainder = remainder;
            }
        }


        #region Control of this control's visual children
        private List<Visual> visuals = new List<Visual>();
        protected override int VisualChildrenCount {
            get { return visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }
        public void ClearVisual()
        {
            while (this.visuals.Count > 0)
            {
                this.DeleteVisual(this.visuals.First());
            }
        }
        public void DeleteVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }
        #endregion
    }
}
