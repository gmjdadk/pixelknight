using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ActionTypeCategory
    {
        [XmlEnum("SetPower")] SetPower,
        [XmlEnum("Target")] Target,
        [XmlEnum("SetItem")] SetItem,
        [XmlEnum("UseSpecialPower")] UseSpecialPower,
    }
}