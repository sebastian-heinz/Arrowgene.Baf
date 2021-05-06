using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf.Server.Core
{
    public class BafSetting
    {
        public BafSetting()
        {
            ServerSetting = new AsyncEventSettings();
        }
        
        public BafSetting(BafSetting setting)
        {
            ServerSetting = new AsyncEventSettings(setting.ServerSetting);
        }
        
       public AsyncEventSettings ServerSetting { get; set; }
    }
}