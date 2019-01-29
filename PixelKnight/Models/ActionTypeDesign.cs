using System;
using System.Xml.Serialization;
using PixelKnight.Enums;

namespace PixelKnight.Models
{
    [XmlRoot("ActionType")]
    [Serializable]
    public class ActionTypeDesign : PSObject
    {
        public const string ALL_ACTIONTYPES_CACHE = "allActionTypes";
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
        public int ActionTypeId { get; set; }

        [XmlAttribute]
        public string ActionTypeName { get; set; }

        [XmlIgnore]
        public RoomType RoomType { get; set; }

        [XmlAttribute("RoomType")]
        public string RoomTypeString
        {
            get => RoomType.ToString();
            set => RoomType = !Enum.IsDefined(typeof(RoomType), value) ? RoomType.None : (RoomType)Enum.Parse(typeof(RoomType), value);
        }

        [XmlIgnore]
        public ActionTypeCategory ActionTypeCategory { get; set; }

        [XmlAttribute("ActionTypeCategory")]
        public string ActionTypeCategoryString
        {
            get => ActionTypeCategory.ToString();
            set => ActionTypeCategory = !Enum.IsDefined(typeof(ActionTypeCategory), value) ? ActionTypeCategory.SetPower : (ActionTypeCategory)Enum.Parse(typeof(ActionTypeCategory), value);
        }

        [XmlAttribute]
        public string ActionTypeKey { get; set; }

        [XmlAttribute]
        public int ActionTypeParameterValue { get; set; }

        [XmlAttribute]
        public string ActionTypeDescription { get; set; }

        [XmlIgnore]
        public ActionTypeParameterRelativity ActionTypeParameterRelativity { get; set; }

        [XmlAttribute("ActionTypeParameterRelativity")]
        public string ActionTypeParameterRelativityString
        {
            get => ActionTypeParameterRelativity.ToString();
            set => ActionTypeParameterRelativity = !Enum.IsDefined(typeof(ActionTypeParameterRelativity), value.Replace(" ", string.Empty)) ? ActionTypeParameterRelativity.None : (ActionTypeParameterRelativity)Enum.Parse(typeof(ActionTypeParameterRelativity), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int ImageSpriteId { get; set; }

        [XmlIgnore]
        public ConditionTypeCategory ConditionTypeCategory { get; set; }

        [XmlAttribute("ConditionTypeCategory")]
        public string ConditionTypeCategoryString
        {
            get => ConditionTypeCategory.ToString();
            set => ConditionTypeCategory = !Enum.IsDefined(typeof(ConditionTypeCategory), value.Replace(" ", string.Empty)) ? ConditionTypeCategory.YourShip : (ConditionTypeCategory)Enum.Parse(typeof(ConditionTypeCategory), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int RequiredResearchDesignId { get; set; }

        [XmlIgnore]
        public ConditionTypeParameter ConditionTypeParameter { get; set; }

        [XmlAttribute("ConditionTypeParameter")]
        public string ConditionTypeParameterString
        {
            get => ConditionTypeParameter.ToString();
            set => ConditionTypeParameter = !Enum.IsDefined(typeof(ConditionTypeParameter), value.Replace(" ", string.Empty)) ? ConditionTypeParameter.None : (ConditionTypeParameter)Enum.Parse(typeof(ConditionTypeParameter), value.Replace(" ", string.Empty));
        }
    }
}