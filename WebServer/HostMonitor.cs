using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using WebServer.Models;

namespace WebServer
{
    public class PingerEventArgs
    {
        public IEnumerable<Log> PingResults { get; set; }
    }

    public class HostMonitor
    {
        public bool IsWorking {
            get
            {
                return _isWorking;
            }
        }
        public EventHandler<PingerEventArgs> OnPingCompleted;

        private bool _isWorking = false;
        //IConfiguration configuration;
        private HMLib.Settings _settings = null;
        private HMLib.Pinger _pinger;
        private readonly List<Host> _hosts;

        public HostMonitor(HMLib.Settings settings, IEnumerable<Host> hosts)
        {
            _settings = settings;
            _hosts = hosts.ToList();

            ////var intt = ConfigHelper.GetProperty("UserSettings:Ttl");
            //MessageParams.MailTo = settings.MailTo;
            //MessageParams.ReplyTo = settings.ReplyTo;
            //MessageParams.SenderName = settings.SenderName;
            //MessageParams.TextFormat = MimeKit.Text.TextFormat.Text;
        }

        public void Start()
        {
            Thread pingWorker = new Thread(new ThreadStart(PingWorker));
            pingWorker.Start();
            _isWorking = true;
        }

        public void PingWorker()
        {
            //MailSendAdapter emailSendAdapter = new MailSendAdapter(
            //    SmtpServer: settings.SmtpServer,
            //    SmtpPort: settings.SmtpPort,
            //    Login: settings.Email, Password: settings.Password);

            _pinger = new HMLib.Pinger(_hosts.Select(h => IPAddress.Parse(h.IpAddress)), _settings.TimeOut, _settings.Ttl, _settings.DataSize);

            while (true)
            {
                var logTime = DateTime.Now;
                var pingResults = _pinger.Pereimenovat()
                    .Select(t => new Log
                    {
                        Delay = (t.Item1 != null) ? (t.Item1.Status != IPStatus.Success) ? -1 : ((int)t.Item1.RoundtripTime == 0) ? 1 : (int)t.Item1.RoundtripTime : -1,
                        TimeStamp = logTime,
                        IpAddress = t.Item2.ToString()
                    });
                OnPingCompleted?.Invoke(this, new PingerEventArgs { PingResults = pingResults.ToList() });
                
                Thread.Sleep(_settings.PingInterval);
            }
        }
    }
}
