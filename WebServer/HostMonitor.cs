using HMLib;
using Microsoft.AspNetCore.Http;
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
    public class HostMonitor
    {
        private readonly MonitorContext _context;
        public Settings Settings = new Settings();

        public HostMonitor(MonitorContext context)
        {
            _context = context;
            MessageParams.MailTo = Settings.MailTo;
            MessageParams.ReplyTo = Settings.ReplyTo;
            MessageParams.SenderName = Settings.SenderName;
            MessageParams.TextFormat = MimeKit.Text.TextFormat.Text;

            Thread DoPing = new Thread(new ThreadStart(PingWorker));
            DoPing.Start();
        }


        public void PingWorker()
        {
            List<Host> hosts = JsonConvert.DeserializeObject<List<Host>>(File.ReadAllText(settings.PathToHostsFile));
            List<Host> changedHosts = new List<Host>();
            MailSendAdapter emailSendAdapter = new MailSendAdapter(
                SmtpServer: Settings.SmtpServer,
                SmtpPort: Settings.SmtpPort,
                Login: Settings.Email, Password: Settings.Password);

            while (true)
            {
                hosts = CheckStatus(hosts);

                changedHosts = hosts.FindAll(ChangedStatus);

                /*delegate (Host host)
                {
                    return host.StatusChanged == true;
                }
                );
                */

                foreach (Host host in changedHosts)
                {
                    MessageParams message = new MessageParams
                    {
                        Body = "Хост: " + host.Name + "с ip: " + host.IP + " изменил статус на " + host.Status,
                        Caption = host.Name + " статус " + host.Status
                    };
                    emailSendAdapter.Send(message); //SendAsync чёт не работает
                }
                Thread.Sleep(Settings.PingInterval);
            }
        }

        private bool ChangedStatus(Host host)
        {

            if (host.StatusChanged == true)
            {
                host.StatusChanged = false;
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<Host> CheckStatus(List<Host> hosts)
        {
            PingReply pingReply;

            Pinger SendPing = new Pinger
            {
                DataSize = Settings.DataSize,      //
                TimeOut = Settings.TimeOut,    //TODO:Перенести в класс, сделать файл с настройками
                Ttl = Settings.Ttl         //
            };
            int count = hosts.Count;
            Task[] tasks = new Task[count];
            int taskIndex = -1;

            foreach (Host host in hosts)
            {
                taskIndex++;
                tasks[taskIndex] = Task.Factory.StartNew(() =>  //перенести проверку статуса куда-нибудь.
                {                                               //оставить только пинг и возвращать лист пингрезултов
                    if (host.Condition == true)
                    {
                        pingReply = SendPing.Ping(host.IP);
                        if (pingReply.Status.ToString() != host.Status && !String.IsNullOrEmpty(host.Status))
                        {
                            host.StatusChanged = true;
                            Console.WriteLine("Хост: " + host.Name + "с ip: " + host.IP + " изменил статус с " + host.Status + " на " + pingReply.Status.ToString());
                        }
                        host.Status = pingReply.Status.ToString();
                    }
                });
            }
            Task.WaitAll(tasks);
            return hosts;
        }
    }
}
