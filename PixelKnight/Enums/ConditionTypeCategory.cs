using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ConditionTypeCategory
    {
        [XmlEnum("Your Ship")] YourShip,
        [XmlEnum("Your Room")] YourRoom,
        [XmlEnum("Your Character")] YourCharacter,
        [XmlEnum("Enemy Ship")] EnemyShip,
        [XmlEnum("Enemy Room")] EnemyRoom,
        [XmlEnum("Enemy Character")] EnemyCharacter,
        [XmlEnum("None")] None,
        [XmlEnum("Current")] Current,
        [XmlEnum("EnemyCraft")] EnemyCraft,
        [XmlEnum("TargetRoom")] TargetRoom,
        [XmlEnum("YourCraft")] YourCraft,
    }
}