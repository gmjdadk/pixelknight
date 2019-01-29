using System;
using System.Drawing;
using System.Xml.Serialization;
using PixelKnight.Enums;

namespace PixelKnight.Models
{
    [XmlRoot("ConditionType")]
    [Serializable]
    public class ConditionTypeDesign : PSObject
    {
        public const string ALL_CONDITIONTYPES_CACHE = "allConditionTypes";
        [NonSerialized]
        private string _colorString;

        [XmlIgnore]
        public bool IsAvailable
        {
            get
            {
                PSResearch research = SingletonManager<ResearchManager>.Instance.GetResearch(RequiredResearchDesignId);
                return RequiredResearchDesignId == 0 || research != null && research.ResearchState == ResearchState.Completed;
            }
        }

        [XmlAttribute]
        public int ConditionTypeId { get; set; }

        [XmlAttribute]
        public string ConditionTypeName { get; set; }

        [XmlAttribute]
        public string ConditionTypeDescription { get; set; }

        [XmlIgnore]
        public ConditionTypeCategory ConditionTypeCategory { get; set; }

        [XmlAttribute("ConditionTypeCategory")]
        public string ConditionTypeCategoryString
        {
            get => ConditionTypeCategory.ToString();
            set => ConditionTypeCategory = !Enum.IsDefined(typeof(ConditionTypeCategory), value.Replace(" ", string.Empty)) ? ConditionTypeCategory.YourShip : (ConditionTypeCategory)Enum.Parse(typeof(ConditionTypeCategory), value.Replace(" ", string.Empty));
        }

        [XmlIgnore]
        public ConditionTypeComparison ConditionTypeComparison { get; set; }

        [XmlAttribute("ConditionTypeComparison")]
        public string ConditionTypeComparisonString
        {
            get => ConditionTypeComparison.ToString();
            set => ConditionTypeComparison = !Enum.IsDefined(typeof(ConditionTypeComparison), value.Replace(" ", string.Empty)) ? ConditionTypeComparison.Higher : (ConditionTypeComparison)Enum.Parse(typeof(ConditionTypeComparison), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int ConditionTypeParameterValue { get; set; }

        [XmlIgnore]
        public ConditionTypeParameter ConditionTypeParameter { get; set; }

        [XmlAttribute("ConditionTypeParameter")]
        public string ConditionTypeParameterString
        {
            get => ConditionTypeParameter.ToString();
            set => ConditionTypeParameter = !Enum.IsDefined(typeof(ConditionTypeParameter), value.Replace(" ", string.Empty)) ? ConditionTypeParameter.None : (ConditionTypeParameter)Enum.Parse(typeof(ConditionTypeParameter), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int ImageSpriteId { get; set; }

        [XmlIgnore]
        public RoomType RoomType { get; set; }

        [XmlAttribute("RoomType")]
        public string RoomTypeString
        {
            get => RoomType.ToString();
            set => RoomType = !Enum.IsDefined(typeof(RoomType), value.Replace(" ", string.Empty)) ? RoomType.None : (RoomType)Enum.Parse(typeof(RoomType), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int RequiredResearchDesignId { get; set; }

        [XmlIgnore]
        public Color Color { get; set; }
    }
}