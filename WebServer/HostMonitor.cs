﻿using Microsoft.AspNetCore.Http;
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
    public class HostMonitor
    {
        private readonly MonitorContext _context;
        //IConfiguration configuration;
        public HMLib.Settings settings = null;
        public HMLib.Pinger pinger;

        public HostMonitor()
        {
            settings = ConfigHelper.GetSettings();       //получаю настройки из конфигурации
            string connection = ConfigHelper.GetDefaultConnection();        //получаю строку подключения из конфигурации
            var dbContextOptions = new DbContextOptionsBuilder<MonitorContext>()
                .UseSqlite(connection).Options;
            _context = new MonitorContext(dbContextOptions);

            pinger = new HMLib.Pinger
            {
                DataSize = settings.DataSize,
                TimeOut = settings.TimeOut,
                Ttl = settings.Ttl
            };

            //var intt = ConfigHelper.GetProperty("UserSettings:Ttl");
            MessageParams.MailTo = settings.MailTo;
            MessageParams.ReplyTo = settings.ReplyTo;
            MessageParams.SenderName = settings.SenderName;
            MessageParams.TextFormat = MimeKit.Text.TextFormat.Text;

            Thread pingWorker = new Thread(new ThreadStart(PingWorker));
            pingWorker.Start();
        }

        public void PingWorker()
        {
            List<Host> hosts = _context.Hosts.ToList();
            List<Host> changedHosts = new List<Host>();

            MailSendAdapter emailSendAdapter = new MailSendAdapter(
                SmtpServer: settings.SmtpServer,
                SmtpPort: settings.SmtpPort,
                Login: settings.Email, Password: settings.Password);

            while (true)
            {

                var tasks = new Task[hosts.Count];
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        PingReply pingReply;
                        Log log = new Log();
                        pingReply = pinger.Ping(hosts[i].IpAddress);
                        log.Status = pingReply.Status.ToString();
                        log.Delay = (int)pingReply.RoundtripTime;
                        log.TimeStamp = DateTime.Now;
                        log.HostId = hosts[i].Id; //// i всегда 4
                            //log.Host = host;
                            _context.Logs.Add(log);
                    }
                    );
                }
                Task.WaitAll(tasks);
                _context.SaveChanges();
                //changedHosts = hosts.FindAll(ChangedStatus);

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
                        //Body = "Хост: " + host.Name + "с ip: " + host.IpAddress + " изменил статус на " + host.Status,
                        //Caption = host.Name + " статус " + host.Status
                    };
                    emailSendAdapter.Send(message); //SendAsync чёт не работает
                }
                Thread.Sleep(settings.PingInterval);
            }
        }

        //private bool ChangedStatus(Host host)
        //{

        //    if (host.StatusChanged == true)
        //    {
        //        host.StatusChanged = false;
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public List<Host> CheckStatus(List<Host> hosts)
        {
            PingReply pingReply;


            int count = hosts.Count;
            Task[] tasks = new Task[count];
            int taskIndex = -1;

            foreach (Host host in hosts)
            {
                taskIndex++;
                tasks[taskIndex] = Task.Factory.StartNew(() =>
                {
                    if (host.Condition == true)
                    {
                        //pingReply = SendPing.Ping(host.IP);
                        //if (pingReply.Status.ToString() != host.Status && !String.IsNullOrEmpty(host.Status))
                        //{
                        //    host.StatusChanged = true;
                        //    Console.WriteLine("Хост: " + host.Name + "с ip: " + host.IP + " изменил статус с " + host.Status + " на " + pingReply.Status.ToString());
                        //}
                        //host.Status = pingReply.Status.ToString();
                    }
                });
            }
            Task.WaitAll(tasks);
            return hosts;
        }
    }
}
