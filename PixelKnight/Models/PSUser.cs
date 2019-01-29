using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("User")]
    [Serializable]
    public class PSUser : PSObject
    {
        [NonSerialized]
        private List<int> _unlockedCharacterDesignIds = new List<int>();
        [NonSerialized]
        private List<int> _unlockedShipDesignIds = new List<int>();
        [NonSerialized]
        private DateTime _creationDate;
        [NonSerialized]
        private DateTime _lastLoginDate;
        [NonSerialized]
        private DateTime _lastAlertDate;
        [NonSerialized]
        private DateTime _facebookTokenExpiryDate;
        [NonSerialized]
        private DateTime _vipExpiryDate;
        [NonSerialized]
        private DateTime _lastHeartBeatDate;
        [NonSerialized]
        private DateTime _lastCollectedStarbuxDate;
        [NonSerialized]
        private DateTime _lastRewardActionDate;
        [NonSerialized]
        private DateTime _lastPurchaseDate;
        [NonSerialized]
        private DateTime _lastCatalogPurchaseDate;
        [NonSerialized]
        private DateTime _cooldownExpiryDate;
        [NonSerialized]
        private DateTime _allianceJoinDate;
        [NonSerialized]
        private DateTime _blockAuthAttemptsUntilDate;
        [NonSerialized]
        private SpriteDesign _iconSprite;
        [NonSerialized]
        private SpriteDesign _allianceSprite;
        [NonSerialized]
        private string _allianceName;
        [NonSerialized]
        private string _name;
        [NonSerialized]
        private List<CharacterDesign> _unlockedCharacterDesigns;
        [NonSerialized]
        private List<ShipDesign> _unlockedShipDesigns;
        [XmlIgnore]
        [NonSerialized]
        public TimeSpan serverTimeDiff;
        [NonSerialized]
        private ShipDesign _shipDesign;
        [NonSerialized]
        private CharacterDesign _captainCharacterDesign;
        [NonSerialized]
        private PSAlliance _alliance;

        [XmlIgnore]
        public CharacterDesign CaptainCharacterDesign
        {
            get
            {
                if (CaptainCharacterDesignId == 0 && SingletonManager<UserManager>.Instance.user.Id == Id)
                    CaptainCharacterDesignId = SingletonManager<ShipManager>.Instance.PlayerShip.PlayerPSCharacter.CharacterDesignId;
                if (CaptainCharacterDesignId != 0 && (_captainCharacterDesign == null || _captainCharacterDesign.CharacterDesignId != CaptainCharacterDesignId))
                    _captainCharacterDesign = SingletonManager<CharacterManager>.Instance.GetCharacterDesignByID(CaptainCharacterDesignId);
                return _captainCharacterDesign;
            }
        }

        [XmlAttribute]
        public int CaptainCharacterDesignId { get; set; }

        [XmlIgnore]
        public AlliancePermissions AlliancePermissions
        {
            get
            {
                var alliancePermissions = new AlliancePermissions();
                alliancePermissions.SetAllianceUserPermissions(AllianceMembership);
                return alliancePermissions;
            }
        }

        [XmlIgnore]
        public int Points => TournamentRewardPoints + PurchaseRewardPoints - UsedRewardPoints;

        [XmlIgnore]
        public int SaleCapacity
        {
            get
            {
                PSResearch completedResearchOfType = SingletonManager<ResearchManager>.Instance.GetHighestCompletedResearchOfType(ResearchDesignType.TradeCapacity);
                if (completedResearchOfType != null)
                    return completedResearchOfType.ResearchDesign.Argument;
                return 0;
            }
        }

        [XmlIgnore]
        public PSLeague PsLeague => SingletonManager<LeagueManager>.Instance.GetLeagueForTrophyCount(Trophy);

        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public string FacebookToken { get; set; }

        [XmlAttribute]
        public string FacebookId { get; set; }

        [XmlAttribute]
        public int BadgeCount { get; set; }

        [XmlIgnore]
        public UserType UserType { get; set; }

        [XmlAttribute("UserType")]
        public string UserTypeString
        {
            get => UserType.ToString();
            set => UserType = !Enum.IsDefined(typeof(UserType), value) ? UserType.UserTypeUnverified : (UserType)Enum.Parse(typeof(UserType), value);
        }

        [XmlAttribute]
        public bool IsPushNotificationEnabled { get; set; }

        [XmlAttribute]
        public bool IsEmailNotificationEnabled { get; set; }

        [XmlAttribute]
        public bool IsFacebookNotificationEnabled { get; set; }

        [XmlAttribute]
        public bool IsTwitterNotificationEnabled { get; set; }

        [XmlAttribute]
        public int FacebookFriendCount { get; set; }

        [XmlIgnore]
        public GenderType GenderType { get; set; }

        [XmlAttribute("GenderType")]
        public string GenderTypeString
        {
            get => GenderType.ToString();
            set => GenderType = !Enum.IsDefined(typeof(GenderType), value) ? GenderType.Unknown : (GenderType)Enum.Parse(typeof(GenderType), value);
        }

        [XmlIgnore]
        public RaceType RaceType { get; set; }

        [XmlAttribute("RaceType")]
        public string RaceTypeString
        {
            get => RaceType.ToString();
            set => RaceType = !Enum.IsDefined(typeof(RaceType), value) ? RaceType.Unknown : (RaceType)Enum.Parse(typeof(RaceType), value);
        }

        [XmlAttribute]
        public int Credits { get; set; }

        [XmlAttribute]
        public string ProfileImageUrl { get; set; }

        [XmlAttribute]
        public int Trophy { get; set; }

        [XmlAttribute]
        public string GameCenterName { get; set; }

        [XmlIgnore]
        public List<int> CompletedMissionDesignIds { get; set; }

        [XmlAttribute("CompletedMissionDesigns")]
        public string CompletedMissionDesignIdString
        {
            get
            {
                string str = string.Empty;
                if (CompletedMissionDesignIds != null)
                {
                    for (int index = 0; index < CompletedMissionDesignIds.Count; ++index)
                        str = str + CompletedMissionDesignIds[index] + (index == CompletedMissionDesignIds.Count - 1 ? string.Empty : (object)",");
                }
                return str;
            }
            set
            {
                List<int> intList;
                if (value != string.Empty)
                {
                    string[] strArray = value.Replace(" ", string.Empty).Split(',');
                    if (PSUser.\u003C\u003Ef__mg\u0024cache0 == null)
            PSUser.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache0 = PSUser.\u003C\u003Ef__mg\u0024cache0;
                    intList = strArray.Select(fMgCache0).ToList();
                }
                else
                    intList = new List<int>();
                CompletedMissionDesignIds = intList;
            }
        }

        [XmlAttribute]
        public string LanguageKey { get; set; }

        [XmlAttribute]
        public int TutorialStatus { get; set; }

        [XmlAttribute]
        public int IconSpriteId { get; set; }

        [XmlAttribute]
        public int TipStatus { get; set; }

        [XmlIgnore]
        public AllianceMembership AllianceMembership { get; set; }

        [XmlAttribute("AllianceMembership")]
        public string AllianceMembershipString
        {
            get => AllianceMembership.ToString();
            set => AllianceMembership = !Enum.IsDefined(typeof(AllianceMembership), value) ? AllianceMembership.None : (AllianceMembership)Enum.Parse(typeof(AllianceMembership), value);
        }

        [XmlAttribute]
        public int CrewDonated { get; set; }

        [XmlAttribute]
        public int CrewReceived { get; set; }

        [XmlAttribute]
        public int FreeStarbuxReceivedToday { get; set; }

        [XmlAttribute]
        public int DailyRewardStatus { get; set; }

        [XmlAttribute]
        public int DailyChallengeWinStreak { get; set; }

        [XmlAttribute]
        public int HeroBonusChance { get; set; }

        [XmlAttribute]
        public int GameCenterFriendCount { get; set; }

        [XmlIgnore]
        public List<int> CompletedMissionEventIds { get; set; }

        [XmlAttribute("CompletedMissionEventIds")]
        public string CompletedMissionEventIdsString
        {
            get
            {
                string str = string.Empty;
                if (CompletedMissionEventIds != null)
                {
                    for (int index = 0; index < CompletedMissionEventIds.Count; ++index)
                        str = str + CompletedMissionEventIds[index] + (index == CompletedMissionEventIds.Count - 1 ? string.Empty : (object)",");
                }
                return str;
            }
            set
            {
                List<int> intList;
                if (value != string.Empty)
                {
                    string[] strArray = value.Replace(" ", string.Empty).Split(',');
                    if (PSUser.\u003C\u003Ef__mg\u0024cache1 == null)
            PSUser.\u003C\u003Ef__mg\u0024cache1 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache1 = PSUser.\u003C\u003Ef__mg\u0024cache1;
                    intList = strArray.Select(fMgCache1).ToList();
                }
                else
                    intList = new List<int>();
                CompletedMissionEventIds = intList;
            }
        }

        [XmlIgnore]
        public List<ShipDesign> UnlockedShipDesigns
        {
            get
            {
                if (_unlockedShipDesigns == null || _unlockedShipDesigns.Count != _unlockedShipDesignIds.Count)
                {
                    _unlockedShipDesigns = new List<ShipDesign>();
                    foreach (int shipDesignId in _unlockedShipDesignIds.ToArray())
                    {
                        ShipDesign shipDesignById = SingletonManager<ShipManager>.Instance.GetShipDesignById(shipDesignId);
                        if (shipDesignById != null)
                            _unlockedShipDesigns.Add(shipDesignById);
                    }
                }
                return _unlockedShipDesigns;
            }
            set
            {
                _unlockedShipDesigns = value;
            }
        }

        [XmlAttribute]
        public string UnlockedShipDesignIds
        {
            get
            {
                string str = string.Empty;
                for (int index = 0; index < _unlockedShipDesignIds.Count; ++index)
                    str = str + _unlockedShipDesignIds[index] + (index == _unlockedShipDesignIds.Count - 1 ? string.Empty : (object)",");
                return str;
            }
            set
            {
                string str = value.Trim().Replace(" ", string.Empty).Replace(",,", ",");
                List<int> intList;
                if (str != string.Empty)
                {
                    string[] strArray = str.Split(',');
                    if (PSUser.\u003C\u003Ef__mg\u0024cache2 == null)
            PSUser.\u003C\u003Ef__mg\u0024cache2 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache2 = PSUser.\u003C\u003Ef__mg\u0024cache2;
                    intList = strArray.Select(fMgCache2).ToList();
                }
                else
                    intList = new List<int>();
                _unlockedShipDesignIds = intList;
            }
        }

        [XmlIgnore]
        public List<CharacterDesign> UnlockedCharacterDesigns
        {
            get
            {
                if (_unlockedCharacterDesigns == null || _unlockedCharacterDesigns.Count != _unlockedCharacterDesignIds.Count)
                {
                    _unlockedCharacterDesigns = new List<CharacterDesign>();
                    foreach (int designID in _unlockedCharacterDesignIds.ToArray())
                        _unlockedCharacterDesigns.Add(SingletonManager<CharacterManager>.Instance.GetCharacterDesignByID(designID));
                }
                return _unlockedCharacterDesigns;
            }
            set
            {
                _unlockedCharacterDesigns = value;
            }
        }

        [XmlAttribute]
        public string UnlockedCharacterDesignIds
        {
            get
            {
                string str = string.Empty;
                for (int index = 0; index < _unlockedCharacterDesignIds.Count; ++index)
                    str = str + _unlockedCharacterDesignIds[index] + (index == _unlockedCharacterDesignIds.Count - 1 ? string.Empty : (object)",");
                return str;
            }
            set
            {
                List<int> intList;
                if (value != string.Empty)
                {
                    string[] strArray = value.Replace(" ", string.Empty).Split(',');
                    if (PSUser.\u003C\u003Ef__mg\u0024cache3 == null)
            PSUser.\u003C\u003Ef__mg\u0024cache3 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache3 = PSUser.\u003C\u003Ef__mg\u0024cache3;
                    intList = strArray.Select(fMgCache3).ToList();
                }
                else
                    intList = new List<int>();
                _unlockedCharacterDesignIds = intList;
            }
        }

        [XmlIgnore]
        public SpriteDesign AllianceSprite
        {
            get
            {
                if (_allianceSprite == null || AllianceSpriteId != _allianceSprite.SpriteId)
                    _allianceSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(AllianceSpriteId);
                return _allianceSprite;
            }
        }

        [XmlAttribute]
        public int AllianceSpriteId { get; set; }

        [XmlIgnore]
        public int AllianceRanking
        {
            get
            {
                if (AllianceId == 0 || !SingletonManager<TournamentManager>.Instance.IsLastTournamentPhase || SingletonManager<AllianceManager>.Instance.topAlliances.Count == 0)
                    return 0;
                List<PSAlliance> source = new List<PSAlliance>((IEnumerable<PSAlliance>)SingletonManager<AllianceManager>.Instance.topAlliances);
                foreach (PSAlliance psAlliance in source.ToArray())
                {
                    if (psAlliance.DivisionDesignId == 0)
                        source.Remove(psAlliance);
                }
                List<PSAlliance> list = source.OrderByDescending(o => o.Score).ToList().OrderBy(o => o.DivisionDesignId).ToList();
                int num = 0;
                for (int index = 0; index < list.Count; ++index)
                {
                    if (list[index].AllianceId == AllianceId)
                        num = index + 1;
                }
                return num;
            }
        }

        [XmlAttribute("Ranking")]
        public int UserRanking { get; set; }

        [XmlIgnore]
        public DateTime LastPurchaseDate
        {
            get
            {
                return _lastPurchaseDate;
            }
            set
            {
                _lastPurchaseDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastPurchaseDate")]
        public string LastPurchaseDateString
        {
            get
            {
                if (LastPurchaseDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastPurchaseDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastPurchaseDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastCatalogPurchaseDate
        {
            get
            {
                return _lastCatalogPurchaseDate;
            }
            set
            {
                _lastCatalogPurchaseDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastCatalogPurchaseDate")]
        public string LastCatalogPurchaseDateString
        {
            get
            {
                if (LastCatalogPurchaseDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastCatalogPurchaseDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastCatalogPurchaseDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastHeartBeatDate
        {
            get
            {
                return _lastHeartBeatDate;
            }
            set
            {
                _lastHeartBeatDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastHeartBeatDate")]
        public string LastHeartBeatDateString
        {
            get
            {
                if (LastHeartBeatDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastHeartBeatDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastHeartBeatDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastRewardActionDate
        {
            get
            {
                return _lastRewardActionDate;
            }
            set
            {
                _lastRewardActionDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastRewardActionDate")]
        public string LastRewardActionDateString
        {
            get
            {
                if (LastRewardActionDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastRewardActionDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastRewardActionDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public bool IsVIP
        {
            get
            {
                return SingletonManager<UserManager>.Instance.user.VipExpiryDate > Singleton<SharedManager>.Instance.CurrentTime(false);
            }
        }

        [XmlIgnore]
        public DateTime VipExpiryDate
        {
            get
            {
                return _vipExpiryDate;
            }
            set
            {
                _vipExpiryDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("VipExpiryDate")]
        public string VipExpiryDateString
        {
            get
            {
                if (VipExpiryDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(VipExpiryDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                VipExpiryDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime FacebookTokenExpiryDate
        {
            get
            {
                return _facebookTokenExpiryDate;
            }
            set
            {
                _facebookTokenExpiryDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("FacebookTokenExpiryDate")]
        public string FacebookTokenExpiryDateString
        {
            get
            {
                if (FacebookTokenExpiryDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(FacebookTokenExpiryDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                FacebookTokenExpiryDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                _creationDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("CreationDate")]
        public string CreationDateString
        {
            get
            {
                if (CreationDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(CreationDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                CreationDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastLoginDate
        {
            get
            {
                return _lastLoginDate;
            }
            set
            {
                _lastLoginDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastLoginDate")]
        public string LastLoginDateString
        {
            get
            {
                if (LastLoginDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastLoginDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastLoginDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastAlertDate
        {
            get
            {
                return _lastAlertDate;
            }
            set
            {
                _lastAlertDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastAlertDate")]
        public string LastAlertDateString
        {
            get
            {
                if (LastAlertDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastAlertDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastAlertDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime LastCollectedStarbuxDate
        {
            get
            {
                return _lastCollectedStarbuxDate;
            }
            set
            {
                _lastCollectedStarbuxDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("LastCollectedStarbuxDate")]
        public string LastCollectedStarbuxDateString
        {
            get
            {
                if (LastCollectedStarbuxDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(LastCollectedStarbuxDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                LastCollectedStarbuxDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public int AllianceId { get; set; }

        [XmlAttribute]
        public string AllianceName
        {
            get
            {
                return _allianceName;
            }
            set
            {
                _allianceName = Singleton<SharedManager>.Instance.StripUnsupportedUnicodeChars(value);
            }
        }

        [XmlAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = Singleton<SharedManager>.Instance.StripUnsupportedUnicodeChars(value);
            }
        }

        [XmlAttribute]
        public string GameCenterId { get; set; }

        [XmlAttribute]
        public string FacebookUsername { get; set; }

        [XmlAttribute]
        public string TwitterId { get; set; }

        [XmlAttribute]
        public string TwitterUsername { get; set; }

        [XmlAttribute]
        public string TwitterToken { get; set; }

        [XmlAttribute]
        public string Email { get; set; }

        [XmlAttribute]
        public string LastIP { get; set; }

        [XmlAttribute]
        public int ChallengeDesignId { get; set; }

        [XmlAttribute]
        public int LastChallengeDesignId { get; set; }

        [XmlAttribute]
        public int ChallengeWins { get; set; }

        [XmlAttribute]
        public int ChallengeLosses { get; set; }

        [XmlAttribute]
        public int AllianceSupplyDonation { get; set; }

        [XmlAttribute]
        public int TotalSupplyDonation { get; set; }

        public bool HasUnlockedShipDesign(int shipDesignId)
        {
            if (shipDesignId == 0)
                return true;
            foreach (ShipDesign shipDesign in UnlockedShipDesigns.ToArray())
            {
                if (shipDesign.ShipDesignId == shipDesignId)
                    return true;
            }
            return false;
        }

        [XmlIgnore]
        public Dictionary<int, int> DailyMissionsAttempted { get; set; }

        [XmlAttribute("DailyMissionsAttempted")]
        public string DailyMissionsAttemptedString
        {
            get
            {
                string str = string.Empty;
                foreach (KeyValuePair<int, int> keyValuePair in DailyMissionsAttempted)
                {
                    for (int index = 0; index < keyValuePair.Value; ++index)
                        str = str + keyValuePair.Key + (index == keyValuePair.Value - 1 ? string.Empty : (object)",");
                }
                return str;
            }
            set
            {
                List<int> intList1;
                if (value != string.Empty)
                {
                    string[] strArray = value.Replace(" ", string.Empty).Split(',');
                    if (PSUser.\u003C\u003Ef__mg\u0024cache4 == null)
            PSUser.\u003C\u003Ef__mg\u0024cache4 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache4 = PSUser.\u003C\u003Ef__mg\u0024cache4;
                    intList1 = strArray.Select(fMgCache4).ToList();
                }
                else
                    intList1 = new List<int>();
                List<int> intList2 = intList1;
                DailyMissionsAttempted = new Dictionary<int, int>();
                foreach (int key in intList2)
                {
                    if (DailyMissionsAttempted.ContainsKey(key))
                    {
                        Dictionary<int, int> missionsAttempted;
                        int index;
                        (missionsAttempted = DailyMissionsAttempted)[index = key] = missionsAttempted[index] + 1;
                    }
                    else
                        DailyMissionsAttempted.Add(key, 1);
                }
            }
        }

        [XmlIgnore]
        public DateTime CooldownExpiryDate
        {
            get
            {
                return _cooldownExpiryDate;
            }
            set
            {
                _cooldownExpiryDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("CooldownExpiry")]
        public string CooldownExpiryDateString
        {
            get
            {
                if (CooldownExpiryDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(CooldownExpiryDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                CooldownExpiryDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime AllianceJoinDate
        {
            get
            {
                return _allianceJoinDate;
            }
            set
            {
                _allianceJoinDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("AllianceJoinDate")]
        public string AllianceJoinDateString
        {
            get
            {
                if (AllianceJoinDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(AllianceJoinDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                AllianceJoinDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime BlockAuthAttemptsUntilDate
        {
            get
            {
                return _blockAuthAttemptsUntilDate;
            }
            set
            {
                _blockAuthAttemptsUntilDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("BlockAuthAttemptsUntilDate")]
        public string BlockAuthAttemptsUntilDateString
        {
            get
            {
                if (BlockAuthAttemptsUntilDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(BlockAuthAttemptsUntilDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                BlockAuthAttemptsUntilDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public int ChampionshipScore { get; set; }

        [XmlAttribute]
        public int PurchaseRewardPoints { get; set; }

        [XmlAttribute]
        public int AllianceScore { get; set; }

        [XmlAttribute]
        public int Alignment { get; set; }

        [XmlAttribute]
        public string ActivatedPromotions { get; set; }

        [XmlAttribute]
        public int TournamentRewardPoints { get; set; }

        [XmlAttribute]
        public int TotalRewardPoints { get; set; }

        [XmlAttribute]
        public int UsedRewardPoints { get; set; }

        [XmlAttribute]
        public string GooglePlayName { get; set; }

        [XmlAttribute]
        public string GooglePlayIdToken { get; set; }

        [XmlAttribute]
        public string GooglePlayAuthCode { get; set; }

        [XmlAttribute("Status")]
        public int UserStatus { get; set; }

        [XmlIgnore]
        public ShipDesign ShipDesign
        {
            get
            {
                if (_shipDesign == null || ShipDesignId != 0 && _shipDesign.ShipDesignId != ShipDesignId)
                {
                    if (ShipDesignId != 0)
                    {
                        _shipDesign = SingletonManager<ShipManager>.Instance.GetShipDesignById(ShipDesignId);
                    }
                    else
                    {
                        PSUser user = SingletonManager<UserManager>.Instance.user;
                        int? nullable = user != null ? user.Id : new int?();
                        if ((nullable.GetValueOrDefault() != Id ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
                            _shipDesign = SingletonManager<ShipManager>.Instance.PlayerShip.ShipDesign;
                    }
                }
                return _shipDesign;
            }
        }

        [XmlAttribute]
        public int ShipDesignId { get; set; }

        [XmlAttribute]
        public int DrawsUsedToday { get; set; }

        [XmlAttribute]
        public int PVPAttackWins { get; set; }

        [XmlAttribute]
        public int PVPAttackLosses { get; set; }

        [XmlAttribute]
        public int PVPAttackDraws { get; set; }

        [XmlAttribute]
        public int PVPDefenceDraws { get; set; }

        [XmlAttribute]
        public int PVPDefenceWins { get; set; }

        [XmlAttribute]
        public int PVPDefenceLosses { get; set; }

        [XmlAttribute]
        public int HighestTrophy { get; set; }

        [XmlAttribute]
        public int AllianceQualifyDivisionDesignId { get; set; }

        [XmlIgnore]
        public AuthenticationType AuthenticationType { get; set; }

        [XmlAttribute("AuthenticationType")]
        public string AuthenticationTypeString
        {
            get
            {
                return AuthenticationType.ToString();
            }
            set
            {
                AuthenticationType = !Enum.IsDefined(typeof(AuthenticationType), value) ? AuthenticationType.Basic : (AuthenticationType)Enum.Parse(typeof(AuthenticationType), value);
            }
        }

        [XmlElement]
        public PSAlliance Alliance
        {
            get
            {
                return _alliance;
            }
            set
            {
                _alliance = value;
            }
        }
    }
}
