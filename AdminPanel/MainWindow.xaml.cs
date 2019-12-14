using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
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

namespace AdminPanel
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<Host> hosts = new List<Host>();

            string address = "http://localhost:10500/api/hosts";
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage Response = client.GetAsync(address).Result;
                    if (Response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent responseContent = Response.Content;
                        var json = responseContent.ReadAsStringAsync().Result;
                        hosts = JsonConvert.DeserializeObject<List<Host>>(json);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            ResultDataGrid.ItemsSource = hosts;
        }
    }
}

