using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
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
using System.Xml;
using System.IO;

namespace Aliyun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ArrayList instances = new ArrayList();
        public ECSAPI.ImageType imageType = ECSAPI.ImageType.Type0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadUserData();

        }

        public void ReadUserData()
        {
            if (File.Exists("userdata.ini"))
            {
                try
                {
                    FileStream userData = new FileStream("userdata.ini", FileMode.Open);
                    using (StreamReader reader = new StreamReader(userData))
                    {

                        ECSAPI.UserData.AccessKeyId = reader.ReadLine();
                        ECSAPI.UserData.Secret = reader.ReadLine();
                        ECSAPI.UserData.SecurityGroupId = reader.ReadLine();
                        ECSAPI.UserData.DefaultPassword = reader.ReadLine();
                        ECSAPI.UserData.DefaultHostName = reader.ReadLine();
                        ECSAPI.UserData.ImageTypeId0 = reader.ReadLine();
                        ECSAPI.UserData.ImageTypeId1 = reader.ReadLine();
                        ECSAPI.UserData.ImageTypeName0 = reader.ReadLine();
                        ECSAPI.UserData.ImageTypeName1 = reader.ReadLine();
                    }
                    userData.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to read userdata.ini.");
                }
            }
            else
            {
                CreateUserData();
            }
        }

        public void CreateUserData()
        {
            Window userDataFormWnd = new UserDataForm();
            userDataFormWnd.Owner = this;
            Hide();
            userDataFormWnd.ShowDialog();
            ReadUserData();
        }

        async void GetInstanceList(object sender, RoutedEventArgs e)
        {
            var url = ECSAPI.GetInstanceList();
            string content;
            //Clipboard.SetText(url); return;
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    content = await response.Content.ReadAsStringAsync();
                    RefreshInstanceList(content);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                    return;
                }
            }
        }

        void RefreshInstanceList(string response)
        {
            InstanceList.Children.Clear();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(response);

            XmlNodeList nodeList = xmlDocument.SelectNodes("/DescribeInstancesResponse/Instances/Instance");

            instances.Clear();
            for (int i = 0; i < nodeList.Count; i++)
            {
                RadioButton radio = new RadioButton();
                radio.Margin = new Thickness(10, 0, 10, 0);
                radio.VerticalContentAlignment = VerticalAlignment.Center;
                if (nodeList[i]["PublicIpAddress"].HasChildNodes)
                {
                    radio.Content += nodeList[i]["PublicIpAddress"]["IpAddress"].InnerText + "  ";
                }
                radio.Content += nodeList[i]["CreationTime"].InnerText;
                radio.Content += " (";
                radio.Content += nodeList[i]["Status"].InnerText;
                radio.Content += ")";
                radio.Name = "Instance" + i.ToString();
                InstanceList.Children.Add(radio);
                instances.Add(nodeList[i]["InstanceId"].InnerText);
            }
        }

        async void CreateInstance(object sender, RoutedEventArgs e)
        {
            Window instanceTypeWnd = new SpecifyInstanceType();
            instanceTypeWnd.Owner = this;
            Hide();
            instanceTypeWnd.ShowDialog();
            var url = ECSAPI.CreateInstance(imageType);
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("Action succeeded.");
                    GetInstanceList(this, new RoutedEventArgs());
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                    return;
                }
            }
        }

        public string GetSelectedInstance()
        {
            var radios = InstanceList.Children.OfType<RadioButton>();
            foreach (var ctrl in radios)
            {
                if (ctrl.IsChecked == true)
                {
                    return (string)instances[int.Parse(ctrl.Name.Substring(8, ctrl.Name.Length - 8))];
                }
            }
            return "Not found";
        }

        async void DeleteInstance(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.DeleteInstance(instanceId);
                var radios = InstanceList.Children.OfType<RadioButton>();
                string instanceAlias = "";
                foreach (var ctrl in radios)
                {
                    if (ctrl.IsChecked == true)
                    {
                        instanceAlias = ctrl.Content.ToString();
                        break;
                    }
                }
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete instance " + instanceAlias + "?", "Confirm your operation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }

        async void AllocateIP(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.AllocateIP(instanceId);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }

        async void StartInstance(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.StartInstance(instanceId);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }

        async void StopInstance(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.StopInstance(instanceId);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }

        async void RebootInstance(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.RebootInstance(instanceId);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }

        async void ModifyPassword(object sender, RoutedEventArgs e)
        {
            string instanceId = GetSelectedInstance();
            if (instanceId != "Not found")
            {
                var url = ECSAPI.ModifyPassword(instanceId);
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        MessageBox.Show("Action succeeded.");
                        GetInstanceList(this, new RoutedEventArgs());
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.ToString());
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an instance.");
            }
        }
    }
}
