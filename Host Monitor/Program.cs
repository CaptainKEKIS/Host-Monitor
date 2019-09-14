using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Host_Monitor
{
    class Program
    {
        static Settings settings = new Settings();

        static void Main(string[] args)
        {
            settings = settings.ReadFromFile("Settings.json");


            MessageParams.MailTo = settings.MailTo;
            MessageParams.ReplyTo = settings.ReplyTo;
            MessageParams.SenderName = settings.SenderName;
            MessageParams.TextFormat = MimeKit.Text.TextFormat.Text;

            Thread DoPing = new Thread(new ThreadStart(PingWorker));
            DoPing.Start();

            //Host.WriteHostsToFile(Hosts, "Hosts.json");
            //List<Host> Hosts2 = Host.ReadHostsFromFile("Hosts.json");
            //Console.WriteLine(Hosts2[0].Name + "\n" + Hosts2[0].IP + "\n");             
        }

        public static void PingWorker()
        {
            List<Host> hosts = JsonConvert.DeserializeObject<List<Host>>(settings.PathToHostsFile);
            List<Host> changedHosts = new List<Host>();
            MailSendAdapter emailSendAdapter = new MailSendAdapter(
                SmtpServer: settings.SmtpServer,
                SmtpPort: settings.SmtpPort,
                Login: settings.Email, Password: settings.Password);

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
                Thread.Sleep(settings.PingInterval);
            }
        }

        private static bool ChangedStatus(Host host)
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

        public static List<Host> CheckStatus(List<Host> hosts)
        {
            PingReply pingReply;

            Pinger SendPing = new Pinger
            {
                DataSize = settings.DataSize,      //
                TimeOut = settings.TimeOut,    //TODO:Перенести в класс, сделать файл с настройками
                Ttl = settings.Ttl         //
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




    /*
 status = SendPing.GetStatus(host.IP);
 if (pingReply.Status.ToString() != "Success" && pingReply.Status.ToString() != host.Status)
 {
     message.Add(new MessageParams
     {
         Body = "Хост: " + host.Name + " с ip: " + host.IP + " офлайн. Причина: " + status + ".",
         Caption = host.Name + " упал."
     });
     Console.WriteLine("Упал. Хост: " + host.Name + "с ip: " + host.IP + " " + status);
 }
 else if (status == "Success" && status != host.Status && host.Status != null)
 {
     message.Add(new MessageParams
     {
         Body = "Хост: " + host.Name + " с ip: " + host.IP + " снова онлайн.",
         Caption = host.Name + " поднялся."
     });
     Console.WriteLine("Поднялся. Хост: " + host.Name + "с ip: " + host.IP + " " + status);
 }
 else
 {
     Console.WriteLine("Хост: " + host.Name + "с ip: " + host.IP + " " + status);
 }
 */
}


