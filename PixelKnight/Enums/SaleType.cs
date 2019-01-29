using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum SaleType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Character")] Character,
        [XmlEnum("Bonus")] Bonus,
        [XmlEnum("Room")] Room,
        [XmlEnum("Item")] Item,
    }
}