using System;
using System.Collections.Generic;
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

namespace NormalMath.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for NormalUserControl.xaml
    /// </summary>
    public partial class NormalView : UserControl
    {
        public NormalView()
        {
            InitializeComponent();
            this.DataContextChanged += NormalView_DataContextChanged;
        }

        private void NormalView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(NormalView), new PropertyMetadata(false));


    }
}
