using System;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class CraftDesign : PSObject
    {
        public const string ALL_CRAFTDESIGNS_CACHE = "allCraftDesigns";
        [NonSerialized]
        private MissileDesign _missileDesign;
        [NonSerialized]
        private SpriteDesign _sprite;

        [XmlIgnore]
        public SpriteDesign Sprite => _sprite ?? (_sprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(SpriteId));

        [XmlIgnore]
        public MissileDesign MissileDesign => _missileDesign ?? (_missileDesign = SingletonManager<RoomManager>.Instance.GetMissileDesignById(MissileDesignId));

        [XmlAttribute]
        public int CraftDesignId { get; set; }

        [XmlAttribute]
        public string CraftName { get; set; }

        [XmlAttribute]
        public string AchievementDescription { get; set; }

        [XmlAttribute]
        public double FlightSpeed { get; set; }

        [XmlAttribute]
        public int Reload { get; set; }

        [XmlAttribute]
        public int SpriteId { get; set; }

        [XmlAttribute]
        public int MissileDesignId { get; set; }

        [XmlAttribute]
        public int Volley { get; set; }

        [XmlAttribute]
        public int VolleyDelay { get; set; }

        [XmlAttribute]
        public int Hp { get; set; }
    }
}