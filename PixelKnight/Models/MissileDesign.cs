using System;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class MissileDesign : PSObject
    {
        public const string ALL_MISSILEDESIGNS_CACHE = "allMissileDesigns";

        [XmlAttribute]
        public int MissileDesignId { get; set; }

        [XmlAttribute]
        public string MissileDesignName { get; set; }

        [XmlAttribute]
        public double SystemDamage { get; set; }

        [XmlAttribute]
        public double HullDamage { get; set; }

        [XmlAttribute]
        public double ShieldDamage { get; set; }

        [XmlAttribute]
        public double DirectSystemDamage { get; set; }

        [XmlAttribute]
        public int StunLength { get; set; }

        [XmlAttribute]
        public int EMPLength { get; set; }

        [XmlAttribute]
        public double CharacterDamage { get; set; }

        [XmlAttribute]
        public int FireLength { get; set; }

        [XmlAttribute]
        public double BreachChance { get; set; }

        [XmlIgnore]
        public MissileType MissileType { get; set; }

        [XmlAttribute("MissileType")]
        public string MissileTypeString
        {
            get => MissileType.ToString();
            set => MissileType = !Enum.IsDefined(typeof(MissileType), value) ? MissileType.Laser : (MissileType)Enum.Parse(typeof(MissileType), value);
        }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int SpriteId { get; set; }

        [XmlIgnore]
        public FlightType FlightType { get; set; }

        [XmlAttribute("FlightType")]
        public string FlightTypeString
        {
            get => FlightType.ToString();
            set => FlightType = !Enum.IsDefined(typeof(FlightType), value) ? FlightType.Immediate : (FlightType)Enum.Parse(typeof(FlightType), value);
        }

        [XmlIgnore]
        public ExplosionType ExplosionType { get; set; }

        [XmlAttribute("ExplosionType")]
        public string ExplosionTypeString
        {
            get => ExplosionType.ToString();
            set => ExplosionType = !Enum.IsDefined(typeof(ExplosionType), value) ? ExplosionType.Single : (ExplosionType)Enum.Parse(typeof(ExplosionType), value);
        }

        [XmlAttribute]
        public int ExplosionRadius { get; set; }

        [XmlAttribute]
        public int Speed { get; set; }

        [XmlAttribute]
        public int Volley { get; set; }

        [XmlAttribute]
        public int VolleyDelay { get; set; }

        [XmlAttribute]
        public int LaunchAnimationId { get; set; }

        [XmlAttribute]
        public int LaunchSoundFileId { get; set; }

        [XmlAttribute]
        public int HitAnimationId { get; set; }

        [XmlAttribute]
        public int HitSoundFileId { get; set; }

        [XmlAttribute]
        public int AnimationId { get; set; }

        [XmlAttribute]
        public int HullPercentageDamage { get; set; }

        public double GetProperty(ConditionTypeParameter key)
        {
            switch (key)
            {
                case ConditionTypeParameter.SystemDamage:
                    return SystemDamage;
                case ConditionTypeParameter.HullDamage:
                    return HullDamage;
                case ConditionTypeParameter.StunLength:
                    return StunLength;
                case ConditionTypeParameter.EMPLength:
                    return EMPLength;
                case ConditionTypeParameter.CharacterDamage:
                    return CharacterDamage;
                case ConditionTypeParameter.FireLength:
                    return FireLength;
                case ConditionTypeParameter.BreachChance:
                    return BreachChance;
                case ConditionTypeParameter.Speed:
                    return Speed;
                case ConditionTypeParameter.DirectSystemDamage:
                    return DirectSystemDamage;
                case ConditionTypeParameter.ShieldDamage:
                    return ShieldDamage;
                default:
                    return 0.0;
            }
        }

        [XmlAttribute]
        public int FlightArgumentX { get; set; }

        [XmlAttribute]
        public int FlightArgumentY { get; set; }
    }
}