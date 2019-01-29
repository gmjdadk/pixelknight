using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace PixelKnight.Models
{
    [XmlRoot("Item")]
    [Serializable]
    public class PSItem : PSObject
    {
        public const string ALL_ITEMS_CACHE = "allItems";
        [XmlIgnore]
        private ItemDesign _itemDesign;

        [XmlIgnore]
        public ItemDesign ItemDesign
        {
            get
            {
                if (this._itemDesign == null || this._itemDesign.ItemDesignId != this.ItemDesignId)
                    this._itemDesign = SingletonManager<ItemManager>.Instance.GetItemDesignByID(this.ItemDesignId);
                return this._itemDesign;
            }
        }

        [XmlAttribute]
        public int ItemId { get; set; }

        [XmlAttribute]
        public int ShipId { get; set; }

        [XmlAttribute]
        public int Quantity { get; set; }

        [XmlAttribute]
        public int ItemDesignId { get; set; }

        [XmlIgnore]
        public EnhancementType BonusEnhancementType { get; set; }

        [XmlAttribute("BonusEnhancementType")]
        public string BonusEnhancementTypeString
        {
            get
            {
                return this.BonusEnhancementType.ToString();
            }
            set
            {
                this.BonusEnhancementType = !Enum.IsDefined(typeof(EnhancementType), (object)value) ? EnhancementType.None : (EnhancementType)Enum.Parse(typeof(EnhancementType), value);
            }
        }

        [XmlAttribute]
        public double BonusEnhancementValue { get; set; }
    }
}