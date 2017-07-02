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
using System.Windows.Shapes;

namespace Aliyun
{
    /// <summary>
    /// Interaction logic for SpecifyInstanceType.xaml
    /// </summary>
    public partial class SpecifyInstanceType : Window
    {
        public SpecifyInstanceType()
        {
            InitializeComponent();
            ImageType0.Content = ECSAPI.UserData.ImageTypeName0;
            ImageType1.Content = ECSAPI.UserData.ImageTypeName1;
        }

        private void ReportSelection(object sender, RoutedEventArgs e)
        {
            var parentWnd = Owner as MainWindow;
            if (ImageType0.IsChecked == true)
            {
                parentWnd.imageType = ECSAPI.ImageType.Type0;
            }else if (ImageType1.IsChecked == true)
            {
                parentWnd.imageType = ECSAPI.ImageType.Type1;
            }
            else
            {
                MessageBox.Show("Please select an image.");
                return;
            }
            parentWnd.Show();
            Close();
        }
    }
}
