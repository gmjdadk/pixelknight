using System;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class ResearchDesign : PSObject
    {
        public const string ALL_RESEARCHDESIGNS_CACHE = "allResearchDesigns";
        [NonSerialized]
        private SpriteDesign _sprite;
        [NonSerialized]
        private ResearchDesign _nextDesign;

        [XmlIgnore]
        public SpriteDesign Sprite => _sprite ?? (_sprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(ImageSpriteId));

        [XmlIgnore]
        public ResearchDesign NextDesign => _nextDesign ?? (_nextDesign = SingletonManager<ResearchManager>.Instance.GetNextResearchDesign(ResearchDesignId));

        [XmlIgnore]
        public ResearchDesign TopDesign
        {
            get
            {
                var researchDesign1 = this;
                for (var researchDesign2 = researchDesign1; researchDesign2 != null; researchDesign2 = researchDesign1.NextDesign)
                    researchDesign1 = researchDesign2;
                return researchDesign1;
            }
        }

        [XmlAttribute]
        public int ResearchDesignId { get; set; }

        [XmlAttribute]
        public string ResearchName { get; set; }

        [XmlAttribute]
        public string ResearchDescription { get; set; }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int ImageSpriteId { get; set; }

        [XmlAttribute]
        public int GasCost { get; set; }

        [XmlAttribute]
        public int RequiredLabLevel { get; set; }

        [XmlAttribute]
        public int ResearchTime { get; set; }

        [XmlIgnore]
        public ResearchDesignType ResearchDesignType { get; set; }

        [XmlAttribute("ResearchDesignType")]
        public string ResearchDesignTypeString
        {
            get => ResearchDesignType.ToString();
            set => ResearchDesignType = !Enum.IsDefined(typeof(ResearchDesignType), value) ? ResearchDesignType.Normal : (ResearchDesignType)Enum.Parse(typeof(ResearchDesignType), value);
        }

        [XmlAttribute]
        public int Argument { get; set; }

        [XmlAttribute]
        public int StarbuxCost { get; set; }

        [XmlAttribute]
        public int RequiredResearchDesignId { get; set; }

        [XmlAttribute]
        public int RequiredItemDesignId { get; set; }

        [XmlAttribute]
        public int AvailabilityMask { get; set; }
    }
}