using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;

namespace PixelKnight.Models
{
    [XmlRoot("Room")]
    [Serializable]
    public class PSRoom : PSObject, ICloneable
    {
        public static readonly string ALL_ROOMS_CACHE = "allRooms";
        [XmlIgnore]
        [NonSerialized]
        public List<Dictionary<string, PSObject>> setPowerActionList = new List<Dictionary<string, PSObject>>();
        [XmlIgnore]
        [NonSerialized]
        public List<Dictionary<string, PSObject>> setItemActionList = new List<Dictionary<string, PSObject>>();
        [XmlIgnore]
        [NonSerialized]
        public List<Dictionary<string, PSObject>> targetActionList = new List<Dictionary<string, PSObject>>();
        [XmlArray("RoomActions")]
        [XmlArrayItem("RoomAction")]
        public List<PSRoomAction> RoomActions = new List<PSRoomAction>();
        [NonSerialized]
        private DateTime _manufactureStartDate;
        [NonSerialized]
        private DateTime _constructionStartDate;
        [NonSerialized]
        private RoomDesign _roomDesign;
        [NonSerialized]
        private int _roomDesignId;
        [NonSerialized]
        private RoomStatus _roomStatus;
        [NonSerialized]
        private double _progress;
        [NonSerialized]
        protected List<PSItem> _moduleItems;
        [NonSerialized]
        protected List<int> _originalModuleItemIds;
        [NonSerialized]
        protected MissileDesign _missileDesign;
        [XmlIgnore]
        [NonSerialized]
        public bool locked;
        [XmlIgnore]
        [NonSerialized]
        public bool destroyed;
        [XmlIgnore]
        [NonSerialized]
        public bool constructing;
        [XmlIgnore]
        [NonSerialized]
        public int defenceEnhance;
        private RoomDesign _upgradeToRoomDesign;
        protected List<int> _moduleItemIds;

        public Vector3 ExternalAngles { get; set; }

        [XmlIgnore]
        public RoomDesign RoomDesign
        {
            get
            {
                if (this._roomDesign == null)
                    this._roomDesign = SingletonManager<RoomManager>.Instance.GetRoomDesignByID(this.RoomDesignId);
                return this._roomDesign;
            }
        }

        [XmlIgnore]
        public double Hp { get; set; }

        [XmlIgnore]
        public double MaxHp
        {
            get
            {
                return this.RoomDesign.RoomType != RoomType.Reactor || this.RoomStatus != RoomStatus.Building ? (double)(this.RoomDesign.MaxSystemPower + this.RoomDesign.MaxPowerGenerated) : 0.0;
            }
        }

        [XmlIgnore]
        public RoomType RoomType
        {
            get
            {
                return this.RoomDesign.RoomType;
            }
        }

        [XmlIgnore]
        public string RoomName
        {
            get
            {
                return this.RoomDesign.RoomName;
            }
        }

        [XmlIgnore]
        public int Level
        {
            get
            {
                return this.RoomDesign.Level;
            }
        }

        public RoomDesign UpgradeToRoomDesign
        {
            get
            {
                if (this._upgradeToRoomDesign == null)
                    this._upgradeToRoomDesign = this._roomStatus != RoomStatus.Building ? SingletonManager<RoomManager>.Instance.GetRoomDesignByID(this.UpgradeRoomDesignId) : this.RoomDesign;
                return this._upgradeToRoomDesign;
            }
            set
            {
                this._upgradeToRoomDesign = value;
            }
        }

        [XmlIgnore]
        public double ConstructionTimeElapsed
        {
            get
            {
                return (Singleton<SharedManager>.Instance.CurrentTime(true) - this.ConstructionStartDate).TotalSeconds;
            }
        }

        [XmlIgnore]
        public double ConstructionTimeRemaining
        {
            get
            {
                if (this.UpgradeToRoomDesign != null)
                    return (double)this.UpgradeToRoomDesign.ConstructionTime - this.ConstructionTimeElapsed;
                return 0.0;
            }
        }

        [XmlIgnore]
        public int MaxPowerGeneratable
        {
            get
            {
                if (this.RoomDesign.RefillUnitCost > 0)
                    return Mathf.Min(this.CapacityUsed * 100, this.RoomDesign.MaxPowerGenerated);
                return this.RoomDesign.MaxPowerGenerated;
            }
        }

        [XmlIgnore]
        public int TotalDefenceBonus
        {
            get
            {
                return this.defenceEnhance + this.RoomDesign.DefaultDefenceBonus;
            }
        }

        [XmlAttribute]
        public int RoomId { get; set; }

        [XmlAttribute]
        public int RoomDesignId
        {
            get
            {
                return this._roomDesignId;
            }
            set
            {
                this._roomDesignId = value;
                this._roomDesign = SingletonManager<RoomManager>.Instance.GetRoomDesignByID(value);
            }
        }

        [XmlAttribute]
        public int ShipId { get; set; }

        [XmlIgnore]
        public RoomStatus RoomStatus
        {
            get
            {
                RoomStatus roomStatus = this._roomStatus;
                switch (roomStatus)
                {
                    case RoomStatus.Building:
                    case RoomStatus.Upgrading:
                        if (this.ConstructionTimeRemaining <= 0.0)
                        {
                            roomStatus = RoomStatus.Normal;
                            break;
                        }
                        break;
                }
                return roomStatus;
            }
            set
            {
                this._roomStatus = value;
            }
        }

        [XmlAttribute("RoomStatus")]
        public string RoomStatusString
        {
            get
            {
                return this.RoomStatus.ToString();
            }
            set
            {
                if (Enum.IsDefined(typeof(RoomStatus), (object)value))
                    this.RoomStatus = (RoomStatus)Enum.Parse(typeof(RoomStatus), value);
                else
                    this.RoomStatus = RoomStatus.Building;
            }
        }

