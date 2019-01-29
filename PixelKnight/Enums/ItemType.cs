using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ItemType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Mineral")] Mineral,
        [XmlEnum("Gas")] Gas,
        [XmlEnum("Missile")] Missile,
        [XmlEnum("Craft")] Craft,
        [XmlEnum("Equipment")] Equipment,
        [XmlEnum("Android")] Android,
    }
}