using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Timers;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdminPanel
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    private ObservableCollection<Host> _hosts;
    private string _hostApiUrl = "http://localhost:10500/api/hosts";
    private Timer _timer = new Timer();
    public ObservableCollection<Host> Hosts
    {
      get
      {
        return _hosts;
      }
      set
      {
        _hosts = value;
        OnPropertyChanged();
      }
    }
    public MainWindow()
    {
      InitializeComponent();
      DataContext = this;
      _timer.Elapsed += GetData;
      _timer.Start();
    }
    
    private void GetData(object sender, ElapsedEventArgs e)
    {
      try
      {
        using (var client = new HttpClient())
        {
          HttpResponseMessage Response = client.GetAsync(_hostApiUrl).Result;
          HttpContent responseContent = Response.Content;
          var json = responseContent.ReadAsStringAsync().Result;
          Hosts = JsonConvert.DeserializeObject<ObservableCollection<Host>>(json);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Произошла ошибка!\n" + ex.Message);
      }
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var text = (RefreshComboBox.SelectedItem as TextBlock).Text;
      var parsedText = int.Parse(text);
      _timer.Interval = parsedText;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      AddHost();
    }

    private async void AddHost()
    {
      HttpResponseMessage response;
      var host = new HMLib.Host
      {
        IpAddress = TextBoxAddress.Text,
        Name = TextBoxHostName.Text,
        Condition = true
      };
      var hostInJson = JsonConvert.SerializeObject(host);
      try
      {
        using (var client = new HttpClient())
        {
          response = await client.PostAsync(_hostApiUrl, new StringContent(hostInJson, Encoding.UTF8, "application/json"));
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Произошла ошибка!\n" + ex.Message);
      }
    }



    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string prop = "")
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
  }
}
