using System;
using Arrowgene.Baf.Server.Asset;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Logging;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;

namespace Arrowgene.Baf
{
    class Program
    {
        private static readonly object ConsoleLock = new();
        private static readonly BafSetting Setting = new();

        static void Main(string[] args)
        {
            LogProvider.Configure<BafLogger>(Setting);
            LogProvider.OnLogWrite += LogProviderOnOnLogWrite;
            LogProvider.Start();

            if (args.Length == 0)
            {
                BafServer server = new BafServer(Setting);
                server.Start();
                Console.ReadKey();
                server.Stop();
            }

            if (args.Length == 1)
            {
                DataArchive archive = new DataArchive();
                archive.Load(args[0]);
            }

            if (args.Length == 2)
            {
                DataArchive archive = new DataArchive();
                archive.Load(args[0]);
                archive.ExtractAll(args[1]);
            }
        }

        private static void LogProviderOnOnLogWrite(object sender, LogWriteEventArgs e)
        {
            ConsoleColor consoleColor = ConsoleColor.Gray;
            switch (e.Log.LogLevel)
            {
                case LogLevel.Debug:
                    consoleColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    consoleColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Error:
                    consoleColor = ConsoleColor.Red;
                    break;
            }

            if (e.Log.Tag is BafPacket packet)
            {
                switch (packet.Source)
                {
                    case PacketSource.Server:
                        consoleColor = ConsoleColor.Green;
                        break;
                    case PacketSource.Client:
                        consoleColor = ConsoleColor.Magenta;
                        break;
                }
            }

            lock (ConsoleLock)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(e.Log);
                Console.ResetColor();
            }
        }
    }
}