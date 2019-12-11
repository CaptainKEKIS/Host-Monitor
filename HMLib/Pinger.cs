using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HMLib
{
    public class Pinger : IDisposable
    {
        private byte[] _pingBuffer;

        public int TimeOut { get; set; }
        public int Ttl { get; set; }
        public int DataSize { get; set; }
        private IPAddress[] EndPoints { get; set; }
        private int Count => EndPoints.Length;
        private PingOptions POptions { get; set; }
        private byte[] PingBuffer
        {
            get
            {
                if (_pingBuffer == null)
                {
                    _pingBuffer = new byte[DataSize];
                    Random rnd = new Random();
                    rnd.NextBytes(_pingBuffer);
                }
                return _pingBuffer;
            }
        }
        //public PingReply[] PingResults { get; private set; }
        private Ping[] Pings { get; set; }

        public Pinger(IEnumerable<IPAddress> addresses, int TimeOut, int Ttl, int DataSize)
        {
            EndPoints = addresses.ToArray();
            this.TimeOut = TimeOut;
            this.Ttl = Ttl;
            POptions = new PingOptions(Ttl, false);
            this.DataSize = DataSize;
            Pings = EndPoints.Select(h => new Ping()).ToArray();
            //PingResults = new PingReply[Count];
        }

        public List<Tuple<PingReply, object>> Pereimenovat()
        {
            //var tasks = new Task<PingReply>[Count];
            var pingTasks = new List<Task<Tuple<PingReply, object>>>();
            for (int i = 0; i < Count; i++)
            {
                pingTasks.Add(PingAsync(Pings[i], EndPoints[i]));
            }
            Task.WaitAll(pingTasks.ToArray());
            var pingResults = pingTasks.Select(pt => pt.Result).ToList();
            return pingResults;
        }

        private Task<Tuple<PingReply, object>> PingAsync(Ping ping, IPAddress epIP)
        {
            try
            {
                var tcs = new TaskCompletionSource<Tuple<PingReply, object>>();
                PingCompletedEventHandler act = null;

                act = (obj, sender) =>
                {
                    ping.PingCompleted -= act;
                    Console.WriteLine($"host: {sender.UserState}\tdelay: {((sender.Reply != null) ? (sender.Reply.Status != IPStatus.Success) ? -1 : ((int)sender.Reply.RoundtripTime == 0) ? 1 : (int)sender.Reply.RoundtripTime : -1)}\tstatus: {((sender.Reply != null) ? sender.Reply.Status : IPStatus.Unknown)}");
                    tcs.SetResult(Tuple.Create(sender.Reply, sender.UserState));
                };
                ping.PingCompleted += act;
                ping.SendAsync(epIP, TimeOut, PingBuffer, POptions, epIP);
                return tcs.Task;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Пока не надо
        public async Task<string> GetStatusAsync(string IpAddresses)
        {
            Ping Piping = new Ping();
            PingReply Reply;
            string Status = "";
            PingOptions POptions = new PingOptions(Ttl, false);
            //byte[] PingBuffer = new byte[DataSize];
            //Random rnd = new Random();
            //rnd.NextBytes(PingBuffer);
            Reply = await Piping.SendPingAsync(IpAddresses, TimeOut, _pingBuffer, POptions);
            Status = Reply.Status.ToString();
            return Status;
        }

        public string GetStatus(string IpAddresses)
        {
            Ping Piping = new Ping();
            PingReply Reply;
            string Status = "";
            PingOptions POptions = new PingOptions(Ttl, false);
            byte[] PingBuffer = new byte[DataSize];
            Random rnd = new Random();
            rnd.NextBytes(PingBuffer);
            Reply = Piping.Send(IpAddresses, TimeOut, PingBuffer, POptions);
            Status = Reply.Status.ToString();
            return Status;
        }

        public PingReply Ping(string IpAddress)
        {
            Ping Piping = new Ping();
            PingReply pingReply;
            IPAddress Address = IPAddress.Parse(IpAddress);
            PingOptions POptions = new PingOptions(Ttl, false);
            byte[] PingBuffer = new byte[DataSize];
            Random rnd = new Random();
            rnd.NextBytes(PingBuffer);
            pingReply = Piping.Send(Address, TimeOut, PingBuffer, POptions);
            return pingReply;
        }

        public void PingAsync(string IpAddress)
        {
            Ping Piping = new Ping();
            IPAddress Address = IPAddress.Parse(IpAddress);
            PingOptions POptions = new PingOptions(Ttl, false);
            byte[] PingBuffer = new byte[DataSize];
            Random rnd = new Random();
            rnd.NextBytes(PingBuffer);
            Piping.PingCompleted += new PingCompletedEventHandler(PingComplete);
            Piping.SendAsync(Address, TimeOut, PingBuffer, POptions);
        }

        private void PingComplete(object sender, PingCompletedEventArgs e)
        {
            PingReply Reply;
            if (e.Cancelled)
            {
                Console.WriteLine("Ping Cencelled");
                ((AutoResetEvent)e.UserState).Set();
            }
            else if (e.Error != null)
            {
                Console.WriteLine("Fatal error!!!" + e.Error);
                ((AutoResetEvent)e.UserState).Set();
            }
            else
            {
                Reply = e.Reply;
                PingResult(Reply);
            }
        }

        private void PingResult(PingReply Reply)
        {
            /*
            Status = Reply.Status.ToString();
            if (Reply.RoundtripTime.ToString() == "0" && Reply.Status.ToString() == "Success")
            {
                Delay = "<1";
            }
            else
            {
                Delay = Reply.RoundtripTime.ToString();
            }

            try//TODO: Починить отображение имени хоста
            {
                HostName = Dns.GetHostEntry(_address).HostName;
            }
            catch (Exception)
            {
                //_hostName = "Mission Failed!";
                HostName = _address.ToString();
            }
            */
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~Pinger() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
