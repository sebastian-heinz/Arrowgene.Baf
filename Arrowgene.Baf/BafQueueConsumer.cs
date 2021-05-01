using System;
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp;
using Arrowgene.Networking.Tcp.Consumer.BlockingQueueConsumption;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf
{
    public class BafQueueConsumer : ThreadedBlockingQueueConsumer
    {
        private static readonly ILogger _Logger = LogProvider.Logger<Logger>(typeof(BafQueueConsumer));

        public BafQueueConsumer(AsyncEventSettings socketSetting, string identity = "ThreadedBlockingQueueConsumer") :
            base(socketSetting, identity)
        {
            test();
        }

        protected override void HandleReceived(ITcpSocket socket, byte[] data)
        {
            _Logger.Debug("HandleReceived");
            
            _Logger.Debug(Environment.NewLine + Util.HexDump(data));
            byte[] packet = BafXor.Xor_1(data);
            _Logger.Debug(Environment.NewLine + Util.HexDump(packet));
            
            IBuffer buffer = new StreamBuffer();
            buffer.WriteUInt16(0);
            buffer.WriteUInt16(0x1770);
            //buffer.WriteBytes(new byte[0x10]);

            ushort size = (ushort)buffer.Size;
            buffer.SetPositionStart();
            buffer.WriteUInt16(size);
            
            

            socket.Send(buffer.GetAllBytes());
        }

        protected override void HandleDisconnected(ITcpSocket socket)
        {
            _Logger.Debug("HandleDisconnected");
        }

        protected override void HandleConnected(ITcpSocket socket)
        {
            _Logger.Debug("HandleConnected");
        }

        private void test()
        {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            uint seed = (uint) unixTime;
            seed = 0x608D3ee7;
            BafPrng rng = new BafPrng(seed);

            byte[] random = new byte[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                random[i] = (byte) rng.Next();
            }

            byte[] packet = BafXor.Xor_1(random);

            _Logger.Debug(Util.HexDump(packet));
        }
        
        
        
        
        
        

        private static byte[] testA =
        {
            0x86, 0xC4, 0x7B, 0x31, 0xF1, 0xB9, 0x36, 0x70, 0x26, 0x86, 0xCD, 0xF6, 0xEC, 0x64, 0xDF, 0xFB, 0x2F, 0x53,
            0xE6, 0x5C, 0x94, 0x76, 0x4A, 0xD4, 0x02, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x2F, 0x53, 0xE6, 0x5C,
            0x94, 0x76, 0x4A, 0xD4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
    }
}