using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GaussianWpf
{

    public class GaussianControl : Control
    {
        private ISubject<Rectangle> rect1Subject = new Subject<Rectangle>();
        private ISubject<int> ratioSubject = new Subject<int>();

        static GaussianControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GaussianControl), new FrameworkPropertyMetadata(typeof(GaussianControl)));
        }

        public override void OnApplyTemplate()
        {
            rect1Subject.OnNext(this.GetTemplateChild("Rectangle1") as Rectangle);

            base.OnApplyTemplate();
        }



        public int Ratio
        {
            get { return (int)GetValue(RatioProperty); }
            set { SetValue(RatioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ratio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatioProperty =
            DependencyProperty.Register("Ratio", typeof(int), typeof(GaussianControl), new PropertyMetadata(50, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GaussianControl).ratioSubject.OnNext((int)e.NewValue);
        }

        public GaussianControl()
        {

            rect1Subject
                .CombineLatest(ratioSubject, (a, b) => (a, b))
                .Subscribe(c =>
                {
                    GradientStopAnimationExample(c.a,c.b / 100d);
                });
        }

        public void GradientStopAnimationExample(Rectangle rectangle, double to)
        {
            //NameScope.SetNameScope(rectangle, new NameScope());

            var r = (rectangle.Fill as LinearGradientBrush);
            var z = r.GradientStops[1];
      
            DoubleAnimation offsetAnimation = new DoubleAnimation();
     
            offsetAnimation.To = to;
            offsetAnimation.Duration = TimeSpan.FromSeconds(1.5);

            z.BeginAnimation(GradientStop.OffsetProperty,offsetAnimation);
        }
    }
}
