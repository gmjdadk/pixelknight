using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelKnight.Models;
using PixelKnight.Utils;

namespace PixelStarships
{
    [Serializable]
    public class ShipDesign : PSObject
    {
        [NonSerialized]
        private string _unlockCostString = string.Empty;
        [NonSerialized]
        private string _upgradeCostString = string.Empty;
        public const string ALL_SHIPDESIGNS_CACHE = "allShipDesigns";
        [NonSerialized]
        private double _maxHp;
        [NonSerialized]
        private int _totalType1Grids;
        [NonSerialized]
        private int _totalType2Grids;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _interiorSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _exteriorSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _shieldSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _shieldActiveSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _doorFrameLeftSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _doorFrameRightSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _roomFrameSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _liftSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _logoSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _miniShipSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _thrustParticleSprite;
        [XmlIgnore]
        [NonSerialized]
        public SpriteDesign _hpBarSprite;
        [XmlIgnore]
        [NonSerialized]
        public ShipDesign _unlockFromShipDesign;

        [XmlAttribute]
        public int ShipDesignId { get; set; }

        [XmlAttribute]
        public string ShipDesignName { get; set; }

        [XmlAttribute]
        public string ShipDescription { get; set; }

        [XmlAttribute]
        public int ShipLevel { get; set; }

        [XmlAttribute]
        public int RequiredShipDesignId { get; set; }

        [XmlAttribute]
        public int Rows { get; set; }

        [XmlAttribute]
        public int Columns { get; set; }

        [XmlAttribute]
        public string Mask { get; set; }

        [XmlAttribute]
        public double Hp
        {
            get
            {
                return this._maxHp;
            }
            set
            {
                this._maxHp = value * 100.0;
            }
        }

        [XmlAttribute]
        public int MineralCost { get; set; }

        [XmlAttribute]
        public int RepairTime { get; set; }

        [XmlAttribute]
        public int ExteriorSpriteId { get; set; }

        [XmlAttribute]
        public int InteriorSpriteId { get; set; }

        [XmlAttribute]
        public int LogoSpriteId { get; set; }

        [XmlAttribute]
        public int UpgradeTime { get; set; }

        [XmlAttribute]
        public int RaceId { get; set; }

        [XmlAttribute]
        public int RoomFrameSpriteId { get; set; }

        [XmlAttribute]
        public int UpgradeOffsetRows { get; set; }

        [XmlAttribute]
        public int UpgradeOffsetColumns { get; set; }

        [XmlAttribute]
        public int LiftSpriteId { get; set; }

        [XmlAttribute]
        public int DoorFrameLeftSpriteId { get; set; }

        [XmlAttribute]
        public int DoorFrameRightSpriteId { get; set; }

        [XmlAttribute]
        public int StarbuxCost { get; set; }

        [XmlAttribute]
        public int EngineX { get; set; }

        [XmlAttribute]
        public int EngineY { get; set; }

        [XmlAttribute]
        public int ThrustParticleSpriteId { get; set; }

        [XmlAttribute]
        public int ThrustLineAnimationId { get; set; }

        [XmlAttribute]
        public int MineralCapacity { get; set; }

        [XmlAttribute]
        public int GasCapacity { get; set; }

        [XmlAttribute]
        public int EquipmentCapacity { get; set; }

        [XmlAttribute]
        public float ThrustScale { get; set; }

        [XmlAttribute]
        public int FlagX { get; set; }

        [XmlAttribute]
        public int FlagY { get; set; }

        [XmlAttribute]
        public int RequiredResearchDesignId { get; set; }

        [XmlAttribute]
        public int MiniShipSpriteId { get; set; }

        [XmlAttribute]
        public bool AllowInteracial { get; set; }

        [XmlAttribute]
        public ShipType ShipType { get; set; }

        [XmlAttribute("UnlockCost")]
        public string UnlockCostString
        {
            get
            {
                return this._unlockCostString;
            }
            set
            {
                this._unlockCostString = value;
                this.UnlockCost = SharedManager.ParseRewardStrings(this._unlockCostString);
            }
        }

        [XmlIgnore]
        public PSResourceGroup[] UnlockCost { get; set; }

        [XmlAttribute("UpgradeCost")]
        public string UpgradeCostString
        {
            get
            {
                return this._upgradeCostString;
            }
            set
            {
                this._upgradeCostString = value;
                this.UpgradeCost = SharedManager.ParseRewardStrings(this._upgradeCostString);
            }
        }

        [XmlIgnore]
        public PSResourceGroup[] UpgradeCost { get; set; }

        [XmlAttribute]
        public int UnlockFromShipDesignId { get; set; }

        [XmlIgnore]
        public ShipDesign UnlockFromShipDesign
        {
            get
            {
                if (this._unlockFromShipDesign == null)
                    this._unlockFromShipDesign = SingletonManager<ShipManager>.Instance.GetShipDesignById(this.UnlockFromShipDesignId);
                return this._unlockFromShipDesign;
            }
        }

        [XmlIgnore]
        public ShipDesign RequiredShipDesign
        {
            get
            {
                if (this._unlockFromShipDesign == null)
                    this._unlockFromShipDesign = SingletonManager<ShipManager>.Instance.GetShipDesignById(this.RequiredShipDesignId);
                return this._unlockFromShipDesign;
            }
        }

        [XmlIgnore]
        public int TotalType1Grids
        {
            get
            {
                if (this._totalType1Grids == 0)
                {
                    string[] array = ((IEnumerable<char>)this.Mask.ToCharArray()).Select<char, string>((Func<char, string>)(c => c.ToString())).ToArray<string>();
                    if (ShipDesign.\u003C\u003Ef__mg\u0024cache0 == null)
            ShipDesign.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache0 = ShipDesign.\u003C\u003Ef__mg\u0024cache0;
                    foreach (int num in ((IEnumerable<string>)array).Select<string, int>(fMgCache0).ToArray<int>())
                    {
                        if (num == 1)
                            ++this._totalType1Grids;
                    }
                }
                return this._totalType1Grids;
            }
        }

        [XmlIgnore]
        public int TotalType2Grids
        {
            get
            {
                if (this._totalType2Grids == 0)
                {
                    string[] array = ((IEnumerable<char>)this.Mask.ToCharArray()).Select<char, string>((Func<char, string>)(c => c.ToString())).ToArray<string>();
                    if (ShipDesign.\u003C\u003Ef__mg\u0024cache1 == null)
            ShipDesign.\u003C\u003Ef__mg\u0024cache1 = new Func<string, int>(int.Parse);
                    Func<string, int> fMgCache1 = ShipDesign.\u003C\u003Ef__mg\u0024cache1;
                    foreach (int num in ((IEnumerable<string>)array).Select<string, int>(fMgCache1).ToArray<int>())
                    {
                        if (num == 2)
                            ++this._totalType2Grids;
                    }
                }
                return this._totalType2Grids;
            }
        }

        [XmlAttribute]
        public string RequirementString { get; set; }
    }
}
