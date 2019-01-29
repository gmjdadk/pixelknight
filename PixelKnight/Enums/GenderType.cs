using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum GenderType
    {
        [XmlEnum("Unknown")] Unknown,
        [XmlEnum("Male")] Male,
        [XmlEnum("Female")] Female,
    }
}