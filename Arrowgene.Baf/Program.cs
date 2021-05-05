using System;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Logging;

namespace Arrowgene.Baf
{
    class Program
    {
        static void Main(string[] args)
        {
            LogProvider.OnLogWrite += LogProviderOnOnLogWrite;
            LogProvider.Start();
            BafServer server = new BafServer();
            server.Start();
            Console.ReadKey();
            server.Stop();
        }

        private static void LogProviderOnOnLogWrite(object? sender, LogWriteEventArgs e)
        {
           Console.WriteLine(e.Log);
        }
    }
}