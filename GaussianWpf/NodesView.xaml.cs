using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GaussianWpf
{
    /// <summary>
    /// Interaction logic for NodesView.xaml
    /// </summary>
    public partial class NodesView : UserControl
    {
        public NodesView()
        {
            InitializeComponent();
        }

        //public IEnumerable Nodes
        //{
        //    get { return (IEnumerable)GetValue(NodesProperty); }
        //    set { SetValue(NodesProperty, value); }
        //}

        //public static readonly DependencyProperty NodesProperty = DependencyProperty.Register("Nodes", typeof(IEnumerable), typeof(NodesView), new PropertyMetadata(null, Changed));

        //private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as NodesView).NodesControl.ItemsSource = (e.NewValue as IEnumerable);

            
        //}
    }
}
