using System.Xml.Serialization;

namespace PixelKnight.Enums
{
    public enum ResearchDesignType
    {
        [XmlEnum("Normal")] Normal,
        [XmlEnum("CrewLevelUpCost")] CrewLevelUpCost,
        [XmlEnum("ConcurrentConstruction")] ConcurrentConstruction,
        [XmlEnum("Paint")] Paint,
        [XmlEnum("TradeCapacity")] TradeCapacity,
        [XmlEnum("ModuleCapacity")] ModuleCapacity,
        [XmlEnum("StickerCapacity")] StickerCapacity,
        [XmlEnum("AmmoSalvageCapacity")] AmmoSalvageCapacity,
        [XmlEnum("CollectAll")] CollectAll,
    }
}