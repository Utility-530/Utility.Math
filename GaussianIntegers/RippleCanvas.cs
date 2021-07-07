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
using In_Extremis.Editor.Gauss;

namespace In_Extremis.Editor
{
    public class RippleCanvas : Canvas
    {
        #region Dependency Properties used by Animations.
        public double Radius {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public double RadiusSquared {
            get { return Radius * Radius; }
        }

        public static readonly DependencyProperty RadiusProperty;
        static RippleCanvas()
        {
            var radiusMetadata = new FrameworkPropertyMetadata(OnRadiusChanged);
            RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(RippleCanvas), radiusMetadata);
        }
        public RippleCanvas()
            : base()
        {
            Loaded += RippleCanvas_Loaded;
        }
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((RippleCanvas)d).Draw();
        }
        #endregion

        #region Visual details
        private double scale = 20;
        private SolidColorBrush background;
        private Pen pen;

        public ObservableCollection<PrimeFactors> Factors { get; private set; }
        //private Point[][] Lattice;
        private Dictionary<int, Point[]> Lattice;
        private int progress = 0;
        private double point_radius = 1;
        private bool loaded = false;
        private void RippleCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Factors = new ObservableCollection<PrimeFactors>(Gaussian.Factors.Select(x => x.Value));
            Lattice = Gaussian.Lattice(scale);
            visual_registry = new Dictionary<int, Visual>(Lattice.Count);
            background = Brushes.Transparent;
            var gradient = new LinearGradientBrush(new GradientStopCollection(new[] { new GradientStop(Colors.DarkMagenta, 0.0), new GradientStop(Colors.DarkBlue, .25), new GradientStop(Colors.DarkMagenta, 0.75) }));
            pen = new Pen(gradient, 1);

            Draw();
            DoubleAnimation radiusAnimation = new DoubleAnimation();
            radiusAnimation.From = 0;
            radiusAnimation.To = Radius;
            radiusAnimation.AutoReverse = true;
            radiusAnimation.RepeatBehavior = RepeatBehavior.Forever;
            radiusAnimation.Duration = TimeSpan.FromMinutes(.125);
            this.BeginAnimation(RippleCanvas.RadiusProperty, radiusAnimation);
            loaded = true;
        }

        DrawingVisual circle_visual = new DrawingVisual();
        double last_radiusSquared = 0;
        public void Draw()
        {
            if (loaded)
            {
                DeleteVisual(circle_visual);
                DrawingVisual visual = new DrawingVisual();

                var circle_radius = Radius * scale;
                using (DrawingContext dc = circle_visual.RenderOpen())
                {
                    dc.DrawEllipse(background, pen, new Point(0, 0), circle_radius, circle_radius);
                }
                this.AddVisual(circle_visual);

                var temp_radius = RadiusSquared;
                var forward = temp_radius - last_radiusSquared >= 0;
                last_radiusSquared = temp_radius;
                while (progress < RadiusSquared && progress < Lattice.Count() && forward)
                {
                    visual = new DrawingVisual();
                    using (DrawingContext dc = visual.RenderOpen())
                    {
                        for (int j = 0; j < Lattice[progress].Length; j++)
                        {
                            dc.DrawEllipse(background, pen, Lattice[progress][j], point_radius, point_radius);
                        }
                    }
                    this.RegisterAndAddVisual(visual, progress);
                    progress++;
                }
                while(progress > RadiusSquared && progress > 0 && !forward)
                {
                    DeleteVisual(visual_registry[progress - 1]);
                    progress--;
                }

            }
        }
        #endregion

        #region Visual data controls
        private List<Visual> visuals = new List<Visual>();
        private Dictionary<int, Visual> visual_registry = new Dictionary<int, Visual>();
        protected override int VisualChildrenCount {
            get { return visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        public void RegisterAndAddVisual(Visual visual, int index)
        {
            visual_registry[index] = visual;
            AddVisual(visual);
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