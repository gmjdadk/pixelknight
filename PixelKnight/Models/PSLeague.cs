using System;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("League")]
    [Serializable]
    public class PSLeague : PSObject
    {
        public const string ALL_LEAGUES_CACHE = "allLeagues";
        [NonSerialized]
        private SpriteDesign _logoSprite;

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

        [XmlAttribute]
        public int LeagueId { get; set; }

        [XmlAttribute]
        public string LeagueName { get; set; }

        [XmlAttribute]
        public int MinTrophy { get; set; }

        [XmlAttribute]
        public int MaxTrophy { get; set; }

        [XmlAttribute]
        public int MineralReward { get; set; }

        [XmlAttribute]
        public int GasReward { get; set; }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int BackgroundSpriteId { get; set; }
    }
}