using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelKnight.Utils;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class ItemDesign : PSObject
    {
        [NonSerialized]
        private List<PSResourceGroup> _ingredients = new List<PSResourceGroup>();
        [NonSerialized]
        private string _manufactureCostString = string.Empty;
        public const string ALL_ITEMDESIGNS_CACHE = "allItemDesigns";
        [NonSerialized]
        private SpriteDesign _sprite;
        [NonSerialized]
        private SpriteDesign _logoSprite;
        [NonSerialized]
        private SpriteDesign _borderSprite;
        [NonSerialized]
        private CraftDesign _craftDesign;
        [NonSerialized]
        private MissileDesign _missileDesign;
        [NonSerialized]
        private CharacterDesign _characterDesign;
        [NonSerialized]
        private ItemDesign _parentDesign;
        [NonSerialized]
        private TrainingDesign _trainingDesign;
        [NonSerialized]
        private PSResearch _requiredPsResearch;
        [NonSerialized]
        private int _level;

        [XmlIgnore]
        public bool CanUpgrade
        {
            get
            {
                foreach (ItemDesign itemDesign in SingletonManager<ItemManager>.Instance.itemDesignList.ToArray())
                {
                    foreach (PSResourceGroup psResourceGroup in itemDesign.Ingredients.ToArray())
                    {
                        if (this.ItemDesignId == psResourceGroup.resourceId)
                            return true;
                    }
                }
                return false;
            }
        }

        [XmlIgnore]
        public bool IsGear
        {
            get
            {
                if (this.ItemSubType < ItemSubType.GasPack)
                    return this.ItemSubType != ItemSubType.None;
                return false;
            }
        }

        [XmlIgnore]
        public PSResearch RequiredPsResearch
        {
            get
            {
                if (this._requiredPsResearch == null)
                    this._requiredPsResearch = SingletonManager<ResearchManager>.Instance.GetResearch(this.RequiredResearchDesignId);
                return this._requiredPsResearch;
            }
        }

        [XmlIgnore]
        public int Level
        {
            get
            {
                if (this._level == 0)
                {
                    ItemDesign parentDesign = this.ParentDesign;
                    ++this._level;
                    while (parentDesign != null)
                    {
                        parentDesign = parentDesign.ParentDesign;
                        ++this._level;
                    }
                }
                return this._level;
            }
        }

        [XmlIgnore]
        public ItemDesign ParentDesign
        {
            get
            {
                if (this._parentDesign == null)
                    this._parentDesign = SingletonManager<ItemManager>.Instance.GetItemDesignByID(this.ParentItemDesignId);
                return this._parentDesign;
            }
        }

        [XmlIgnore]
        public MissileDesign MissileDesign
        {
            get
            {
                if (this._missileDesign == null)
                    this._missileDesign = SingletonManager<RoomManager>.Instance.GetMissileDesignById(this.MissileDesignId);
                return this._missileDesign;
            }
        }

        [XmlIgnore]
        public CraftDesign CraftDesign
        {
            get
            {
                if (this._craftDesign == null)
                    this._craftDesign = SingletonManager<RoomManager>.Instance.GetCraftDesignById(this.CraftDesignId);
                return this._craftDesign;
            }
        }

        [XmlIgnore]
        public CharacterDesign CharacterDesign
        {
            get
            {
                if (this._characterDesign == null)
                    this._characterDesign = SingletonManager<CharacterManager>.Instance.GetCharacterDesignByID(this.CharacterDesignId);
                return this._characterDesign;
            }
        }

        [XmlIgnore]
        public SpriteDesign ImageSprite
        {
            get
            {
                if (this._sprite == null)
                    this._sprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.ImageSpriteId);
                return this._sprite;
            }
        }

        [XmlIgnore]
        public SpriteDesign LogoSprite
        {
            get
            {
                if (this._logoSprite == null)
                    this._logoSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.LogoSpriteId);
                return this._logoSprite;
            }
        }

        [XmlIgnore]
        public SpriteDesign BorderSprite
        {
            get
            {
                if (this._borderSprite == null)
                    this._borderSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.BorderSpriteId);
                return this._borderSprite;
            }
        }

        [XmlAttribute]
        public int ItemDesignId { get; set; }

        [XmlAttribute]
        public string ItemDesignName { get; set; }

        [XmlAttribute]
        public string ItemDesignKey { get; set; }

        [XmlAttribute]
        public string ItemDesignDescription { get; set; }

        [XmlAttribute]
        public int ImageSpriteId { get; set; }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int BorderSpriteId { get; set; }

        [XmlAttribute]
        public int ItemSpace { get; set; }

        [XmlIgnore]
        public ItemType ItemType { get; set; }

        [XmlAttribute("ItemType")]
        public string ItemTypeString
        {
            get
            {
                return this.ItemType.ToString();
            }
            set
            {
                this.ItemType = !Enum.IsDefined(typeof(ItemType), (object)value) ? ItemType.None : (ItemType)Enum.Parse(typeof(ItemType), value);
            }
        }

        [XmlIgnore]
        public EnhancementType EnhancementType { get; set; }

        [XmlAttribute("EnhancementType")]
        public string EnhancementTypeString
        {
            get
            {
                return this.EnhancementTypeString.ToString();
            }
            set
            {
                this.EnhancementType = !Enum.IsDefined(typeof(EnhancementType), (object)value) ? EnhancementType.None : (EnhancementType)Enum.Parse(typeof(EnhancementType), value);
            }
        }

        [XmlIgnore]
        public ItemSubType ItemSubType { get; set; }

        [XmlAttribute("ItemSubType")]
        public string ItemSubTypeString
        {
            get
            {
                return this.ItemSubType.ToString();
            }
            set
            {
                this.ItemSubType = !Enum.IsDefined(typeof(ItemSubType), (object)value) ? ItemSubType.None : (ItemSubType)Enum.Parse(typeof(ItemSubType), value);
            }
        }

        [XmlAttribute]
        public int GasCost { get; set; }

        [XmlAttribute]
        public int MineralCost { get; set; }

        [XmlAttribute]
        public int Rank { get; set; }

        [XmlAttribute]
        public int MinRoomLevel { get; set; }

        [XmlAttribute]
        public int BuildTime { get; set; }

        [XmlAttribute]
        public int RootItemDesignId { get; set; }

        [XmlAttribute]
        public int RaceId { get; set; }

        [XmlAttribute]
        public int RequiredResearchDesignId { get; set; }

        [XmlAttribute]
        public int ParentItemDesignId { get; set; }

        [XmlAttribute]
        public int CraftDesignId { get; set; }

        [XmlAttribute]
        public int MissileDesignId { get; set; }

        [XmlAttribute]
        public int CharacterPartId { get; set; }

        [XmlIgnore]
        public ModuleType ModuleType { get; set; }

        [XmlAttribute("ModuleType")]
        public string ModuleTypeString
        {
            get
            {
                return this.ModuleType.ToString();
            }
            set
            {
                this.ModuleType = !Enum.IsDefined(typeof(ModuleType), (object)value) ? ModuleType.None : (ModuleType)Enum.Parse(typeof(ModuleType), value);
            }
        }

        [XmlElement("CharacterPart")]
        public PSCharacterPart CharacterPart { get; set; }

        [XmlIgnore]
        public List<PSResourceGroup> Ingredients
        {
            get
            {
                return this._ingredients;
            }
            set
            {
                this._ingredients = value;
            }
        }

        [XmlAttribute("Ingredients")]
        public string IngredientsString
        {
            get
            {
                string str = string.Empty;
                for (int index = 0; index < this._ingredients.Count; ++index)
                    str = str + (object)this._ingredients[index].resourceId + "x" + (object)this._ingredients[index].quantity + (index == this._ingredients.Count - 1 ? (object)string.Empty : (object)",");
                return str;
            }
            set
            {
                if (!(value != string.Empty))
                    return;
                this._ingredients = ((IEnumerable<PSResourceGroup>)SharedManager.ParseRewardStrings(value)).ToList<PSResourceGroup>();
            }
        }

        [XmlIgnore]
        public Rarity Rarity { get; set; }

        [XmlAttribute("Rarity")]
        public string RarityString
        {
            get
            {
                return this.Rarity.ToString();
            }
            set
            {
                this.Rarity = !Enum.IsDefined(typeof(Rarity), (object)value) ? Rarity.None : (Rarity)Enum.Parse(typeof(Rarity), value);
            }
        }

        [XmlAttribute]
        public double EnhancementValue { get; set; }

        [XmlAttribute]
        public int MarketPrice { get; set; }

        [XmlAttribute]
        public int DropChance { get; set; }

        [XmlAttribute]
        public int AnimationId { get; set; }

        [XmlAttribute]
        public int ActiveAnimationId { get; set; }

        [XmlAttribute]
        public EnhancementType BonusEnhancementType { get; set; }

        [XmlAttribute]
        public double BonusEnhancementValue { get; set; }

        [XmlAttribute]
        public int SoundFileId { get; set; }

        [XmlAttribute]
        public int CharacterDesignId { get; set; }

        [XmlAttribute]
        public int Flags { get; set; }

        [XmlAttribute]
        public int BuildPrice { get; set; }

        [XmlAttribute]
        public int FairPrice { get; set; }

        [XmlAttribute]
        public int MinShipLevel { get; set; }

        [XmlIgnore]
        public TrainingDesign TrainingDesign
        {
            get
            {
                if (this._trainingDesign == null || this._trainingDesign.TrainingDesignId != this.TrainingDesignId)
                    this._trainingDesign = SingletonManager<TrainingManager>.Instance.GetTrainingDesignById(this.TrainingDesignId);
                return this._trainingDesign;
            }
        }

        [XmlAttribute]
        public int TrainingDesignId { get; set; }

        [XmlAttribute("ManufactureCost")]
        public string ManufactureCostString
        {
            get
            {
                return this._manufactureCostString;
            }
            set
            {
                this._manufactureCostString = value;
                this.ManufactureCost = SharedManager.ParseRewardStrings(this._manufactureCostString);
            }
        }

        [XmlIgnore]
        public PSResourceGroup[] ManufactureCost { get; set; }
    }
}