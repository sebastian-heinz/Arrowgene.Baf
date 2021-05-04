using System;
using System.IO;
using System.Security.Cryptography;
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
        }

        protected override void HandleReceived(ITcpSocket socket, byte[] data)
        {
            _Logger.Debug("HandleReceived");

            IBuffer recv = new StreamBuffer(data);
            recv.SetPositionStart();
            ushort sz = recv.ReadUInt16();
            byte[] rcv = recv.ReadBytes(sz - 2);

            _Logger.Debug(Environment.NewLine + Util.HexDump(rcv));
            byte[] packet = BafXor.Xor_1(rcv);
            _Logger.Debug(Environment.NewLine + Util.HexDump(packet));

            IBuffer pBuf = new StreamBuffer(packet);
            pBuf.SetPositionStart();
            byte[] a = pBuf.ReadBytes(16);
            byte[] b = pBuf.ReadBytes(8);
            byte[] c = pBuf.ReadBytes(8);
            byte[] d = pBuf.ReadBytes(8);
            
            using var md5 = MD5.Create();
            byte[] hash = a;
            for (int i = 0; i < 0x10; i++)
            {
                hash = md5.ComputeHash(hash);
            }
            IBuffer phash = new StreamBuffer(hash);
            phash.SetPositionStart();
            byte[] e = phash.ReadBytes(8);
            byte[] f = phash.ReadBytes(8);
            
            byte[] dec = Dec(b, f, e);
            _Logger.Debug(Environment.NewLine + Util.HexDump(dec));
        }

        private byte[] Enc(byte[] input, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cProv = new DESCryptoServiceProvider();


            ICryptoTransform transform = cProv.CreateEncryptor(key, iv);
            MemoryStream inStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(inStream, transform, CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            byte[] ret = inStream.ToArray();
            return ret;
        }

        private byte[] Dec(byte[] input, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cProv = new DESCryptoServiceProvider();
            cProv.Padding = PaddingMode.None;
            cProv.Mode = CipherMode.CBC;
            ICryptoTransform transform = cProv.CreateDecryptor(key, iv);
            MemoryStream inStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(inStream, transform, CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            byte[] ret = inStream.ToArray();
            return ret;
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
            IBuffer pBuf = new StreamBuffer(testA);
            pBuf.SetPositionStart();
            byte[] a = pBuf.ReadBytes(16);
            byte[] b = pBuf.ReadBytes(8);
            byte[] c = pBuf.ReadBytes(8);
            byte[] d = pBuf.ReadBytes(8);
            
            
            using var md5 = MD5.Create();
            byte[] hash = a;
            for (int i = 0; i < 0x10; i++)
            {
                hash = md5.ComputeHash(hash);
            }
            IBuffer phash = new StreamBuffer(hash);
            phash.SetPositionStart();
            byte[] e = phash.ReadBytes(8);
            byte[] f = phash.ReadBytes(8);
            
            byte[] dec = Dec(b, f, e);
            _Logger.Debug(Environment.NewLine + Util.HexDump(dec));
        }


        private void testC()
        {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset) foo).ToUnixTimeSeconds();
            uint seed = (uint) unixTime;
            seed = 0x60913E2B;
            BafPrng rng = new BafPrng(seed);

            byte[] random = new byte[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                random[i] = (byte) rng.Next();
            }

            _Logger.Debug(Util.HexDump(random));

            using var md5 = MD5.Create();
            byte[] hash = random;
            for (int i = 0; i < 0x10; i++)
            {
                hash = md5.ComputeHash(hash);
            }


            _Logger.Debug(Util.HexDump(hash));
        }

        private static byte[] testA =
        {
            0xB2, 0xB6, 0xE1, 0x67, 0xB1, 0x6F, 0xD1, 0x8C, 0xA5, 0xA9, 0x26, 0xC6, 0x8D, 0x73, 0x81, 0xC4,
            0x5F, 0x94, 0x71, 0x6B, 0xB6, 0xB8, 0xFF, 0xA1, 0x02, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00,
            0x5F, 0x94, 0x71, 0x6B, 0xB6, 0xB8, 0xFF, 0xA1
        };
    }
}