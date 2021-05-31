using System.Collections.Generic;
using System.IO;
using System.Net;
using Arrowgene.Baf.Server.Asset;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Baf.Server.PacketHandle;
using Arrowgene.Baf.Server.Scripting;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;

namespace Arrowgene.Baf.Server.Core
{
    public class BafServer
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(BafServer));

        public const short ChannelTabs = 3;
        public const short MaxChannels = 5;

        private readonly AsyncEventServer _server;
        private readonly BafQueueConsumer _consumer;
        private readonly BafSetting _setting;
        private readonly BafScriptEngine _scriptEngine;
        private readonly Channel[][] _channels;
        private readonly Dictionary<int, ShopItem> _shopItems;
        private readonly Dictionary<int, Song> _songs;

        public BafServer(BafSetting setting)
        {
            _setting = new BafSetting(setting);
            _consumer = new BafQueueConsumer(_setting.ServerSetting);
            _scriptEngine = new BafScriptEngine();
            _shopItems = new Dictionary<int, ShopItem>();
            _songs = new Dictionary<int, Song>();
            _channels = new Channel[ChannelTabs][];
            _server = new AsyncEventServer(
                IPAddress.Any,
                3232,
                _consumer,
                _setting.ServerSetting
            );
            Load();
        }

        private void Load()
        {
            string dressXmlPath = Path.Combine(Util.ExecutingDirectory().FullName, "Files/DRESS.XML");
            List<ShopItem> items = DressXml.Parse(dressXmlPath);
            foreach (ShopItem item in items)
            {
                _shopItems.Add(item.Id, item);
            }

            Logger.Info($"Loaded Items: {_shopItems.Count}");

            for (short channelTab = 0; channelTab < ChannelTabs; channelTab++)
            {
                _channels[channelTab] = new Channel[MaxChannels];
                for (short channelNumber = 0; channelNumber < MaxChannels; channelNumber++)
                {
                    // 0 = easy ; 1 = hard; 2 = active channel
                    _channels[channelTab][channelNumber] = new Channel(channelTab, channelNumber,
                        $"Channel {channelTab}-{channelNumber}");
                }
            }

            _consumer.AddHandler(new UnknownHandle(this));
            _consumer.AddHandler(new InitialHandle(this));
            _consumer.AddHandler(new LoginHandle(this));
            _consumer.AddHandler(new ChannelListHandle(this));
            _consumer.AddHandler(new JoinChannelHandle(this));
            _consumer.AddHandler(new CreateRoomHandle(this));
            _consumer.AddHandler(new ChannelChatHandle(this));
            _consumer.AddHandler(new ProfileHandle(this));
            _consumer.AddHandler(new JoinRoomHandle(this));
            _consumer.AddHandler(new LobbyProfileHandle(this));
            _consumer.AddHandler(new RoomListHandle(this));
            _consumer.AddHandler(new RoomChatHandle(this));
            _consumer.AddHandler(new RoomChangeColorHandle(this));
            _consumer.AddHandler(new RoomChangeSettingHandle(this));
            _consumer.AddHandler(new RoomLeaveHandle(this));
            _consumer.AddHandler(new RoomStartSongHandle(this));
            _consumer.AddHandler(new RoomSpectatorHandle(this));
            _consumer.AddHandler(new RoomSelectSongHandle(this));
            _consumer.AddHandler(new RoomSelectModeHandle(this));
            _consumer.AddHandler(new ShopBuyItemHandle(this));
            _consumer.AddHandler(new CreateBandHandle(this));
            _consumer.AddHandler(new Unknown0Handle(this));

            Logger.Info($"Loaded PacketHandler: {_consumer.GetHandlerCount()}");
        }

        public Channel GetChannel(short channelTab, short channelNumber)
        {
            if (channelTab >= ChannelTabs || channelTab < 0)
            {
                Logger.Error($"GetChannel: Invalid ChannelTab: {channelTab} ({channelTab}-{channelNumber})");
                return null;
            }

            if (channelNumber >= MaxChannels || channelNumber < 0)
            {
                Logger.Error($"GetChannel: Invalid ChannelNumber: {channelNumber} ({channelTab}-{channelNumber})");
                return null;
            }

            return _channels[channelTab][channelNumber];
        }

        public ShopItem GetShopItem(int shopItemId)
        {
            if (!_shopItems.ContainsKey(shopItemId))
            {
                return null;
            }
            return _shopItems[shopItemId];
        }

        public List<Channel> GetChannels(short channelTab)
        {
            if (channelTab >= ChannelTabs || channelTab < 0)
            {
                Logger.Error($"GetChannels: Invalid ChannelTab: {channelTab}");
                return null;
            }

            List<Channel> channels = new List<Channel>();
            for (int i = 0; i < MaxChannels; i++)
            {
                Channel channel = _channels[channelTab][i];
                if (channel == null)
                {
                    continue;
                }

                channels.Add(channel);
            }

            return channels;
        }

        public List<Channel> GetChannels()
        {
            List<Channel> channel = new List<Channel>();
            for (short channelTab = 0; channelTab < ChannelTabs; channelTab++)
            {
                channel.AddRange(GetChannels(channelTab));
            }

            return channel;
        }

        public void ReLoadHandler(DirectoryInfo directoryInfo)
        {
            _consumer.ClearHandler();
            _scriptEngine.ReLoadHandler(directoryInfo, _consumer, this);
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