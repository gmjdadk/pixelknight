using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum FlightType
    {
        [XmlEnum("Immediate")] Immediate,
        [XmlEnum("Linear")] Linear,
        [XmlEnum("Ion")] Ion,
        [XmlEnum("Homing")] Homing,
    }
}