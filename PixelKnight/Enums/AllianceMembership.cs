using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum AllianceMembership
    {
        [XmlEnum("None")] None = 0,
        [XmlEnum("Candidate")] Candidate = 1,
        [XmlEnum("Ensign")] Ensign = 2,
        [XmlEnum("Lieutenant")] Lieutenant = 3,
        [XmlEnum("Major")] Major = 4,
        [XmlEnum("Commander")] Commander = 5,
        [XmlEnum("ViceAdmiral")] ViceAdmiral = 6,
        [XmlEnum("FleetAdmiral")] FleetAdmiral = 7,
        [XmlEnum("Starbase")] Starbase = 100, // 0x00000064
    }
}