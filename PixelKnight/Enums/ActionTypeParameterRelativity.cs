using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ActionTypeParameterRelativity
    {
        [XmlEnum("None")] None,
        [XmlEnum("Relative")] Relative,
        [XmlEnum("Absolute")] Absolute,
        [XmlEnum("Percentage")] Percentage,
        [XmlEnum("Highest")] Highest,
        [XmlEnum("Lowest")] Lowest,
    }
}