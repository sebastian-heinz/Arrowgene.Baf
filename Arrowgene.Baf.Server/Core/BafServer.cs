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
        private readonly AsyncEventSettings _setting;
        private readonly BafQueueConsumer _consumer;

        public BafServer()
        {
            _setting = new AsyncEventSettings();
            _consumer = new BafQueueConsumer(_setting);
            _consumer.AddHandler(new Crypt_F303());
            _consumer.AddHandler(new Login_E803());
            _server = new AsyncEventServer(
                IPAddress.Any,
                3232,
                _consumer,
                _setting
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