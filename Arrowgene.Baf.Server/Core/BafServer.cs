using System.Net;
using Arrowgene.Baf.Server.PacketHandle;
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

        public BafServer(BafSetting setting)
        {
            _setting = new BafSetting(setting);
            _consumer = new BafQueueConsumer(_setting.ServerSetting);
            
            _consumer.AddHandler(new UnknownHandle());
            _consumer.AddHandler(new InitialHandle());
            _consumer.AddHandler(new LoginHandle());
            _consumer.AddHandler(new ChannelListHandle());
            _consumer.AddHandler(new JoinChannelHandle());
            
            _server = new AsyncEventServer(
                IPAddress.Any,
                3232,
                _consumer,
                _setting.ServerSetting
            );
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