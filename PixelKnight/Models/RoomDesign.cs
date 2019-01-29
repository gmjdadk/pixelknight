using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;

namespace PixelKnight.Models
{
    [Serializable]
    public class RoomDesign : PSObject
    {
        [NonSerialized]
        private string _priceString = string.Empty;
        public const string ALL_ROOMDESIGNS_CACHE = "allRoomDesigns";
        [NonSerialized]
        private int maxPowerGenerated;
        [NonSerialized]
        private int maxSystemPower;
        [NonSerialized]
        private List<RoomDesign> upgradedRoomDesigns;
        [NonSerialized]
        private MissileDesign _missileDesign;

        [XmlIgnore]
        public MissileDesign MissileDesign => _missileDesign ?? (_missileDesign = SingletonManager<RoomManager>.Instance.GetMissileDesignById(MissileDesignId));

        [XmlIgnore]
        public List<RoomDesign> UpgradedRoomDesigns => upgradedRoomDesigns ?? (upgradedRoomDesigns = SingletonManager<RoomManager>.Instance.GetNextLevelRoomDesigns(RoomDesignId));

        [XmlAttribute]
        public int RoomDesignId { get; set; }

        [XmlAttribute]
        public string RoomName { get; set; }

        [XmlAttribute]
        public string RoomShortName { get; set; }

        [XmlIgnore]
        public RoomType RoomType { get; set; }

        [XmlAttribute("RoomType")]
        public string RoomTypeString
        {
            get => RoomType.ToString();
            set => RoomType = !Enum.IsDefined(typeof(RoomType), value) ? RoomType.None : (RoomType)Enum.Parse(typeof(RoomType), value);
        }

        [XmlAttribute]
        public int UpgradeFromRoomDesignId { get; set; }

        [XmlAttribute]
        public int MineralCost { get; set; }

        [XmlAttribute]
        public int ConstructionTime { get; set; }

        [XmlAttribute]
        public int Capacity { get; set; }

        [XmlAttribute]
        public int ReloadTime { get; set; }

        [XmlAttribute]
        public int ImageSpriteId { get; set; }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int MissileDesignId { get; set; }

        [XmlAttribute]
        public int MaxSystemPower
        {
            get => maxSystemPower;
            set => maxSystemPower = value * 100;
        }

        [XmlAttribute]
        public int RandomImprovements { get; set; }

        [XmlAttribute]
        public int ImprovementAmounts { get; set; }

        [XmlAttribute]
        public int MaxPowerGenerated
        {
            get => maxPowerGenerated;
            set => maxPowerGenerated = value * 100;
        }

        [XmlAttribute]
        public int ManufactureCapacity { get; set; }

        [XmlAttribute]
        public float ManufactureRate { get; set; }

        [XmlIgnore]
        public ItemType ManufactureType { get; set; }

        [XmlAttribute("ManufactureType")]
        public string ManufactureTypeString
        {
            get => ManufactureType.ToString();
            set => ManufactureType = !Enum.IsDefined(typeof(ItemType), value) ? ItemType.None : (ItemType)Enum.Parse(typeof(ItemType), value);
        }

        [XmlAttribute]
        public int GasCost { get; set; }

        [XmlAttribute]
        public int RaceId { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        [XmlIgnore]
        public CategoryType CategoryType { get; set; }

        [XmlAttribute("CategoryType")]
        public string CategoryTypeString
        {
            get => CategoryType.ToString();
            set => CategoryType = !Enum.IsDefined(typeof(CategoryType), value) ? CategoryType.None : (CategoryType)Enum.Parse(typeof(CategoryType), value);
        }

        [XmlAttribute]
        public int ConstructionSpriteId { get; set; }

        [XmlAttribute]
        public int MinShipLevel { get; set; }

        [XmlAttribute]
        public int ItemRank { get; set; }

        [XmlAttribute]
        public bool Rotate { get; set; }

        [XmlAttribute]
        public int Rows { get; set; }

        [XmlAttribute]
        public int Columns { get; set; }

        [XmlAttribute]
        public string RoomDescription { get; set; }

        [XmlAttribute]
        public bool FlipOnEnemyShip { get; set; }

        [XmlAttribute]
        public int RootRoomDesignId { get; set; }

        [XmlAttribute]
        public int RefillUnitCost { get; set; }

        [XmlAttribute]
        public int DefaultDefenceBonus { get; set; }

        [XmlAttribute]
        public string PriceString
        {
            get => _priceString;
            set
            {
                _priceString = value;
                if (_priceString == string.Empty)
                    return;
                var rewardStrings = SharedManager.ParseRewardStrings(_priceString);
                MineralCost = 0;
                GasCost = 0;
                StarbuxCost = 0;
                SupplyCost = 0;
                foreach (var psResourceGroup in rewardStrings)
                {
                    switch (psResourceGroup.resourceType)
                    {
                        case CurrencyType.Gas:
                            GasCost += psResourceGroup.quantity;
                            break;
                        case CurrencyType.Mineral:
                            MineralCost += psResourceGroup.quantity;
                            break;
                        case CurrencyType.Starbux:
                            StarbuxCost += psResourceGroup.quantity;
                            break;
                        case CurrencyType.Supply:
                            SupplyCost += psResourceGroup.quantity;
                            break;
                    }
                }
            }
        }

        [XmlAttribute]
        public int StarbuxCost { get; set; }

        [XmlAttribute]
        public int SupplyCost { get; set; }

        [XmlAttribute]
        public int Flags { get; set; }

        [XmlIgnore]
        public EnhancementType EnhancementType { get; set; }

        [XmlAttribute("EnhancementType")]
        public string EnhancementTypeString
        {
            get => EnhancementType.ToString();
            set => EnhancementType = !Enum.IsDefined(typeof(EnhancementType), value) ? EnhancementType.None : (EnhancementType)Enum.Parse(typeof(EnhancementType), value);
        }

        [XmlAttribute]
        public int CooldownTime { get; set; }

        [XmlAttribute]
        public int SupportedGridTypes { get; set; }
    }
}