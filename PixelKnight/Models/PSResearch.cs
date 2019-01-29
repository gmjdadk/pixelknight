using System;
using System.Xml.Serialization;
using PixelKnight.Managers;

namespace PixelKnight.Models
{
    [XmlRoot("Research")]
    [Serializable]
    public class PSResearch : PSObject
    {
        public const string ALL_RESEARCHES_CACHE = "allResearches";
        [NonSerialized]
        private DateTime _researchStartDate;
        [NonSerialized]
        private ResearchDesign _researchDesign;

        [XmlIgnore]
        public double ResearchTimeRemaining
        {
            get
            {
                return (double)this.ResearchDesign.ResearchTime - (Singleton<SharedManager>.Instance.CurrentTime(false) - this.ResearchStartDate).TotalSeconds;
            }
        }

        [XmlIgnore]
        public ResearchDesign ResearchDesign
        {
            get
            {
                if (this._researchDesign == null)
                    this._researchDesign = SingletonManager<ResearchManager>.Instance.GetResearchDesignById(this.ResearchDesignId);
                return this._researchDesign;
            }
        }

        [XmlAttribute]
        public int ResearchId { get; set; }

        [XmlAttribute]
        public int ShipId { get; set; }

        [XmlAttribute]
        public int ResearchDesignId { get; set; }

        [XmlAttribute]
        public DateTime ResearchStartDate
        {
            get
            {
                return this._researchStartDate;
            }
            set
            {
                this._researchStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlIgnore]
        public ResearchState ResearchState { get; set; }

        [XmlAttribute("ResearchState")]
        public string ResearchStateString
        {
            get
            {
                return this.ResearchState.ToString();
            }
            set
            {
                this.ResearchState = !Enum.IsDefined(typeof(ResearchState), (object)value) ? ResearchState.None : (ResearchState)Enum.Parse(typeof(ResearchState), value);
            }
        }
    }
}