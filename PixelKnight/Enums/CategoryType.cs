using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum CategoryType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Weapon")] Weapon,
        [XmlEnum("Defence")] Defence,
        [XmlEnum("Support")] Support,
        [XmlEnum("Resources")] Resources,
    }
}