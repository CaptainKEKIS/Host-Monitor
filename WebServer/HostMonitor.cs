using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
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

        public HostMonitor(HMLib.Settings settings, List<Host> hosts)
        {
            _settings = settings;
            _hosts = hosts;
            //var dbContextOptions = new DbContextOptionsBuilder<MonitorContext>()
            //    .UseSqlite(connection).Options;
            //_context = new MonitorContext(dbContextOptions);


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
                var pingResults = _pinger.Pereimenovat()
                    .Select(t => new Log
                    {
                        Delay = (t.Item1 != null) ? (t.Item1.Status != IPStatus.Success) ? -1 : (int)t.Item1.RoundtripTime : -1,
                        TimeStamp = DateTime.Now,
                        IpAddress = t.Item2.ToString()
                    });
                OnPingCompleted?.Invoke(this, new PingerEventArgs { PingResults = pingResults.ToList() });
                //RiseFinishEvent(pingResults.ToList());
                /*
                _context.Logs.AddRange(pingResults);
                _context.SaveChanges();

                foreach (Host host in changedHosts)
                {
                    MessageParams message = new MessageParams
                    {
                        //Body = "Хост: " + host.Name + "с ip: " + host.IpAddress + " изменил статус на " + host.Status,
                        //Caption = host.Name + " статус " + host.Status
                    };
                    emailSendAdapter.Send(message); //SendAsync чёт не работает
                }
                */
                Thread.Sleep(_settings.PingInterval);
            }
        }
    }
}
