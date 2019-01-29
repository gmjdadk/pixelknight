using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum UserType
    {
        [XmlEnum("Unverified")] UserTypeUnverified,
        [XmlEnum("Verified")] UserTypeVerified,
        [XmlEnum("Administrator")] UserTypeAdministrator,
        [XmlEnum("Banned")] UserTypeBanned,
        [XmlEnum("Backer")] UserTypeBacker,
        [XmlEnum("Mission")] UserTypeMission,
        [XmlEnum("UserTypePaying")] UserTypePaying,
        [XmlEnum("UserTypeJailBroken")] UserTypeJailBroken,
        [XmlEnum("UserTypeAlliance")] UserTypeAlliance,
    }
}