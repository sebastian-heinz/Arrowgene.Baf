using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.Logging
{
    public class PacketLog : Log
    {
        public PacketLog(LogLevel logLevel, string text, object tag = null, string loggerIdentity = null, string loggerName = null) : base(logLevel, text, tag, loggerIdentity, loggerName)
        {
        }
    }
}