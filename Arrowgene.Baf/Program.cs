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
                return;
            }

            if (args.Length > 1)
            {
                string operation = args[0].ToLower();
                switch (operation)
                {
                    case "extract":
                    {
                        if (args.Length == 2)
                        {
                            DataArchive archive = new DataArchive();
                            archive.Load(args[1]);
                        }
                        else if (args.Length == 3)
                        {
                            DataArchive archive = new DataArchive();
                            archive.Load(args[1]);
                            archive.ExtractAll(args[2]);
                        }

                        break;
                    }
                    case "save":
                    {
                        if (args.Length == 4)
                        {
                            DataArchive archive = new DataArchive();
                            archive.AddFolder(args[1]);
                            archive.Save(args[2],args[3]);
                        }
                        break;
                    }
                }
            }
            
            LogProvider.Stop();
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