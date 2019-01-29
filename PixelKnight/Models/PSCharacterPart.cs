using System;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("CharacterPart")]
    [Serializable]
    public class PSCharacterPart : PSObject
    {
        public const string ALL_CHARACTERPARTS_CACHE = "allCharacterParts";
        [XmlIgnore]
        public SpriteDesign _standardSprite;
        [XmlIgnore]
        public SpriteDesign _actionSprite;
        [XmlIgnore]
        public SpriteDesign _standardBorderSprite;
        [XmlIgnore]
        public SpriteDesign _actionBorderSprite;

        [XmlIgnore]
        public SpriteDesign StandardSprite
        {
            get
            {
                if (this._standardSprite == null)
                    this._standardSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.StandardSpriteId);
                return this._standardSprite;
            }
        }

        [XmlIgnore]
        public SpriteDesign ActionSprite
        {
            get
            {
                if (this._actionSprite == null)
                    this._actionSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.ActionSpriteId);
                return this._actionSprite;
            }
        }

        [XmlIgnore]
        public SpriteDesign StandardBorderSprite
        {
            get
            {
                if (this._standardBorderSprite == null)
                    this._standardBorderSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.StandardBorderSpriteId);
                return this._standardBorderSprite;
            }
        }

        [XmlIgnore]
        public SpriteDesign ActionBorderSprite
        {
            get
            {
                if (this._actionBorderSprite == null)
                    this._actionBorderSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(this.ActionBorderSpriteId);
                return this._actionBorderSprite;
            }
        }

        [XmlAttribute]
        public int CharacterPartId { get; set; }

        [XmlAttribute]
        public string CharacterPartName { get; set; }

        [XmlIgnore]
        public CharacterPartType CharacterPartType { get; set; }

        [XmlAttribute("CharacterPartType")]
        public string CharacterPartTypeString
        {
            get
            {
                return this.CharacterPartType.ToString();
            }
            set
            {
                this.CharacterPartType = !Enum.IsDefined(typeof(CharacterPartType), (object)value) ? CharacterPartType.Head : (CharacterPartType)Enum.Parse(typeof(CharacterPartType), value);
            }
        }

        [XmlAttribute]
        public int StandardSpriteId { get; set; }

        [XmlAttribute]
        public int ActionSpriteId { get; set; }

        [XmlAttribute]
        public int StandardBorderSpriteId { get; set; }

        [XmlAttribute]
        public int ActionBorderSpriteId { get; set; }
    }
}
