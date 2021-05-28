using System.IO;
using System.Net;
using Arrowgene.Baf.Server.PacketHandle;
using Arrowgene.Baf.Server.Scripting;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf.Server.Core
{
    public class BafServer
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(BafServer));

        private readonly AsyncEventServer _server;
        private readonly BafQueueConsumer _consumer;
        private readonly BafSetting _setting;
        private readonly BafScriptEngine _scriptEngine;

        public BafServer(BafSetting setting)
        {
            _setting = new BafSetting(setting);
            _consumer = new BafQueueConsumer(_setting.ServerSetting);
            _scriptEngine = new BafScriptEngine();

            _consumer.AddHandler(new UnknownHandle(this));
            _consumer.AddHandler(new InitialHandle(this));
            _consumer.AddHandler(new LoginHandle(this));
            _consumer.AddHandler(new ChannelListHandle(this));
            _consumer.AddHandler(new JoinChannelHandle(this));
            _consumer.AddHandler(new CreateRoomHandle(this));
            _consumer.AddHandler(new ChannelChatHandle(this));
            _consumer.AddHandler(new ProfileHandle(this));
            _consumer.AddHandler(new JoinRoomHandle(this));
            _consumer.AddHandler(new LobbyProfileHandle(this));
            _consumer.AddHandler(new RoomListHandle(this));

            _server = new AsyncEventServer(
                IPAddress.Any,
                3232,
                _consumer,
                _setting.ServerSetting
            );
        }

        public void ReLoadHandler(DirectoryInfo directoryInfo)
        {
            _scriptEngine.ReLoadHandler(directoryInfo, _consumer, this);
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}