using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ConditionTypeParameter
    {
        [XmlEnum("None")] None,
        [XmlEnum("Hp")] Hp,
        [XmlEnum("Shield")] Shield,
        [XmlEnum("Count")] Count,
        [XmlEnum("FriendlyCount")] FriendlyCount,
        [XmlEnum("EnemyCount")] EnemyCount,
        [XmlEnum("CloakingDuration")] CloakingDuration,
        [XmlEnum("SystemDamage")] SystemDamage,
        [XmlEnum("HullDamage")] HullDamage,
        [XmlEnum("StunLength")] StunLength,
        [XmlEnum("EMPLength")] EMPLength,
        [XmlEnum("CharacterDamage")] CharacterDamage,
        [XmlEnum("FireLength")] FireLength,
        [XmlEnum("BreachChance")] BreachChance,
        [XmlEnum("Speed")] Speed,
        [XmlEnum("DirectSystemDamage")] DirectSystemDamage,
        [XmlEnum("ShieldDamage")] ShieldDamage,
        [XmlEnum("Cost")] Cost,
    }
}