using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum RoomSpriteType
    {
        [XmlEnum("Interior")] Interior,
        [XmlEnum("Exterior")] Exterior,
        [XmlEnum("Interior Destroyed")] InteriorDestroyed,
        [XmlEnum("Exterior Destroyed")] ExteriorDestroyed,
        [XmlEnum("Interior Activate")] InteriorActivate,
        [XmlEnum("Exterior Activate")] ExteriorActivate,
    }
}