using System;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;

namespace PixelKnight.Models
{
    [XmlRoot("RoomDesignPurchase")]
    [Serializable]
    public class PSRoomDesignPurchase : PSObject
    {
        public const string ALL_ROOMDESIGNPURCHASES_CACHE = "allRoomDesignPurchases";
        [NonSerialized]
        private int _maxQuantity;
        [NonSerialized]
        private RoomDesign _roomDesign;

        [XmlIgnore]
        public RoomDesign RoomDesign
        {
            get
            {
                if (_roomDesign != null) return _roomDesign;
                foreach (RoomDesign roomDesign in SingletonManager<RoomManager>.Instance.roomDesignList.ToArray())
                {
                    if (RoomDesignId != roomDesign.RoomDesignId) continue;
                    _roomDesign = roomDesign;
                    break;
                }
                return _roomDesign;
            }
        }

        [XmlIgnore]
        public int TotalQuantity
        {
            get
            {
                _maxQuantity = 0;
                int num = SingletonManager<UserManager>.Instance.user.UserType != UserType.UserTypeAlliance ? 1 : 2;
                foreach (PSRoomDesignPurchase roomDesignPurchase in SingletonManager<RoomManager>.Instance.roomDesignPurchaseList.ToArray())
                {
                    if (roomDesignPurchase.RoomDesignId == RoomDesignId && Level >= roomDesignPurchase.Level && (roomDesignPurchase.AvailabilityMask & num) > 0)
                        _maxQuantity += roomDesignPurchase.Quantity;
                }
                return _maxQuantity;
            }
        }

        [XmlAttribute]
        public int RoomDesignPurchaseId { get; set; }

        [XmlAttribute]
        public int RoomDesignId { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        [XmlAttribute]
        public int Quantity { get; set; }

        [XmlAttribute]
        public int AvailabilityMask { get; set; }
    }
}