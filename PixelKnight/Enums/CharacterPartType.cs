using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum CharacterPartType
    {
        [XmlEnum("Head")] Head,
        [XmlEnum("Body")] Body,
        [XmlEnum("Leg")] Leg,
    }
}