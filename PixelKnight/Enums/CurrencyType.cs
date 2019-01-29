using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum CurrencyType
    {
        [XmlEnum("Unknown")] Unknown,
        [XmlEnum("Gas")] Gas,
        [XmlEnum("Mineral")] Mineral,
        [XmlEnum("Starbux")] Starbux,
        [XmlEnum("Item")] Item,
        [XmlEnum("Supply")] Supply,
        [XmlEnum("Character")] Character,
        [XmlEnum("Room")] Room,
        [XmlEnum("Points")] Point,
        [XmlEnum("Score")] Score,
        [XmlEnum("Research")] Research,
    }
}
