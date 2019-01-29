using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ExplosionType
    {
        [XmlEnum("Single")] Single,
        [XmlEnum("Radius")] Radius,
        [XmlEnum("Line")] Line,
        [XmlEnum("All")] All,
        [XmlEnum("Random")] Random,
    }
}