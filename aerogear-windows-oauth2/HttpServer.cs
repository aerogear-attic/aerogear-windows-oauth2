using System;
using System.Collections.Generic;
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
        private int mPort;
        private StreamSocketListener mListener;
        private bool mIsActive = false;

        public bool IsActive
        {
            get { return mIsActive; }
        }

        public HttpServer(int port)
        {
            mPort = port;
        }

        public async void Start()
        {
            if (!mIsActive)
            {
                mIsActive = true;
                mListener = new StreamSocketListener();
                mListener.Control.QualityOfService = SocketQualityOfService.Normal;
                mListener.ConnectionReceived += mListener_ConnectionReceived;
                await mListener.BindServiceNameAsync(mPort.ToString());
            }

        }

        public void Stop()
        {
            if (mIsActive)
            {
                mListener.Dispose();
                mIsActive = false;
            }
        }

        async void mListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            await Task.Run(() =>
            {
                HandleRequest(args.Socket);
            });
        }

        private async void HandleRequest(StreamSocket socket)
        {
            //Initialize IO classes
            DataReader reader = new DataReader(socket.InputStream);
            DataWriter writer = new DataWriter(socket.OutputStream);

            //handle actual HTTP request
            String request = await StreamReadLine(reader);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            string httpUrl = tokens[1];

            //read HTTP headers - contents ignored in this sample
            while (!String.IsNullOrEmpty(await StreamReadLine(reader))) ;

            StringBuilder ret = new StringBuilder();

            ret.AppendLine("HTTP/1.1 301 Moved Permanently");
            ret.AppendLine("Location: aerogear-oauth:" + httpUrl.Substring(1));
            ret.AppendLine("");

            writer.WriteString(ret.ToString());
            await writer.StoreAsync();

            socket.Dispose();
            //Stop();
        }

        #region static Helper methods
        private static async Task<string> StreamReadLine(DataReader reader)
        {
            int next_char;
            string data = "";
            while (true)
            {
                await reader.LoadAsync(1);
                next_char = reader.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        #endregion static Helper methods
    }
}