        [XmlAttribute]
        public int Row { get; set; }

        [XmlAttribute]
        public int Column { get; set; }

        [XmlIgnore]
        public DateTime ManufactureStartDate
        {
            get
            {
                return this._manufactureStartDate;
            }
            set
            {
                this._manufactureStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("ManufactureStartDate")]
        public string ManufactureStartDateString
        {
            get
            {
                if (this.ManufactureStartDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.ManufactureStartDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.ManufactureStartDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public DateTime ConstructionStartDate
        {
            get
            {
                return this._constructionStartDate;
            }
            set
            {
                this._constructionStartDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("ConstructionStartDate")]
        public string ConstructionStartDateString
        {
            get
            {
                if (this.ConstructionStartDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.ConstructionStartDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.ConstructionStartDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlAttribute]
        public int RandomSeed { get; set; }

        [XmlAttribute]
        public int Manufactured { get; set; }

        [XmlAttribute]
        public int UpgradeRoomDesignId { get; set; }

        [XmlAttribute]
        public int CapacityUsed { get; set; }

        [XmlAttribute]
        public int TargetRoomId { get; set; }

        [XmlAttribute]
        public int TargetCraftId { get; set; }

        [XmlAttribute]
        public int AssignedPower { get; set; }

        [XmlAttribute]
        public int SystemPower { get; set; }

        [XmlAttribute]
        public int PowerGenerated { get; set; }

        [XmlAttribute]
        public int TotalDamage { get; set; }

        [XmlAttribute]
        public double Progress
        {
            get
            {
                return this._progress;
            }
            set
            {
                this._progress = Math.Round(value, 5, MidpointRounding.AwayFromZero);
            }
        }

        [XmlAttribute]
        public double CenterX { get; set; }

        [XmlAttribute]
        public double CenterY { get; set; }

        [XmlAttribute("ItemIds")]
        public string ModuleItemIdsString
        {
            get
            {
                string str = string.Empty;
                if (this._moduleItemIds != null)
                {
                    for (int index = 0; index < this._moduleItemIds.Count; ++index)
                        str = str + (object)this._moduleItemIds[index] + (index == this._moduleItemIds.Count - 1 ? (object)string.Empty : (object)",");
                }
                return str;
            }
            set
            {
                List<int> intList;
                if (value != string.Empty)
                {
                    string[] strArray = value.Replace(" ", string.Empty).Split(',');
                    if (PSRoom.\u003C\u003Ef__mg\u0024cache0 == null)
            PSRoom.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache0 = PSRoom.\u003C\u003Ef__mg\u0024cache0;
                    intList = ((IEnumerable<string>)strArray).Select<string, int>(fMgCache0).ToList<int>();
                }
                else
                    intList = new List<int>();
                this._moduleItemIds = intList;
            }
        }

        [XmlAttribute]
        public string SalvageString { get; set; }

        [XmlIgnore]
        public virtual List<PSItem> ModuleItems
        {
            get
            {
                return this._moduleItems;
            }
            set
            {
                this._moduleItems = value;
                this._moduleItemIds.Clear();
                for (int index = 0; index < this._moduleItems.Count; ++index)
                    this._moduleItemIds.Add(this._moduleItems[index].ItemId);
            }
        }

        [XmlIgnore]
        public virtual MissileDesign MissileDesign
        {
            get
            {
                if (this._missileDesign == null)
                    this._missileDesign = SingletonManager<RoomManager>.Instance.GetMissileDesignById(this.RoomDesign.MissileDesignId);
                return this._missileDesign;
            }
            set
            {
                this._missileDesign = value;
            }
        }

        [XmlAttribute]
        public string ManufactureString { get; set; }

        [XmlAttribute]
        public string TargetManufactureString { get; set; }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }

        private object DeepCopy()
        {
            return (object)(this.ShallowCopy() as PSRoom);
        }

        public object Clone()
        {
            return this.DeepCopy();
        }

        public void ArrangeActions()
        {
            List<PSRoomAction> list = this.RoomActions.ToList<PSRoomAction>();
            list.Sort((Comparison<PSRoomAction>)((x, y) => x.RoomActionIndex.CompareTo(y.RoomActionIndex)));
            this.targetActionList.Clear();
            this.setPowerActionList.Clear();
            this.setItemActionList.Clear();
            if (list.Count > 0)
            {
                for (int index = 0; index < list.Count; ++index)
                {
                    PSRoomAction psRoomAction = list[index];
                    ActionTypeDesign actionTypeById = SingletonManager<AIManager>.Instance.GetActionTypeByID(psRoomAction.ActionTypeId);
                    ConditionTypeDesign conditionTypeById = SingletonManager<AIManager>.Instance.GetConditionTypeByID(psRoomAction.ConditionTypeId);
                    Dictionary<string, PSObject> dictionary = new Dictionary<string, PSObject>();
                    dictionary.Add("CONDITION_DEFINE_STRING", (PSObject)conditionTypeById);
                    dictionary.Add("ACTION_DEFINE_STRING", (PSObject)actionTypeById);
                    if (actionTypeById.ActionTypeCategory == ActionTypeCategory.Target)
                        this.targetActionList.Add(dictionary);
                    if (actionTypeById.ActionTypeCategory == ActionTypeCategory.SetPower)
                        this.setPowerActionList.Add(dictionary);
                    if (actionTypeById.ActionTypeCategory == ActionTypeCategory.SetItem)
                        this.setItemActionList.Add(dictionary);
                }
            }
            this.RoomActions = list;
        }
    }
}