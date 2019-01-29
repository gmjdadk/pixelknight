using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("Alliance")]
    [Serializable]
    public class PSAlliance : PSObject
    {
        public const string ALL_ALLIANCES_CACHE = "allAlliances";
        [NonSerialized]
        private string _allianceDescription;
        [NonSerialized]
        private string _allianceName;
        [NonSerialized]
        private DateTime _immunityDate;

        [XmlAttribute]
        public int AllianceId { get; set; }

        [XmlAttribute]
        public string AllianceName
        {
            get => _allianceName;
            set => _allianceName = SharedManager.StripUnsupportedUnicodeChars(value);
        }

        [XmlAttribute]
        public int AllianceSpriteId { get; set; }

        [XmlAttribute]
        public int MinTrophyRequired { get; set; }

        [XmlAttribute]
        public bool RequiresApproval { get; set; }

        [XmlAttribute]
        public int ChannelId { get; set; }

        [XmlAttribute]
        public int Trophy { get; set; }

        [XmlAttribute]
        public string AllianceCountryCode { get; set; }

        [XmlAttribute]
        public string AllianceDescription
        {
            get => _allianceDescription;
            set => _allianceDescription = SharedManager.StripUnsupportedUnicodeChars(value);
        }

        [XmlAttribute]
        public bool EnableWars { get; set; }

        [XmlAttribute]
        public int NumberOfMembers { get; set; }

        [XmlAttribute]
        public int Ranking { get; set; }

        [XmlAttribute]
        public int AllianceShipUserId { get; set; }

        [XmlAttribute]
        public int Credits { get; set; }

        [XmlIgnore]
        public double ImmunityTimeRemaining => (ImmunityDate - SharedManager.CurrentTime()).TotalSeconds;

        [XmlIgnore]
        public DateTime ImmunityDate
        {
            get => _immunityDate;
            set => _immunityDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
        }

        [XmlAttribute("ImmunityDate")]
        public string ImmunityDateString
        {
            get
            {
                if (this.ImmunityDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.ImmunityDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.ImmunityDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public int Score { get; set; }

        [XmlAttribute]
        public int DivisionDesignId { get; set; }

        [XmlAttribute]
        public int ChampionshipScore { get; set; }

        [XmlIgnore]
        public int AllianceRanking
        {
            get
            {
                if (this.AllianceId == 0 || !SingletonManager<TournamentManager>.Instance.IsLastTournamentPhase || SingletonManager<AllianceManager>.Instance.topAlliances.Count == 0)
                    return 0;
                List<PSAlliance> source = new List<PSAlliance>((IEnumerable<PSAlliance>)SingletonManager<AllianceManager>.Instance.topAlliances);
                foreach (PSAlliance psAlliance in source.ToArray())
                {
                    if (psAlliance.DivisionDesignId == 0)
                        source.Remove(psAlliance);
                }
                List<PSAlliance> list = source.OrderByDescending<PSAlliance, int>((Func<PSAlliance, int>)(o => o.Score)).ToList<PSAlliance>().OrderBy<PSAlliance, int>((Func<PSAlliance, int>)(o => o.DivisionDesignId)).ToList<PSAlliance>();
                int num = 0;
                for (int index = 0; index < list.Count; ++index)
                {
                    if (list[index].AllianceId == this.AllianceId)
                        num = index + 1;
                }
                return num;
            }
        }
    }
}