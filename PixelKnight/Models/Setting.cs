using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelKnight.Utils;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class Setting : PSObject
    {
        private DateTime _saleStartDate;
        private DateTime _saleEndDate;
        private DateTime _newsUpdateDate;
        private DateTime _replayAvailableDate;
        private DateTime _limitedCatalogExpiryDate;
        private DateTime _supportTaskRanDate;
        private string _allianceBadgeSpriteIdString;

        [XmlAttribute]
        public int SettingId { get; set; }

        [XmlAttribute]
        public int ServerSettingVersion { get; set; }

        [XmlAttribute]
        public decimal MinimumClientVersion { get; set; }

        [XmlAttribute]
        public decimal CurrentAndroidVersion { get; set; }

        [XmlAttribute]
        public string News { get; set; }

        [XmlAttribute]
        public string MaintenanceMessage { get; set; }

        [XmlIgnore]
        public List<int> AllianceBadgeSpriteIds { get; set; }

        [XmlIgnore]
        public DailyRewardType DailyRewardType { get; set; }

        [XmlAttribute("DailyRewardType")]
        public string DailyRewardTypeString
        {
            get => DailyRewardType.ToString();
            set => DailyRewardType = !Enum.IsDefined(typeof(DailyRewardType), value) ? DailyRewardType.Starbux : (DailyRewardType)Enum.Parse(typeof(DailyRewardType), value);
        }

        [XmlAttribute]
        public int DailyRewardArgument { get; set; }

        [XmlIgnore]
        public SaleType SaleType { get; set; }

        [XmlAttribute("SaleType")]
        public string SaleTypeString
        {
            get => SaleType.ToString();
            set => SaleType = !Enum.IsDefined(typeof(SaleType), value) ? SaleType.None : (SaleType)Enum.Parse(typeof(SaleType), value);
        }

        [XmlAttribute]
        public int SaleArgument { get; set; }

        [XmlAttribute]
        public string SaleTitle { get; set; }

        [XmlAttribute]
        public int SaleItemMask { get; set; }

        [XmlAttribute]
        public bool SaleOnceOnly { get; set; }

        [XmlAttribute]
        public int FileVersion { get; set; }

        [XmlAttribute]
        public int SpriteVersion { get; set; }

        [XmlAttribute]
        public int CharacterDesignVersion { get; set; }

        [XmlAttribute]
        public int CharacterPartVersion { get; set; }

        [XmlAttribute]
        public int AnimationVersion { get; set; }

        [XmlAttribute]
        public int RoomDesignVersion { get; set; }

        [XmlAttribute]
        public int MissileDesignVersion { get; set; }

        [XmlAttribute]
        public int ResearchDesignVersion { get; set; }

        [XmlAttribute]
        public int TrainingDesignVersion { get; set; }

        [XmlAttribute]
        public int RoomDesignSpriteVersion { get; set; }

        [XmlAttribute]
        public int AchievementDesignVersion { get; set; }

        [XmlAttribute]
        public int ConditionTypeVersion { get; set; }

        [XmlAttribute]
        public int CraftDesignVersion { get; set; }

        [XmlAttribute]
        public int ItemDesignVersion { get; set; }

        [XmlAttribute]
        public int ChallengeDesignVersion { get; set; }

        [XmlAttribute]
        public int ActionTypeVersion { get; set; }

        [XmlAttribute]
        public int RoomDesignPurchaseVersion { get; set; }

        [XmlAttribute]
        public int ShipDesignVersion { get; set; }

        [XmlAttribute]
        public int MissionDesignVersion { get; set; }

        [XmlAttribute]
        public int BackgroundVersion { get; set; }

        [XmlAttribute]
        public int LeagueVersion { get; set; }

        [XmlAttribute]
        public int RewardDesignVersion { get; set; }

        [XmlAttribute]
        public int CollectionDesignVersion { get; set; }

        [XmlAttribute]
        public string DailyItemRewards { get; set; }

        [XmlAttribute]
        public int MaxDailyDraws { get; set; }

        [XmlAttribute]
        public int PromotionDesignVersion { get; set; }

        [XmlAttribute]
        public DateTime SaleStartDate
        {
            get => _saleStartDate;
            set => _saleStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute]
        public DateTime SaleEndDate
        {
            get => _saleEndDate;
            set => _saleEndDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute]
        public int NewsSpriteId { get; set; }

        [XmlAttribute]
        public DateTime NewsUpdateDate
        {
            get => _newsUpdateDate;
            set => _newsUpdateDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute]
        public DateTime ReplayAvailableDate
        {
            get => _replayAvailableDate;
            set => _replayAvailableDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute]
        public int CommonCrewId { get; set; }

        [XmlAttribute("BackgroundId")]
        public int BackgroundDesignId { get; set; }

        [XmlAttribute]
        public int HeroCrewId { get; set; }

        [XmlAttribute]
        public int SaleQuantity { get; set; }

        [XmlIgnore]
        public SaleType LimitedCatalogType { get; set; }

        [XmlAttribute("LimitedCatalogType")]
        public string LimitedCatalogTypeString
        {
            get => LimitedCatalogType.ToString();
            set => LimitedCatalogType = !Enum.IsDefined(typeof(SaleType), value) ? SaleType.None : (SaleType)Enum.Parse(typeof(SaleType), value);
        }

        [XmlAttribute]
        public int LimitedCatalogArgument { get; set; }

        [XmlAttribute]
        public int LimitedCatalogQuantity { get; set; }

        [XmlAttribute]
        public DateTime LimitedCatalogExpiryDate
        {
            get => _limitedCatalogExpiryDate;
            set => _limitedCatalogExpiryDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlIgnore]
        public CurrencyType LimitedCatalogCurrencyType { get; set; }

        [XmlAttribute("LimitedCatalogCurrencyType")]
        public string LimitedCatalogCurrencyTypeString
        {
            get => LimitedCatalogCurrencyType.ToString();
            set => LimitedCatalogCurrencyType = !Enum.IsDefined(typeof(CurrencyType), (object)value) ? CurrencyType.Unknown : (CurrencyType)Enum.Parse(typeof(CurrencyType), value);
        }

        [XmlAttribute]
        public int LimitedCatalogCurrencyAmount { get; set; }

        [XmlAttribute]
        public int LimitedCatalogMaxTotal { get; set; }

        [XmlAttribute]
        public int NewUserCount { get; set; }

        [XmlAttribute]
        public int CharacterDesignActionVersion { get; set; }

        [XmlAttribute]
        public int AndroidBuild { get; set; }

        [XmlAttribute("CargoItems")]
        public string CargoItemsString { get; set; }

        [XmlIgnore]
        public List<PSResourceGroup> CargoItems => CargoItemsString != string.Empty ? SharedManager.ParseRewardStrings(CargoItemsString).ToList() : new List<PSResourceGroup>();

        [XmlAttribute]
        public string CargoPrices { get; set; }

        [XmlAttribute]
        public int TournamentFinalDuration { get; set; }

        [XmlAttribute]
        public string TournamentNews { get; set; }

        [XmlAttribute]
        public int TournamentSpriteId { get; set; }

        [XmlAttribute]
        public int DivisionDesignVersion { get; set; }

        [XmlAttribute]
        public DateTime SupportTaskRanDate
        {
            get => _supportTaskRanDate;
            set => _supportTaskRanDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute]
        public int RewardPointPercentage { get; set; }

        [XmlAttribute]
        public int DrawDesignVersion { get; set; }

        [XmlAttribute]
        public int AbilityDesignVersion { get; set; }

        [XmlAttribute]
        public int VipDesignVersion { get; set; }

        [XmlAttribute]
        public int LimitedCatalogRestockQuantity { get; set; }

        [XmlAttribute]
        public bool IsDebug { get; set; }

        [XmlAttribute]
        public int RewardVideoTimeReduction { get; set; }
    }
}
