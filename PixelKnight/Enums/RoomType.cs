using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum RoomType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Laser")] Laser,
        [XmlEnum("Bridge")] Bridge,
        [XmlEnum("Engine")] Engine,
        [XmlEnum("Lift")] Lift,
        [XmlEnum("Bedroom")] Bedroom,
        [XmlEnum("Shield")] Shield,
        [XmlEnum("Storage")] Storage,
        [XmlEnum("Reactor")] Reactor,
        [XmlEnum("Missile")] Missile,
        [XmlEnum("Teleport")] Teleport,
        [XmlEnum("Mineral")] Mineral,
        [XmlEnum("Gas")] Gas,
        [XmlEnum("Command")] Command,
        [XmlEnum("Research")] Research,
        [XmlEnum("Wall")] Wall,
        [XmlEnum("Council")] Council,
        [XmlEnum("Carrier")] Carrier,
        [XmlEnum("AntiCraft")] AntiCraft,
        [XmlEnum("Medical")] Medical,
        [XmlEnum("Trap")] Trap,
        [XmlEnum("Radar")] Radar,
        [XmlEnum("Stealth")] Stealth,
        [XmlEnum("Training")] Training,
        [XmlEnum("Cannon")] Cannon,
        [XmlEnum("Corridor")] Corridor,
        [XmlEnum("StationMissile")] StationMissile,
        [XmlEnum("Android")] Android,
        [XmlEnum("Printer")] Printer,
        [XmlEnum("Supply")] Supply,
        [XmlEnum("SuperWeapon")] SuperWeapon,
        [XmlEnum("Recycling")] Recycling,
    }
}