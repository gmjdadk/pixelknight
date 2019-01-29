using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PixelKnight.Models
{
    [XmlRoot("RoomAction")]
    [Serializable]
    public class PSRoomAction : PSObject
    {
        [NonSerialized]
        private ConditionTypeDesign _conditionTypeDesign;
        [NonSerialized]
        private ActionTypeDesign _actionTypeDesign;

        [XmlIgnore]
        public ConditionTypeDesign ConditionTypeDesign
        {
            get
            {
                if (this._conditionTypeDesign == null || this._conditionTypeDesign.ConditionTypeId != this.ConditionTypeId)
                    this._conditionTypeDesign = SingletonManager<AIManager>.Instance.GetConditionTypeByID(this.ConditionTypeId);
                return this._conditionTypeDesign;
            }
            set
            {
                this._conditionTypeDesign = value;
            }
        }

        [XmlIgnore]
        public ActionTypeDesign ActionTypeDesign
        {
            get
            {
                if (this._actionTypeDesign == null || this._conditionTypeDesign.ConditionTypeId != this.ActionTypeId)
                    this._actionTypeDesign = SingletonManager<AIManager>.Instance.GetActionTypeByID(this.ActionTypeId);
                return this._actionTypeDesign;
            }
            set
            {
                this._actionTypeDesign = value;
            }
        }

        [XmlAttribute]
        public int RoomActionId { get; set; }

        [XmlAttribute]
        public int RoomId { get; set; }

        [XmlAttribute]
        public int RoomActionIndex { get; set; }

        [XmlAttribute]
        public int ConditionTypeId { get; set; }

        [XmlAttribute]
        public int ActionTypeId { get; set; }
    }
}