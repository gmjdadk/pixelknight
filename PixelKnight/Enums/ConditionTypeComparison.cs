using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ConditionTypeComparison
    {
        [XmlEnum("Higher")] Higher,
        [XmlEnum("Lower")] Lower,
        [XmlEnum("Equal")] Equal,
        [XmlEnum("HigherPercentage")] HigherPercentage,
        [XmlEnum("LowerPercentage")] LowerPercentage,
        [XmlEnum("EqualPercentage")] EqualPercentage,
        [XmlEnum("NotEqual")] NotEqual,
        [XmlEnum("HigherOrEqualPercentage")] HigherOrEqualPercentage,
        [XmlEnum("LowerOrEqualPercentage")] LowerOrEqualPercentage,
    }
}