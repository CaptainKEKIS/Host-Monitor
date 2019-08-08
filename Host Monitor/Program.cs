using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Host_Monitor
{
    class Program
    {
        static Settings settings = new Settings();

        static void Main(string[] args)
        {
            settings = settings.ReadFromFile("Settings.json");
            MailSendAdapter emailSendAdapter = new MailSendAdapter(
                SmtpServer: settings.SmtpServer,
                SmtpPort: settings.SmtpPort,
                Login: settings.Email, Password: settings.Password);

            List<MessageParams> Messages = new List<MessageParams>();
            MessageParams.MailTo = settings.MailTo;
            MessageParams.ReplyTo = settings.ReplyTo;
            MessageParams.SenderName = settings.SenderName;
            MessageParams.TextFormat = MimeKit.Text.TextFormat.Text;

            Messages = CheckStatus(settings.PathToHostsFile);

            foreach (MessageParams message in Messages)
            {
                emailSendAdapter.Send(message); //SendAsync чёт не работает
            }

            //Host.WriteHostsToFile(Hosts, "Hosts.json");
            //List<Host> Hosts2 = Host.ReadHostsFromFile("Hosts.json");
            //Console.WriteLine(Hosts2[0].Name + "\n" + Hosts2[0].IP + "\n");             
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
            Task[] Task1 = new Task[count];
            int TaskIndex = -1;

            foreach (Host host in Hosts)
            {
                TaskIndex++;
                Task1[TaskIndex] = Task.Factory.StartNew(() =>
                {
                    string Status = "";
                    Status = SendPing.GetStatus(host.IP);
                    if (Status != "Success" && Status != host.Status && host.Condition == true)
                    {
                        Message.Add(new MessageParams
                        {
                            Body = "Хост: " + host.Name + " с ip: " + host.IP + " офлайн. Причина: " + Status + ".",
                            Caption = host.Name + " упал."
                        });
                        Console.WriteLine("Упал. Хост: " + host.Name + "с ip: " + host.IP + " " + Status);
                    }
                    else if (Status == "Success" && Status != host.Status && host.Condition == true && host.Status != null)
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
                });
            }
            Task.WaitAll(Task1);

            Host.WriteHostsToFile(Hosts, Path);
            return Message;
        }


    }
}
