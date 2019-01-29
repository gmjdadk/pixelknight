using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("Ship")]
    [Serializable]
    public class PSShip : PSObject
    {
        [XmlArray("Lifts")]
        [XmlArrayItem("Lift")]
        public List<PSLift> Lifts = new List<PSLift>();
        public const string ALL_SHIPS_CACHE = "allShips";
        [NonSerialized]
        private DateTime _updateDate;
        [NonSerialized]
        private DateTime _statusStartDate;
        [NonSerialized]
        private DateTime _upgradeStartDate;
        [NonSerialized]
        private DateTime _immunityDate;
        [NonSerialized]
        private PSItem _mineralPsItem;
        [NonSerialized]
        private PSItem _gasPsItem;
        [NonSerialized]
        private ShipDesign _upgradeShipDesign;
        [NonSerialized]
        private ShipDesign _shipDesign;
        [NonSerialized]
        private SpriteDesign _allianceSprite;
        [NonSerialized]
        private string _allianceName;
        [NonSerialized]
        private string _shipName;
        [NonSerialized]
        private double _initHP;
        [XmlIgnore]
        [NonSerialized]
        public int[,] mask;
        [XmlIgnore]
        [NonSerialized]
        public int rows;
        [XmlIgnore]
        [NonSerialized]
        public int columns;

        [XmlIgnore]
        public int TotalRoomsUnderConstruction
        {
            get
            {
                int num = 0;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomStatus == RoomStatus.Building || room.RoomStatus == RoomStatus.Upgrading)
                        ++num;
                }
                return num;
            }
        }

        [XmlIgnore]
        public virtual List<PSItem> ItemsBelongingToShip
        {
            get
            {
                List<PSItem> psItemList = new List<PSItem>();
                if (this.Items != null)
                {
                    foreach (PSItem psItem in ((IEnumerable<PSItem>)this.Items).ToArray<PSItem>())
                    {
                        if (psItem != null && psItem.ShipId == this.ShipId)
                            psItemList.Add(psItem);
                    }
                }
                return psItemList;
            }
        }

        [XmlIgnore]
        public PSCharacter PlayerPSCharacter
        {
            get
            {
                PSCharacter psCharacter = ((IEnumerable<PSCharacter>)this.Characters).First<PSCharacter>();
                for (int index = 0; index < this.Characters.Length; ++index)
                {
                    PSCharacter character = this.Characters[index];
                    if (character.IsOwnedByShip && character.IsCaptain)
                    {
                        psCharacter = character;
                        break;
                    }
                }
                return psCharacter;
            }
        }

        [XmlIgnore]
        public SpriteDesign AllianceSprite
        {
            get
            {
                if (this._allianceSprite == null || this.AllianceSpriteId != this._allianceSprite.SpriteId)
                    this._allianceSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.AllianceSpriteId);
                return this._allianceSprite;
            }
        }

        [XmlIgnore]
        public ShipDesign ShipDesign
        {
            get
            {
                if (this._shipDesign == null || this.ShipDesignId != this._shipDesign.ShipDesignId)
                    this._shipDesign = SingletonManager<ShipManager>.Instance.GetShipDesignById(this.ShipDesignId);
                return this._shipDesign;
            }
        }

        [XmlIgnore]
        public ShipDesign UpgradeShipDesign
        {
            get
            {
                if (this._upgradeShipDesign == null || this.UpgradeShipDesignId != this._upgradeShipDesign.ShipDesignId)
                    this._upgradeShipDesign = SingletonManager<ShipManager>.Instance.GetShipDesignById(this.UpgradeShipDesignId);
                return this._upgradeShipDesign;
            }
        }

        [XmlIgnore]
        public double UpgradeTimeRemaining
        {
            get
            {
                return (double)(this.UpgradeShipDesign.UpgradeTime / (this.ShipDesign.ShipLevel != this.UpgradeShipDesign.ShipLevel ? 1 : 10)) - Singleton<SharedManager>.Instance.CurrentTime(false).Subtract(this.UpgradeStartDate).TotalSeconds;
            }
        }

        [XmlIgnore]
        public PSItem MineralPsItem
        {
            get
            {
                if (this._mineralPsItem == null)
                {
                    List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
                    for (int index = 0; index < itemsBelongingToShip.Count; ++index)
                    {
                        PSItem psItem = itemsBelongingToShip[index];
                        if (psItem.ShipId == this.ShipId && psItem.ItemDesign.ItemType == ItemType.Mineral)
                        {
                            this._mineralPsItem = psItem;
                            break;
                        }
                    }
                }
                return this._mineralPsItem;
            }
        }

        [XmlIgnore]
        public PSItem GasPsItem
        {
            get
            {
                if (this._gasPsItem == null)
                {
                    List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
                    for (int index = 0; index < itemsBelongingToShip.Count; ++index)
                    {
                        PSItem psItem = itemsBelongingToShip[index];
                        if (psItem.ShipId == this.ShipId && psItem.ItemDesign.ItemType == ItemType.Gas)
                        {
                            this._gasPsItem = psItem;
                            break;
                        }
                    }
                }
                return this._gasPsItem;
            }
        }

        [XmlIgnore]
        public int TotalMinerals
        {
            get
            {
                return this.MineralPsItem.Quantity;
            }
            set
            {
                this.MineralPsItem.Quantity = value;
            }
        }

        [XmlIgnore]
        public int TotalGas
        {
            get
            {
                if (this.GasPsItem != null)
                    return this.GasPsItem.Quantity;
                return 0;
            }
            set
            {
                if (this.GasPsItem == null)
                    return;
                this.GasPsItem.Quantity = value;
            }
        }

        [XmlIgnore]
        public int TotalItems
        {
            get
            {
                int num = 0;
                List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
                for (int index = 0; index < itemsBelongingToShip.Count; ++index)
                {
                    PSItem psItem = itemsBelongingToShip[index];
                    if (psItem.ShipId == this.ShipId && psItem.ItemDesign.ItemType == ItemType.Equipment)
                        num += psItem.Quantity;
                }
                return num;
            }
        }

        [XmlIgnore]
        public int TotalCrewInShip
        {
            get
            {
                int num = 0;
                for (int index = 0; index < this.Characters.Length; ++index)
                {
                    PSCharacter character = this.Characters[index];
                    if (character.RoomId != 0 && (character.DeploymentExpired && !character.IsDonated || this.ShipDesign.ShipType == ShipType.Alliance && character.IsDonated && !character.DeploymentExpired))
                        ++num;
                }
                return num;
            }
        }

        [XmlIgnore]
        public int TotalCrew
        {
            get
            {
                int num = 0;
                for (int index = 0; index < this.Characters.Length; ++index)
                {
                    if (this.Characters[index].OwnerShipId == this.ShipId)
                        ++num;
                }
                return num;
            }
        }

        [XmlIgnore]
        public int MineralCapacity
        {
            get
            {
                int mineralCapacity = this.ShipDesign.MineralCapacity;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && (room.RoomType == RoomType.Storage && room.RoomDesign.ManufactureType == ItemType.Mineral))
                        mineralCapacity += room.RoomDesign.Capacity;
                }
                return mineralCapacity;
            }
        }

        [XmlIgnore]
        public int GasCapacity
        {
            get
            {
                int gasCapacity = this.ShipDesign.GasCapacity;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && (room.RoomType == RoomType.Storage && room.RoomDesign.ManufactureType == ItemType.Gas))
                        gasCapacity += room.RoomDesign.Capacity;
                }
                return gasCapacity;
            }
        }

        [XmlIgnore]
        public int ItemCapacity
        {
            get
            {
                int equipmentCapacity = this.ShipDesign.EquipmentCapacity;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && (room.RoomType == RoomType.Storage && room.RoomDesign.ManufactureType == ItemType.Equipment))
                        equipmentCapacity += room.RoomDesign.Capacity;
                }
                return equipmentCapacity;
            }
        }

        [XmlIgnore]
        public int CrewCapacity
        {
            get
            {
                int num = 0;
                RoomType roomType = this.ShipDesign.ShipType != ShipType.Alliance ? RoomType.Bedroom : RoomType.Council;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && room.RoomType == roomType)
                        num += room.RoomDesign.Capacity;
                }
                return num;
            }
        }

        [XmlAttribute]
        public int ShipId { get; set; }

        [XmlAttribute]
        public int ShipDesignId { get; set; }

        [XmlAttribute]
        public double Hp { get; set; }

        [XmlIgnore]
        public double ConvertedHp { get; set; }

        [XmlIgnore]
        public ShipStatus ShipStatus { get; set; }

        [XmlAttribute("ShipStatus")]
        public string ShipStatusString
        {
            get
            {
                return this.ShipStatus.ToString();
            }
            set
            {
                this.ShipStatus = !Enum.IsDefined(typeof(ShipStatus), (object)value) ? ShipStatus.Online : (ShipStatus)Enum.Parse(typeof(ShipStatus), value);
            }
        }

        [XmlAttribute]
        public int StandardCharacterDraws { get; set; }

        [XmlAttribute]
        public int UniqueCharacterDraws { get; set; }

        [XmlAttribute]
        public float BrightnessValue { get; set; }

        [XmlAttribute]
        public float SaturationValue { get; set; }

        [XmlAttribute]
        public float HueValue { get; set; }

        [XmlIgnore]
        public DateTime UpdateDate
        {
            get
            {
                return this._updateDate;
            }
            set
            {
                this._updateDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("UpdateDate")]
        public string UpdateDateString
        {
            get
            {
                if (this.UpdateDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.UpdateDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.UpdateDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime StatusStartDate
        {
            get
            {
                return this._statusStartDate;
            }
            set
            {
                this._statusStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("StatusStartDate")]
        public string StatusStartDateString
        {
            get
            {
                if (this.StatusStartDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.StatusStartDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.StatusStartDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime UpgradeStartDate
        {
            get
            {
                return this._upgradeStartDate;
            }
            set
            {
                this._upgradeStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("UpgradeStartDate")]
        public string UpgradeStartDateString
        {
            get
            {
                if (this.UpgradeStartDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.UpgradeStartDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.UpgradeStartDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public int UserId { get; set; }

        [XmlAttribute]
        public int AllianceId { get; set; }

        [XmlAttribute]
        public string AllianceName
        {
            get
            {
                return this._allianceName;
            }
            set
            {
                this._allianceName = Singleton<SharedManager>.Instance.StripUnsupportedUnicodeChars(value);
            }
        }

        [XmlAttribute]
        public int AllianceSpriteId { get; set; }

        [XmlAttribute]
        public int OriginalRaceId { get; set; }

        [XmlAttribute]
        public int UpgradeShipDesignId { get; set; }

        [XmlIgnore]
        public double ImmunityTimeRemaining
        {
            get
            {
                return (this.ImmunityDate - Singleton<SharedManager>.Instance.CurrentTime(false)).TotalSeconds;
            }
        }

        [XmlIgnore]
        public DateTime ImmunityDate
        {
            get
            {
                return this._immunityDate;
            }
            set
            {
                this._immunityDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("ImmunityDate")]
        public string ImmunityDateString
        {
            get
            {
                if (this.ImmunityDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.ImmunityDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.ImmunityDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public string ShipName
        {
            get
            {
                return this._shipName;
            }
            set
            {
                this._shipName = Singleton<SharedManager>.Instance.StripUnsupportedUnicodeChars(value);
            }
        }

        [XmlAttribute]
        public double Shield { get; set; }

        [XmlAttribute]
        public double TopLeftX { get; set; }

        [XmlAttribute]
        public double TopLeftY { get; set; }

        [XmlAttribute]
        public double CenterX { get; set; }

        [XmlAttribute]
        public double CenterY { get; set; }

        [XmlIgnore]
        public ItemDesign SkinItemDesign
        {
            get
            {
                return SingletonManager<ItemManager>.Instance.GetItemDesignByID(this.SkinItemDesignId);
            }
        }

        [XmlAttribute]
        public int SkinItemDesignId { get; set; }

        [XmlIgnore]
        public List<PSSticker> UsedStickers { get; set; }

        [XmlAttribute]
        public string StickerString
        {
            get
            {
                string str = string.Empty;
                if (this.UsedStickers != null)
                {
                    for (int index = 0; index < this.UsedStickers.Count; ++index)
                    {
                        PSSticker usedSticker = this.UsedStickers[index];
                        str = str + (!(str == string.Empty) ? "|" : string.Empty) + string.Format("{0}@{1}-{2}-{3}", (object)usedSticker.itemDesignId, (object)usedSticker.xPos, (object)usedSticker.yPos, (object)usedSticker.Scale);
                    }
                }
                return str;
            }
            set
            {
                this.UsedStickers = new List<PSSticker>();
                List<string> stringList1;
                if (!string.IsNullOrWhiteSpace(value))
                    stringList1 = ((IEnumerable<string>)value.Replace(" ", string.Empty).Split('|')).ToList<string>();
                else
                    stringList1 = new List<string>();
                List<string> stringList2 = stringList1;
                for (int index = 0; index < stringList2.Count; ++index)
                {
                    string str = stringList2[index];
                    PSSticker psSticker = new PSSticker();
                    string[] array1 = ((IEnumerable<string>)str.Replace("@-", "@|").Split('@')).ToArray<string>();
                    psSticker.itemDesignId = int.Parse(array1[0]);
                    string[] array2 = ((IEnumerable<string>)array1[1].Replace("--", "-|").Split('-')).ToArray<string>();
                    psSticker.xPos = float.Parse(array2[0].Replace("|", "-"));
                    psSticker.yPos = float.Parse(array2[1].Replace("|", "-"));
                    psSticker.Scale = array2.Length >= 3 ? float.Parse(array2[2].Replace("|", "-")) : 1f;
                    this.UsedStickers.Add(psSticker);
                }
            }
        }

        [XmlIgnore]
        public virtual PSRoom[] Rooms { get; }

        [XmlIgnore]
        public virtual PSCharacter[] Characters { get; }

        [XmlIgnore]
        public virtual PSItem[] Items { get; }

        [XmlAttribute]
        public int PowerScore { get; set; }

        [XmlIgnore]
        public List<PSCharacter> CharactersInInventory
        {
            get
            {
                List<PSCharacter> psCharacterList = new List<PSCharacter>();
                for (int index = 0; index < this.Characters.Length; ++index)
                {
                    PSCharacter character = this.Characters[index];
                    if (character.RoomId == 0)
                        psCharacterList.Add(character);
                }
                return psCharacterList;
            }
        }

        public virtual void MapFrom(PSShip psShip)
        {
            this.Lifts = psShip.Lifts;
            this.ShipDesignId = psShip.ShipDesignId;
            this.Hp = psShip.Hp;
            this.ShipStatusString = psShip.ShipStatusString;
            this.StandardCharacterDraws = psShip.StandardCharacterDraws;
            this.UniqueCharacterDraws = psShip.UniqueCharacterDraws;
            this.BrightnessValue = psShip.BrightnessValue;
            this.SaturationValue = psShip.SaturationValue;
            this.HueValue = psShip.HueValue;
            this.UpdateDateString = psShip.UpdateDateString;
            this.StatusStartDateString = psShip.StatusStartDateString;
            this.UpgradeStartDateString = psShip.UpgradeStartDateString;
            this.UserId = psShip.UserId;
            this.AllianceId = psShip.AllianceId;
            this.AllianceName = psShip.AllianceName;
            this.AllianceSpriteId = psShip.AllianceSpriteId;
            this.OriginalRaceId = psShip.OriginalRaceId;
            this.UpgradeShipDesignId = psShip.UpgradeShipDesignId;
            this.ImmunityDateString = psShip.ImmunityDateString;
            this.ShipName = psShip.ShipName;
            this.Shield = psShip.Shield;
            this.TopLeftX = psShip.TopLeftX;
            this.TopLeftY = psShip.TopLeftY;
            this.CenterX = psShip.CenterX;
            this.CenterY = psShip.CenterY;
            this.SkinItemDesignId = psShip.SkinItemDesignId;
            this.StickerString = psShip.StickerString;
            this.PowerScore = psShip.PowerScore;
        }

        public void ArrangeRoomActions()
        {
            for (int index = 0; index < this.Rooms.Length; ++index)
                this.Rooms[index].ArrangeActions();
        }

        public PSItem GetItemByKey(string itemDesignKey)
        {
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemDesignKey == itemDesignKey)
                    return psItem;
            }
            return (PSItem)null;
        }

        public virtual PSItem GetItemByItemDesignId(int itemDesignId)
        {
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemDesignId == itemDesignId)
                    return psItem;
            }
            return (PSItem)null;
        }

        public virtual PSItem GetItemByItemId(int itemId)
        {
            if (this.Items != null)
            {
                for (int index = 0; index < this.Items.Length; ++index)
                {
                    PSItem psItem = this.Items[index];
                    if (psItem.ItemId == itemId)
                        return psItem;
                }
            }
            return (PSItem)null;
        }

        public virtual PSItem AddItemToShip(PSItem item)
        {
            PSItem psItem = Array.Find<PSItem>(this.Items, (Predicate<PSItem>)(x => x.ItemId == item.ItemId));
            if (psItem != null)
            {
                psItem.ItemDesignId = item.ItemDesignId;
                psItem.Quantity = item.Quantity;
                psItem.ShipId = item.ShipId;
            }
            else
            {
                this.ReplaceParentItem(item);
                Debug.LogError((object)"Trying to get base Ship class to add an item! Not supported!", (Object)null);
            }
            return psItem;
        }

        public virtual void ReplaceParentItem(PSItem item)
        {
            for (int index = 0; index < this.Items.Length; ++index)
            {
                PSItem psItem = this.Items[index];
                if (psItem != null && psItem.ShipId == item.ShipId && item.ItemDesign.ParentItemDesignId == psItem.ItemDesignId)
                {
                    this.Items[index] = item;
                    break;
                }
            }
        }

        public virtual void DeleteParentItem(PSItem item)
        {
            for (int index = 0; index < this.Items.Length; ++index)
            {
                PSItem psItem = this.Items[index];
                if (psItem != null && psItem.ShipId == item.ShipId && item.ItemDesign.ParentItemDesignId == psItem.ItemDesignId)
                {
                    this.Items[index] = (PSItem)null;
                    break;
                }
            }
        }

        public List<PSItem> GetAllItemsByType(ItemType type)
        {
            List<PSItem> psItemList = new List<PSItem>();
            for (int index = 0; index < this.ItemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = this.ItemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemType == type)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public List<PSItem> GetAvailableItemsByTypeAndRank(ItemType type, int rank)
        {
            List<PSItem> psItemList = new List<PSItem>();
            for (int index = 0; index < this.ItemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = this.ItemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemType == type && psItem.Quantity > 0 && psItem.ItemDesign.Rank == rank)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public List<PSItem> GetAllItemsBySubType(ItemSubType subType)
        {
            List<PSItem> psItemList = new List<PSItem>();
            for (int index = 0; index < this.ItemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = this.ItemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemSubType == subType)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public PSItem GetMissileItemByMissileDesignId(int missileDesignId)
        {
            for (int index = 0; index < this.ItemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = this.ItemsBelongingToShip[index];
                if (psItem.ItemDesign.MissileDesignId == missileDesignId)
                    return psItem;
            }
            return (PSItem)null;
        }

        public PSItem GetCraftItemByCraftDesignId(int craftDesignId)
        {
            for (int index = 0; index < this.ItemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = this.ItemsBelongingToShip[index];
                if (psItem.ItemDesign.CraftDesignId == craftDesignId)
                    return psItem;
            }
            return (PSItem)null;
        }

        public int GetCapacityOfItem(ItemType itemType, int itemRank)
        {
            int num = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                RoomDesign roomDesign = room.RoomDesign;
                if (roomDesign.ManufactureType == itemType && roomDesign.ItemRank == itemRank && (room.RoomStatus == RoomStatus.Normal || room.RoomStatus == RoomStatus.Upgrading))
                    num += roomDesign.Capacity;
            }
            switch (itemType)
            {
                case ItemType.Mineral:
                    num += this.ShipDesign.MineralCapacity;
                    break;
                case ItemType.Gas:
                    num += this.ShipDesign.GasCapacity;
                    break;
            }
            return num;
        }

        public bool HasRoomOfType(RoomType roomType)
        {
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomType == roomType && (room.RoomStatus == RoomStatus.Normal || room.RoomStatus == RoomStatus.Upgrading))
                    return true;
            }
            return false;
        }

        public PSRoom GetRoomByRoomId(int roomId)
        {
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomId == roomId)
                    return room;
            }
            return (PSRoom)null;
        }

        public PSCharacter GetCharacterbyCharacterId(int characterId)
        {
            for (int index = 0; index < this.Characters.Length; ++index)
            {
                PSCharacter character = this.Characters[index];
                if (character.CharacterId == characterId)
                    return character;
            }
            return (PSCharacter)null;
        }

        public bool HasRoom(int roomId)
        {
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                if (this.Rooms[index].RoomId == roomId)
                    return true;
            }
            return false;
        }

        public bool HasCharacter(int characterId)
        {
            for (int index = 0; index < this.Characters.Length; ++index)
            {
                if (this.Characters[index].CharacterId == characterId)
                    return true;
            }
            return false;
        }

        public int GetTotalNumberOfItemsOfTypeEquippedOnCharacters(PSItem psItem)
        {
            int num = 0;
            for (int index = 0; index < this.Characters.Length; ++index)
            {
                PSCharacter character = this.Characters[index];
                if (character.EquippedItems != null && character.EquippedItems.Count > 0)
                    num += character.EquippedItems.Count<PSItem>((Func<PSItem, bool>)(equip => equip.ItemId == psItem.ItemId));
            }
            return num;
        }

        public int GetTotalNumberOfItemsOfTypeEquippedOnRooms(PSItem psItem)
        {
            int num1 = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.ModuleItems != null && room.ModuleItems.Count > 0)
                {
                    int num2 = room.ModuleItems.Count<PSItem>((Func<PSItem, bool>)(mod => mod.ItemId == psItem.ItemId));
                    num1 += num2;
                }
            }
            return num1;
        }

        public int GetNumberOfRoomsOfSameDesignInInventory(int roomDesignId)
        {
            int num = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomStatus == RoomStatus.Inventory && room.RoomDesignId == roomDesignId)
                    ++num;
            }
            return num;
        }

        public List<PSItem> GetAllAvailableItemsByType(ItemType type)
        {
            List<PSItem> psItemList = new List<PSItem>();
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                ItemManagementInterface managementInterface = psItem as ItemManagementInterface;
                if (psItem.ItemDesign.ItemType == type && managementInterface.QuantityAvailable > 0)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public List<PSItem> GetAllExistingItemsBySubType(ItemSubType subType)
        {
            List<PSItem> psItemList = new List<PSItem>();
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemSubType == subType && psItem.Quantity > 0)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public List<PSItem> GetAllAvailableItemsBySubType(ItemSubType subType)
        {
            List<PSItem> psItemList = new List<PSItem>();
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                ItemManagementInterface managementInterface = psItem as ItemManagementInterface;
                if (psItem.ItemDesign.ItemSubType == subType && managementInterface.QuantityAvailable > 0)
                    psItemList.Add(psItem);
            }
            return psItemList;
        }

        public int GetTotalNumberOfStickersOfTypeUsed(int itemDesignId)
        {
            int num = 0;
            for (int index = 0; index < this.UsedStickers.Count; ++index)
            {
                if (this.UsedStickers[index].itemDesignId == itemDesignId)
                    ++num;
            }
            return num;
        }

        public List<PSRoom> GetRoomsByType(RoomType roomType)
        {
            List<PSRoom> psRoomList = new List<PSRoom>();
            foreach (PSRoom room in this.Rooms)
            {
                if (room.RoomType == roomType)
                    psRoomList.Add(room);
            }
            return psRoomList;
        }

        public double InitialHP
        {
            get
            {
                return this._initHP;
            }
            set
            {
                this._initHP = value;
            }
        }

        public double MaxHp
        {
            get
            {
                return this.ShipDesign.Hp;
            }
        }

        public double MaxShield
        {
            get
            {
                double num = 0.0;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomType == RoomType.Shield && room.RoomStatus == RoomStatus.Normal)
                        num += (double)room.RoomDesign.Capacity;
                }
                return num * 100.0;
            }
        }

        public double AvailableShield
        {
            get
            {
                double num = 0.0;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if (room.RoomType == RoomType.Shield && room.RoomStatus == RoomStatus.Normal && room.Hp > 0.0)
                        num += (double)room.RoomDesign.Capacity;
                }
                return num * 100.0;
            }
        }

        public int MaxPower
        {
            get
            {
                int num = 0;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if ((room.RoomStatus == RoomStatus.Normal || room.RoomStatus == RoomStatus.Upgrading) && room.RoomType == RoomType.Reactor)
                        num += (int)Math.Floor(room.MaxHp / 100.0);
                }
                return num;
            }
        }
    }
}