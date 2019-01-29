using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class CharacterDesign : PSObject
    {
        [XmlArray("CharacterParts")]
        [XmlArrayItem("CharacterPart")]
        public List<PSCharacterPart> CharacterParts = new List<PSCharacterPart>();
        public const string ALL_CHARACTERDESIGNS_CACHE = "allCharacterDesigns";
        [NonSerialized]
        private SpriteDesign _profileSprite;
        private CollectionDesign _collectionDesign;

        [XmlIgnore]
        public bool IsAbilityActive
        {
            get
            {
                return this.SpecialAbilityType != SpecialAbilityType.FireWalk;
            }
        }

        [XmlAttribute]
        public int CharacterDesignId { get; set; }

        [XmlAttribute]
        public string CharacterDesignName { get; set; }

        [XmlIgnore]
        public GenderType GenderType { get; set; }

        [XmlAttribute("GenderType")]
        public string GenderTypeString
        {
            get
            {
                return this.GenderType.ToString();
            }
            set
            {
                this.GenderType = !Enum.IsDefined(typeof(GenderType), (object)value) ? GenderType.Unknown : (GenderType)Enum.Parse(typeof(GenderType), value);
            }
        }

        [XmlIgnore]
        public RaceType RaceType { get; set; }

        [XmlAttribute("RaceType")]
        public string RaceTypeString
        {
            get
            {
                return this.RaceType.ToString();
            }
            set
            {
                this.RaceType = !Enum.IsDefined(typeof(RaceType), (object)value) ? RaceType.Unknown : (RaceType)Enum.Parse(typeof(RaceType), value);
            }
        }

        [XmlAttribute]
        public int Hp { get; set; }

        [XmlAttribute]
        public double Pilot { get; set; }

        [XmlAttribute]
        public int FireResistance { get; set; }

        [XmlAttribute]
        public double Repair { get; set; }

        [XmlAttribute]
        public double Weapon { get; set; }

        [XmlAttribute]
        public double Science { get; set; }

        [XmlAttribute]
        public double Engine { get; set; }

        [XmlAttribute]
        public double Research { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        [XmlAttribute]
        public double Attack { get; set; }

        [XmlAttribute]
        public double WalkingSpeed { get; set; }

        [XmlAttribute]
        public double ManufactureRate { get; set; }

        [XmlAttribute]
        public int MineralCost { get; set; }

        [XmlAttribute]
        public int GasCost { get; set; }

        [XmlAttribute]
        public int MinShipLevel { get; set; }

        [XmlAttribute]
        public int FinalHp { get; set; }

        [XmlAttribute]
        public double FinalPilot { get; set; }

        [XmlAttribute]
        public double FinalRepair { get; set; }

        [XmlAttribute]
        public double FinalWeapon { get; set; }

        [XmlAttribute]
        public double FinalScience { get; set; }

        [XmlAttribute]
        public double FinalEngine { get; set; }

        [XmlAttribute]
        public double FinalResearch { get; set; }

        [XmlAttribute]
        public double FinalAttack { get; set; }

        [XmlAttribute]
        public int CharacterLegPartId { get; set; }

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
        public int Chance { get; set; }

        [XmlIgnore]
        public ProgressionType ProgressionType { get; set; }

        [XmlAttribute("ProgressionType")]
        public string ProgressionTypeString
        {
            get
            {
                return this.ProgressionType.ToString();
            }
            set
            {
                this.ProgressionType = !Enum.IsDefined(typeof(ProgressionType), (object)value) ? ProgressionType.Linear : (ProgressionType)Enum.Parse(typeof(ProgressionType), value);
            }
        }

        [XmlAttribute]
        public string CharacterDesignDescription { get; set; }

        [XmlAttribute]
        public int TapSoundFileId { get; set; }

        [XmlAttribute]
        public int ActionSoundFileId { get; set; }

        [XmlAttribute]
        public int XpRequirementScale { get; set; }

        [XmlAttribute]
        public int MaxCharacterLevel { get; set; }

        [XmlAttribute]
        public string CollectionKey { get; set; }

        [XmlIgnore]
        public SpecialAbilityType SpecialAbilityType { get; set; }

        [XmlAttribute("SpecialAbilityType")]
        public string SpecialAbilityTypeString
        {
            get
            {
                return this.SpecialAbilityType.ToString();
            }
            set
            {
                this.SpecialAbilityType = !Enum.IsDefined(typeof(SpecialAbilityType), (object)value) ? SpecialAbilityType.None : (SpecialAbilityType)Enum.Parse(typeof(SpecialAbilityType), value);
            }
        }

        [XmlAttribute]
        public int SpecialAbilityArgument { get; set; }

        [XmlAttribute]
        public int SpecialAbilityFinalArgument { get; set; }

        [XmlIgnore]
        public SpriteDesign ProfileSprite
        {
            get
            {
                if (this._profileSprite == null)
                    this._profileSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.ProfileSpriteId);
                return this._profileSprite;
            }
        }

        [XmlAttribute]
        public int ProfileSpriteId { get; set; }

        [XmlAttribute]
        public int RunSpeed { get; set; }

        [XmlAttribute]
        public int TrainingCapacity { get; set; }

        [XmlAttribute]
        public int EquipmentMask { get; set; }

        [XmlAttribute]
        public string SpeechVoice { get; set; }

        [XmlAttribute]
        public string SpeechPhrases { get; set; }

        [XmlAttribute]
        public double SpeechRate { get; set; }

        [XmlAttribute]
        public double SpeechPitch { get; set; }

        [XmlAttribute]
        public int Flags { get; set; }

        [XmlAttribute]
        public int CollectionDesignId { get; set; }

        [XmlIgnore]
        public CollectionDesign CollectionDesign
        {
            get
            {
                if (this._collectionDesign == null)
                    this._collectionDesign = SingletonManager<CollectionManager>.Instance.GetCollectionDesignById(this.CollectionDesignId);
                return this._collectionDesign;
            }
        }

        public string GetAbilityDescriptionString(bool adventure = false)
        {
            return string.Empty;
        }
    }
}
