using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Arrowgene.Baf.Server.Common;
using Arrowgene.Baf.Server.Model;
using Arrowgene.Logging;

namespace Arrowgene.Baf.Server.Asset
{
    public static class DressXml
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(DressXml));

        public static List<ShopItem> Parse(string path)
        {
            List<ShopItem> items = new List<ShopItem>();

            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                Logger.Error($"File: {path} does not exist");
                return items;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                string xml = File.ReadAllText(file.FullName, Util.EncodingSimpChinese);
                xmlDoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to read file: {path}");
                Logger.Exception(ex);
            }

            XmlNodeList itemInfo = xmlDoc.GetElementsByTagName("ItemInfo");
            foreach (XmlNode node in itemInfo[0].ChildNodes)
            {
                if (node.Attributes == null)
                {
                    continue;
                }

                if (!int.TryParse(node.Attributes["id"].InnerText, out int itemId))
                {
                    continue;
                }

                ShopItem shopItem = new ShopItem();
                shopItem.Id = itemId;
                shopItem.Name = node.Attributes["name"].InnerText;

                if (!Enum.TryParse(node.Attributes["type"].InnerText, out ItemType type))
                {
                    Logger.Error($"Failed to parse 'type' for itemId: {itemId}");
                    continue;
                }

                shopItem.Type = type;

                if (!Enum.TryParse(node.Attributes["sex"].InnerText, out GenderType gender))
                {
                    Logger.Error($"Failed to parse 'sex' for itemId: {itemId}");
                    continue;
                }

                shopItem.Gender = gender;

                if (!int.TryParse(node.Attributes["new"].InnerText, out int isNew))
                {
                    Logger.Error($"Failed to parse 'new' for itemId: {itemId}");
                    continue;
                }

                shopItem.IsNew = isNew == 1;

                if (!int.TryParse(node.Attributes["hot"].InnerText, out int isHot))
                {
                    Logger.Error($"Failed to parse 'hot' for itemId: {itemId}");
                    continue;
                }

                shopItem.IsHot = isHot == 1;

                if (!int.TryParse(node.Attributes["onlysend"].InnerText, out int onlySend))
                {
                    Logger.Error($"Failed to parse 'onlysend' for itemId: {itemId}");
                    continue;
                }

                shopItem.OnlyGift = onlySend == 1;


                if (!int.TryParse(node.Attributes["onlymarried"].InnerText, out int onlyMarried))
                {
                    Logger.Error($"Failed to parse 'onlymarried' for itemId: {itemId}");
                    continue;
                }

                shopItem.OnlyMarried = onlyMarried == 1;

                if (!int.TryParse(node.Attributes["level"].InnerText, out int level))
                {
                    Logger.Error($"Failed to parse 'level' for itemId: {itemId}");
                    continue;
                }

                shopItem.Level = level;

                shopItem.Model = node.Attributes["model"].InnerText;

                if (node.Attributes["gem30"] == null)
                {
                }
                else
                {
                    if (!int.TryParse(node.Attributes["gem30"].InnerText, out int gem30))
                    {
                        Logger.Error($"Failed to parse 'gem30' for itemId: {itemId}");
                        continue;
                    }

                    shopItem.Gem30 = gem30;
                }

                if (node.Attributes["money7"] == null)
                {
                }
                else
                {
                    if (!int.TryParse(node.Attributes["money7"].InnerText, out int money7))
                    {
                        Logger.Error($"Failed to parse 'money7' for itemId: {itemId}");
                        continue;
                    }

                    shopItem.Money7 = money7;
                }


                if (node.Attributes["money30"] == null)
                {
                }
                else
                {
                    if (!int.TryParse(node.Attributes["money30"].InnerText, out int money30))
                    {
                        Logger.Error($"Failed to parse 'money30' for itemId: {itemId}");
                        continue;
                    }

                    shopItem.Money30 = money30;
                }

                if (node.Attributes["money"] == null)
                {
                }
                else
                {
                    if (!int.TryParse(node.Attributes["money"].InnerText, out int money))
                    {
                        Logger.Error($"Failed to parse 'money' for itemId: {itemId}");
                        continue;
                    }

                    shopItem.Money = money;
                }


                if (type == ItemType.Set)
                {
                    // set item
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Attributes == null)
                        {
                            continue;
                        }

                        string childModel = child.Attributes["model"].InnerText;
                        switch (child.Name)
                        {
                            case "hair":
                            {
                                break;
                            }
                            case "upper":
                            {
                                break;
                            }
                            case "under":
                            {
                                break;
                            }
                            case "hands":
                            {
                                break;
                            }
                            case "shoes":
                            {
                                break;
                            }
                        }

                        // todo assign set models
                    }
                }

                items.Add(shopItem);
            }

            return items;
        }
    }
}