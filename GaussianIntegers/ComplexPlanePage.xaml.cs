using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace In_Extremis.Editor
{
    /// <summary>
    /// Interaction logic for ComplexPlanePage.xaml
    /// </summary>
    public partial class ComplexPlanePage : Page
    {
        public ComplexPlanePage()
        {
            InitializeComponent();
        }

        private void ComplexPlane_Loaded(object sender, RoutedEventArgs e)
        {
            //Numbers.ItemsSource = ComplexPlane.Factors;
        }
    }
}
