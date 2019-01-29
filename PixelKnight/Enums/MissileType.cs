using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum MissileType
    {
        [XmlEnum("Laser")] Laser,
        [XmlEnum("Rocket")] Rocket,
        [XmlEnum("LongDistanceMissile")] LongDistanceMissile,
    }
}