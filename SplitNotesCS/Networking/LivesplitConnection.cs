using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SplitNotesCS.Networking
{
    class LivesplitConnection
    {

        const int BUFFER_SIZE = 4096;

        readonly string hostname;
        readonly int port;

        private Socket connection;

        public bool Connected { get; private set; } = false;

        double Timeout { get; set; }


        public LivesplitConnection(string hostname = "localhost", int port = 16834, double timeout = 2.0)
        {
            this.hostname = hostname;
            this.port = port;
            this.Timeout = timeout;
        }

        public void Connect()
        {
            // IPV4, Stream, TCP Connection
            this.connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.connection.Connect(this.hostname, this.port);
                this.connection.ReceiveTimeout = Convert.ToInt32(this.Timeout * 1000.0);
                this.Connected = true;
            }
            catch (SocketException e) {
                this.Connected = false;
                throw e;  // Rethrow the error to be handled elsewhere
            }
        }

        public void Disconnect()
        {
            this.connection.Disconnect(true);
            this.Connected = false;
        }

        public string SendReceive(string message)
        {
            message += "\r\n";
            string response = "";

            try
            {
                byte[] bMessage = Encoding.UTF8.GetBytes(message);
                this.connection.Send(bMessage);

                byte[] bResponse = new byte[BUFFER_SIZE];
                int bytesReceived = this.connection.Receive(bResponse);
                if (bytesReceived > 0)
                {
                    response = Encoding.UTF8.GetString(bResponse);
                }
            }
            catch (SocketException e)
            {
                this.Connected = false;
                throw e;
            }
            catch (ObjectDisposedException e)
            {
                this.Connected = false;
                throw e;
            }

            // Empty message indicates the connection is over
            if (response == "")
            {
                this.Connected = false;
            }

            return response;
        }

        public int GetIndex()
        {
            string indexAsString = this.SendReceive("getsplitindex").Trim();
            return Math.Max(Convert.ToInt32(indexAsString), 0);  // Always return at least 0 instead of -ve numbers when the splits aren't running.
        }
    }
}
