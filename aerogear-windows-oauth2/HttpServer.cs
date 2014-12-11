using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace AeroGear.OAuth2
{
    public class HttpServer
    {
        private int port;
        private StreamSocketListener socketLlistener;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public async Task Start()
        {
            socketLlistener = new StreamSocketListener();
            socketLlistener.Control.QualityOfService = SocketQualityOfService.Normal;
            socketLlistener.ConnectionReceived += mListener_ConnectionReceived;
            await socketLlistener.BindServiceNameAsync(port.ToString());
        }

        public void Stop()
        {
            socketLlistener.Dispose();
            socketLlistener = null;
        }

        async void mListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            await Task.Run(async () =>
            {
                using (StreamSocket socket = args.Socket)
                {
                    using (DataReader reader = new DataReader(socket.InputStream))
                    {
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        String request = await StreamReadLine(reader);
                        string[] tokens = request.Split(' ');
                        if (tokens.Length != 3)
                        {
                            return;
                        }

                        string httpUrl = tokens[1];

                        using (DataWriter writer = new DataWriter(socket.OutputStream))
                        {
                            StringBuilder response = new StringBuilder();

                            response.AppendLine("HTTP/1.1 301 Moved Permanently");
                            response.AppendLine("Location: aerogear-oauth:" + httpUrl.Substring(1));
                            response.AppendLine("");

                            writer.WriteString(response.ToString());
                            await writer.StoreAsync();
                        }
                    }

                    Stop();
                }
            });
        }

        #region static Helper methods
        private static async Task<string> StreamReadLine(DataReader reader)
        {
            int currentChar = 0;
            string data = "";
            while (currentChar != '\n')
            {
                try
                {
                    uint op = await reader.LoadAsync(1);
                    if (op == 0) { break; };
                    currentChar = reader.ReadByte();
                    if (currentChar != '\r') 
                    {
                        data += Convert.ToChar(currentChar);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return data;
        }

        #endregion static Helper methods
    }
}
