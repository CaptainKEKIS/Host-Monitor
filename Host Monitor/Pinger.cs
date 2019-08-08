using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Host_Monitor
{
    class Pinger
    {
        private int _ttl;
        private int _timeOut;
        private int _dataSize;

        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        public int Ttl
        {
            get { return _ttl; }
            set { _ttl = value; }
        }

        public int DataSize
        {
            get { return _dataSize; }
            set { _dataSize = value; }
        }

        public async Task<string> GetStatusAsync(string IpAddresses)
        {
            Ping Piping = new Ping();
            PingReply Reply;
            string Status = "";
            PingOptions POptions = new PingOptions(Ttl, false);
            byte[] PingBuffer = new byte[DataSize];
            Random rnd = new Random();
            rnd.NextBytes(PingBuffer);
            Reply = await Piping.SendPingAsync(IpAddresses, TimeOut, PingBuffer, POptions);
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

        #region Пока не надо
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
    }
}
