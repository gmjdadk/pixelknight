using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum RaceType
    {
        [XmlEnum("Unknown")] Unknown,
        [XmlEnum("Asian")] Asian,
        [XmlEnum("White")] White,
        [XmlEnum("Black")] Black,
        [XmlEnum("Animal")] Animal,
        [XmlEnum("Alien")] Alien,
        [XmlEnum("Robot")] Robot,
    }
}