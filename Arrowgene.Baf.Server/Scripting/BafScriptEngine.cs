using System.IO;
using System.Reflection;
using Arrowgene.Baf.Server.Core;
using Arrowgene.Baf.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;
using Microsoft.CodeAnalysis;

namespace Arrowgene.Baf.Server.Scripting
{
    /// <summary>
    /// Can initialize classes from .cs files
    /// </summary>
    public class BafScriptEngine
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(BafScriptEngine));

        private readonly ScriptEngine _engine;

        public BafScriptEngine()
        {
            _engine = new ScriptEngine();
            AddReference(Assembly.GetAssembly(typeof(BafServer)));
            AddReference(Assembly.GetAssembly(typeof(AsyncEventServer)));
        }

        public void AddReference(Assembly assembly)
        {
            _engine.AddReference(assembly);
        }

        /// <summary>
        /// Creates a HandlerId instance for each script found inside the directory.
        /// </summary>
        /// <param name="directoryInfo">Script directory</param>
        /// <param name="consumer">Instance of BafQueueConsumer</param>
        public void ReLoadHandler(DirectoryInfo directoryInfo, BafQueueConsumer consumer, BafServer server)
        {
            FileInfo[] scripts = directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);
            foreach (FileInfo script in scripts)
            {
                string code = File.ReadAllText(script.FullName);
                ScriptTask scriptTask = _engine.CreateTask(script.FullName, code);
                IPacketHandler handler = scriptTask.CreateInstance<IPacketHandler>(server);
                if (handler != null)
                {
                    Logger.Info($"Adding Handler: {handler.Id}");
                    consumer.AddHandler(handler, true);
                }
                else if (scriptTask.Diagnostics != null && scriptTask.Diagnostics.Count > 0)
                {
                    foreach (Diagnostic diagnostic in scriptTask.Diagnostics)
                    {
                        Logger.Error($"{scriptTask.Name}: {diagnostic}");
                    }
                }
                else
                {
                    Logger.Error($"Failed to load script ({scriptTask.Name})");
                }
            }
        }
    }
}