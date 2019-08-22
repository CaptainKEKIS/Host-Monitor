using System;
using System.Collections.Generic;
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

            List<MessageParams> Messages = new List<MessageParams>();
            MailSendAdapter emailSendAdapter = new MailSendAdapter(
                SmtpServer: settings.SmtpServer,
                SmtpPort: settings.SmtpPort,
                Login: settings.Email, Password: settings.Password);

            while (true)
            {
                Messages = CheckStatus(settings.PathToHostsFile);
                foreach (MessageParams message in Messages)
                {
                    emailSendAdapter.Send(message); //SendAsync чёт не работает
                }
                Thread.Sleep(settings.PingInterval);
            }
        }

        public static List<MessageParams> CheckStatus(string Path)
        {
            List<Host> Hosts = Host.ReadHostsFromFile(Path);
            List<MessageParams> Message = new List<MessageParams>();

            Pinger SendPing = new Pinger
            {
                DataSize = settings.DataSize,      //
                TimeOut = settings.TimeOut,    //TODO:Перенести в класс, сделать файл с настройками
                Ttl = settings.Ttl         //
            };
            int count = Hosts.Count;
            Task[] Tasks = new Task[count];
            int TaskIndex = -1;

            foreach (Host host in Hosts)
            {
                TaskIndex++;
                Tasks[TaskIndex] = Task.Factory.StartNew(() =>
                {
                    string Status = "";
                    if(host.Condition == true)
                    {
                        Status = SendPing.GetStatus(host.IP);
                        if (Status != "Success" && Status != host.Status)
                        {
                            Message.Add(new MessageParams
                            {
                                Body = "Хост: " + host.Name + " с ip: " + host.IP + " офлайн. Причина: " + Status + ".",
                                Caption = host.Name + " упал."
                            });
                            Console.WriteLine("Упал. Хост: " + host.Name + "с ip: " + host.IP + " " + Status);
                        }
                        else if (Status == "Success" && Status != host.Status && host.Status != null)
                        {
                            Message.Add(new MessageParams
                            {
                                Body = "Хост: " + host.Name + " с ip: " + host.IP + " снова онлайн.",
                                Caption = host.Name + " поднялся."
                            });
                            Console.WriteLine("Поднялся. Хост: " + host.Name + "с ip: " + host.IP + " " + Status);
                        }
                        else
                        {
                            Console.WriteLine("Хост: " + host.Name + "с ip: " + host.IP + " " + Status);
                        }
                        host.Status = Status;
                    }
                });
            }
            Task.WaitAll(Tasks);

            Host.WriteHostsToFile(Hosts, Path);
            return Message;
        }
    }
}
