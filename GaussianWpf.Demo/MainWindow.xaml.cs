using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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
using DynamicData;

namespace GradientDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random r = new Random();
        private readonly ReadOnlyObservableCollection<RatioViewModel> collection;

        public MainWindow()
        {
            InitializeComponent();

            GradientStopAnimationExample();


            Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(a => new RatioViewModel(r.Next(0, 10), r.Next(0, 100)))
                .ToObservableChangeSet(a => a.Key)
                .ObserveOnDispatcher()
                .DisposeMany()
                .Bind(out collection)
    
                .Subscribe();

            ItemsControl1.ItemsSource = collection;

        }

        

        public void GradientStopAnimationExample()
        {
            Title = "GradientStop Animation Example";
            Background = Brushes.White;
            //
            DoubleAnimation offsetAnimation = new DoubleAnimation();
            offsetAnimation.From = 0.0;
            offsetAnimation.To = 1.0;
            offsetAnimation.Duration = TimeSpan.FromSeconds(1.5);
            offsetAnimation.AutoReverse = true;

         

            Storyboard.SetTargetName(offsetAnimation, "GradientStop1");

            Storyboard.SetTargetProperty(offsetAnimation,
                new PropertyPath(GradientStop.OffsetProperty));


            Rectangle1.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                Storyboard gradientStopAnimationStoryboard = new Storyboard();

                gradientStopAnimationStoryboard.Children.Add(offsetAnimation);
                gradientStopAnimationStoryboard.Begin(this);
            };


        }



        private void GaussianControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          
            GaussianControl1.Ratio = r.Next(0,100);
        }




    }


    public class RatioViewModel
    {

        public readonly int Key;

        int ratio;



        public RatioViewModel(int key, int ratio)
        {
            Key = key;
            Ratio = ratio;

        }

        public int Ratio { get => ratio; set => ratio = value; }

  
    }
}
