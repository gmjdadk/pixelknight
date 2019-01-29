using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ResearchState
    {
        [XmlEnum("None")] None,
        [XmlEnum("Researching")] Researching,
        [XmlEnum("Completed")] Completed,
    }
}