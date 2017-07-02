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
using System.IO;

namespace Aliyun
{
    /// <summary>
    /// Interaction logic for UserDataForm.xaml
    /// </summary>
    public partial class UserDataForm : Window
    {
        private bool confirmed = false;

        public UserDataForm()
        {
            InitializeComponent();
        }

        private void ReportUserData(object sender, RoutedEventArgs e)
        {
            confirmed = true;
            var parentWnd = Owner as MainWindow;
            
            FileStream userData = new FileStream("userdata.ini", FileMode.Create);
            using (StreamWriter writer = new StreamWriter(userData))
            {
                writer.WriteLine(AccessKeyIdInput.Text);
                writer.WriteLine(SecretInput.Text);
                writer.WriteLine(SecurityGroupIdInput.Text);
                writer.WriteLine(DefaultPasswordInput.Text);
                writer.WriteLine(DefaultHostNameInput.Text);
                writer.WriteLine(ImageType0IdInput.Text);
                writer.WriteLine(ImageType1IdInput.Text);
                writer.WriteLine(ImageType0NameInput.Text);
                writer.WriteLine(ImageType1NameInput.Text);
            }
            userData.Close();

            parentWnd.Show();
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!confirmed)
            {
                var parentWnd = Owner as MainWindow;
                parentWnd.Close();
            }
        }
    }
}
