using System;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Logging;

namespace Arrowgene.Baf
{
    class Program
    {
        private static readonly object ConsoleLock = new();
        private static readonly BafSetting Setting = new();

        static void Main(string[] args)
        {
            LogProvider.OnLogWrite += LogProviderOnOnLogWrite;
            LogProvider.Start();
            BafServer server = new BafServer(Setting);
            server.Start();
            Console.ReadKey();
            server.Stop();
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

          // if (e.Log.Tag is LogPacketType)
          // {
          //     switch (logWriteEventArgs.Log.Tag)
          //     {
          //         case EzLogPacketType.In:
          //             consoleColor = ConsoleColor.Green;
          //             break;
          //         case EzLogPacketType.Out:
          //             consoleColor = ConsoleColor.Magenta;
          //             break;
          //         case EzLogPacketType.Unhandled:
          //             consoleColor = ConsoleColor.Red;
          //             break;
          //     }
          // }

            lock (ConsoleLock)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(e.Log);
                Console.ResetColor();
            }
        }
    }
}