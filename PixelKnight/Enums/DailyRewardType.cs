using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum DailyRewardType
    {
        [XmlEnum("Starbux")] Starbux,
        [XmlEnum("Mineral")] Mineral,
        [XmlEnum("Gas")] Gas,
        [XmlEnum("Item")] Item,
        [XmlEnum("Crew")] Crew,
    }
}