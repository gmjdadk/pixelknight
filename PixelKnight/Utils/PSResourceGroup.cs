using System.Collections.Generic;
using System.Linq;
using PixelKnight.Enums;
using PixelStarships;

namespace PixelKnight.Utils
{
    public class PSResourceGroup
    {
        public int quantity;
        public int resourceId;
        public string resourceTypeString;
        public CurrencyType resourceType;
        public string enhancementString;
        public double enhancementValue;

        public void ParseRewardString(string rewardString)
        {
            string[] strArray1 = rewardString.Split(':');
            resourceTypeString = strArray1.First();
            resourceType = RewardItemTypeStringToEnum(resourceTypeString);
            if (strArray1.Last().Contains("("))
            {
                string[] strArray2 = strArray1.Last().Split('(');
                int.TryParse(strArray2.First(), out resourceId);
                int.TryParse(strArray1.Last().Split('x').Last(), out quantity);
                string[] strArray3 = strArray2.Last().Split(')').First().Split(',');
                enhancementString = strArray3.First();
                double.TryParse(strArray3.Last(), out enhancementValue);
            }
            else if (strArray1.Last().Contains('x'))
            {
                string[] strArray2 = strArray1.Last().Split('x');
                int.TryParse(strArray2.First(), out resourceId);
                int.TryParse(strArray2.Last(), out quantity);
            }
            else
            {
                switch (resourceTypeString)
                {
                    case "character":
                    case "room":
                    case "item":
                    case "research":
                        int.TryParse(strArray1.Last(), out resourceId);
                        quantity = 1;
                        break;
                    default:
                        int.TryParse(strArray1.Last(), out quantity);
                        break;
                }
            }
        }

        public string ConvertToStringFormat(bool returnTypeString = true, bool showQuantity = true)
        {
            string str1 = string.Empty;
            if (returnTypeString)
            {
                switch (resourceType)
                {
                    case CurrencyType.Gas:
                        str1 = "gas:";
                        break;
                    case CurrencyType.Mineral:
                        str1 = "mineral:";
                        break;
                    case CurrencyType.Starbux:
                        str1 = "starbux:";
                        break;
                    case CurrencyType.Item:
                        str1 = "item:";
                        break;
                    case CurrencyType.Supply:
                        str1 = "supply:";
                        break;
                    case CurrencyType.Character:
                        str1 = "character:";
                        break;
                    case CurrencyType.Room:
                        str1 = "room:";
                        break;
                    case CurrencyType.Point:
                        str1 = "points:";
                        break;
                    case CurrencyType.Score:
                        str1 = "score:";
                        break;
                    case CurrencyType.Research:
                        str1 = "research:";
                        break;
                    default:
                        return string.Empty;
                }
            }
            string str2;
            if (string.IsNullOrWhiteSpace(enhancementString))
                str2 = resourceId == 0 ? str1 + quantity : (!showQuantity ? string.Format("{0}{1}", str1, resourceId) : string.Format("{0}{1}x{2}", str1, resourceId, quantity));
            else if (showQuantity)
                str2 = string.Format("{0}{1}({2},{3})x{4}", (object)str1, (object)resourceId, (object)enhancementString, (object)enhancementValue, (object)quantity);
            else
                str2 = string.Format("{0}{1}({2},{3})", (object)str1, (object)resourceId, (object)enhancementString, (object)enhancementValue);
            return str2;
        }

        public string GetRewardName()
        {
            //switch (resourceType)
            //{
            //    case CurrencyType.Gas:
            //        return "Gas";
            //    case CurrencyType.Mineral:
            //        return "Minerals";
            //    case CurrencyType.Starbux:
            //        return "Starbux";
            //    case CurrencyType.Item:
            //        return SingletonManager<ItemManager>.Instance.GetItemDesignByID(resourceId).ItemDesignName;
            //    case CurrencyType.Supply:
            //        return "Supplies";
            //    case CurrencyType.Character:
            //        return SingletonManager<CharacterManager>.Instance.GetCharacterDesignByID(resourceId).CharacterDesignName;
            //    case CurrencyType.Room:
            //        return SingletonManager<RoomManager>.Instance.GetRoomDesignByID(resourceId).RoomName;
            //    case CurrencyType.Point:
            //        return "Reward Points";
            //    case CurrencyType.Score:
            //        return "Score";
            //    case CurrencyType.Research:
            //        return SingletonManager<ResearchManager>.Instance.GetResearch(resourceId).ResearchDesign.ResearchName;
            //    default:
            //        return string.Empty;
            //}
            return string.Empty;
        }

        private static CurrencyType RewardItemTypeStringToEnum(string itemType)
        {
            var map = new Dictionary<string, int>(10)
            {
                {"item", 0}, {"starbux", 1}, {"gas", 2}, {"mineral", 3}, {"supply", 4},
                {"character", 5}, {"points", 6}, {"score", 7}, {"room", 8}, {"research", 9}
            };
            if (itemType != null)
            {
                if (map.TryGetValue(itemType, out var num))
                {
                    switch (num)
                    {
                        case 0:
                            return CurrencyType.Item;
                        case 1:
                            return CurrencyType.Starbux;
                        case 2:
                            return CurrencyType.Gas;
                        case 3:
                            return CurrencyType.Mineral;
                        case 4:
                            return CurrencyType.Supply;
                        case 5:
                            return CurrencyType.Character;
                        case 6:
                            return CurrencyType.Point;
                        case 7:
                            return CurrencyType.Score;
                        case 8:
                            return CurrencyType.Room;
                        case 9:
                            return CurrencyType.Research;
                    }
                }
            }
            return CurrencyType.Item;
        }
    }
}
