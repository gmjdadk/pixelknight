using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ShipType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Player")] Player,
        [XmlEnum("Alliance")] Alliance,
        [XmlEnum("Both")] Both,
    }
}
