using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum EnhancementType
    {
        [XmlEnum("None")] None,
        [XmlEnum("Hp")] Hp,
        [XmlEnum("Attack")] Attack,
        [XmlEnum("Pilot")] Pilot,
        [XmlEnum("Repair")] Repair,
        [XmlEnum("Weapon")] Weapon,
        [XmlEnum("Science")] Science,
        [XmlEnum("Engine")] Engine,
        [XmlEnum("Stamina")] Stamina,
        [XmlEnum("Ability")] Ability,
        [XmlEnum("FireResistance")] FireResistance,
        [XmlEnum("SharpShooterSkill")] SharpShooterSkill,
        [XmlEnum("InstantKillSkill")] InstantKillSkill,
        [XmlEnum("ResurrectSkill")] ResurrectSkill,
        [XmlEnum("FreezeAttackSkill")] FreezeAttackSkill,
        [XmlEnum("EmpSkill")] EmpSkill,
        [XmlEnum("BloodThirstSkill")] BloodThirstSkill,
        [XmlEnum("MedicalSkill")] MedicalSkill,
    }
}