using System;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;

namespace Arrowgene.Baf.Server.Logging
{
    public class BafLogger : Logger
    {
        private BafSetting _setting;

        public override void Initialize(string identity, string name, Action<Log> write, object configuration)
        {
            base.Initialize(identity, name, write, configuration);
            _setting = configuration as BafSetting;
            if (_setting == null)
            {
                Error("Couldn't apply BafLogger configuration");
            }
        }

        public void Info(BafClient client, string message)
        {
            Info($"{client.Identity} {message}");
        }

        public void Debug(BafClient client, string message)
        {
            Debug($"{client.Identity} {message}");
        }

        public void Error(BafClient client, string message)
        {
            Error($"{client.Identity} {message}");
        }

        public void Exception(BafClient client, Exception exception)
        {
            if (exception == null)
            {
                Write(LogLevel.Error, $"{client.Identity} Exception was null.", null);
            }
            else
            {
                Write(LogLevel.Error, $"{client.Identity} {exception}", exception);
            }
        }

        public void Info(ITcpSocket socket, string message)
        {
            Info($"[{socket.Identity}] {message}");
        }

        public void Debug(ITcpSocket socket, string message)
        {
            Debug($"[{socket.Identity}] {message}");
        }

        public void Error(ITcpSocket socket, string message)
        {
            Error($"[{socket.Identity}] {message}");
        }

        public void Exception(ITcpSocket socket, Exception exception)
        {
            if (exception == null)
            {
                Write(LogLevel.Error, $"{socket.Identity} Exception was null.", null);
            }
            else
            {
                Write(LogLevel.Error, $"{socket.Identity} {exception}", exception);
            }
        }

        public void Packet(ITcpSocket socket, BafPacket packet)
        {
            Write(LogLevel.Info, $"{socket.Identity}{Environment.NewLine}{packet.AsString()}", packet);
        }

        public void Packet(BafClient client, BafPacket packet)
        {
            Write(LogLevel.Info, $"{client.Identity}{Environment.NewLine}{packet.AsString()}", packet);
        }

        public void Data(ITcpSocket socket, byte[] data, string message = "Data")
        {
            Write(LogLevel.Info, $"{socket.Identity} {message}{Environment.NewLine}{Util.HexDump(data)}", data);
        }

        public void Data(BafClient client, byte[] data, string message = "Data")
        {
            Write(LogLevel.Info, $"{client.Identity} {message}{Environment.NewLine}{Util.HexDump(data)}", data);
        }
    }
}