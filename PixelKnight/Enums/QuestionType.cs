using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum QuestionType
    {
        [XmlEnum("Unknown")] Unknown,
        [XmlEnum("Item")] Item,
        [XmlEnum("Character")] Character,
        [XmlEnum("Room")] Room,
    }
}