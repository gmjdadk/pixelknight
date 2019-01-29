using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using PixelKnight.Utils;

namespace PixelKnight.Models
{
    [XmlRoot("Ship")]
    [Serializable]
    public class PSMainShip : PSShip, ShipInterface, ShipManagementInterface
    {
        private ObservableCollection<PSMainRoom> _PSMainRooms;
        private ObservableCollection<PSMainCharacter> _PSMainCharacters;
        private ObservableCollection<PSMainItem> _PSMainItems;
        private PSRoom[] _Rooms;
        private PSCharacter[] _Characters;
        private PSItem[] _Items;

        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public ObservableCollection<PSMainRoom> PSMainRooms
        {
            get
            {
                return this._PSMainRooms;
            }
            set
            {
                this._PSMainRooms = value;
                this._PSMainRooms.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Rooms_CollectionChanged);
            }
        }

        [XmlArray("Characters")]
        [XmlArrayItem("Character")]
        public ObservableCollection<PSMainCharacter> PSMainCharacters
        {
            get
            {
                return this._PSMainCharacters;
            }
            set
            {
                this._PSMainCharacters = value;
                this._PSMainCharacters.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Characters_CollectionChanged);
            }
        }

        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public ObservableCollection<PSMainItem> PSMainItems
        {
            get
            {
                return this._PSMainItems;
            }
            set
            {
                this._PSMainItems = value;
                this._PSMainItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
            }
        }

        [XmlIgnore]
        public override PSRoom[] Rooms
        {
            get
            {
                if (this._Rooms == null)
                    this._Rooms = this.PSMainRooms.Cast<PSRoom>().ToArray<PSRoom>();
                return this._Rooms;
            }
        }

        private void Rooms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._Rooms = (PSRoom[])null;
        }

        [XmlIgnore]
        public override PSCharacter[] Characters
        {
            get
            {
                if (this._Characters == null)
                    this._Characters = this.PSMainCharacters.Cast<PSCharacter>().ToArray<PSCharacter>();
                return this._Characters;
            }
        }

        private void Characters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._Characters = (PSCharacter[])null;
        }

        [XmlIgnore]
        public override PSItem[] Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = this.PSMainItems.Cast<PSItem>().ToArray<PSItem>();
                return this._Items;
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this._Items = (PSItem[])null;
        }

        public override void MapFrom(PSShip psShip)
        {
            this._Rooms = psShip.Rooms;
            this._Characters = psShip.Characters;
            this._Items = psShip.Items;
            base.MapFrom(psShip);
        }

        [XmlIgnore]
        public List<PSCharacter> DonatedOwnedCrew
        {
            get
            {
                List<PSCharacter> psCharacterList = new List<PSCharacter>();
                for (int index = 0; index < this.Characters.Length; ++index)
                {
                    PSCharacter character = this.Characters[index];
                    if (!character.DeploymentExpired && character.OwnerShipId == this.ShipId)
                        psCharacterList.Add(character);
                }
                return psCharacterList;
            }
        }

        public int MaximumAICommands
        {
            get
            {
                int num = 0;
                for (int index = 0; index < this.Rooms.Length; ++index)
                {
                    PSRoom room = this.Rooms[index];
                    if ((room.RoomStatus == RoomStatus.Normal || room.RoomStatus == RoomStatus.Upgrading) && room.RoomType == RoomType.Command)
                        num += room.RoomDesign.Capacity;
                }
                return num;
            }
        }

        public int CountPlacedRoomsOfType(RoomType type)
        {
            int num = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomStatus != RoomStatus.Inventory && room.RoomType == type)
                    ++num;
            }
            return num;
        }

        public bool CheckCosmeticItemIsUsed(int itemDesignId)
        {
            if (this.SkinItemDesignId == itemDesignId)
                return true;
            for (int index = 0; index < this.UsedStickers.Count; ++index)
            {
                if (this.UsedStickers[index].itemDesignId == itemDesignId)
                    return true;
            }
            return false;
        }

        public int GetMaxCapacityByRoomDesign(RoomDesign roomDesign)
        {
            int num = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && (room.RoomDesign.ItemRank == roomDesign.ItemRank && room.RoomDesign.ManufactureType == roomDesign.ManufactureType))
                    num += room.RoomDesign.Capacity;
            }
            return num;
        }

        public int GetTotalItemsQueuedStorageUsedByItemDesign(ItemDesign itemDesign)
        {
            int num = 0;
            for (int index1 = 0; index1 < this.Rooms.Length; ++index1)
            {
                PSMainRoom room = this.Rooms[index1] as PSMainRoom;
                if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Inventory && (room.RoomDesign.ItemRank == itemDesign.Rank && room.RoomDesign.ManufactureType == itemDesign.ItemType) && room.ManufactureString != string.Empty)
                {
                    List<PSResourceGroup> psResourceGroupList = new List<PSResourceGroup>();
                    for (int index2 = 0; index2 < room.ManufactureItemQueue.Count; ++index2)
                    {
                        PSResourceGroup manufactureItem = room.ManufactureItemQueue[index2];
                        if (!psResourceGroupList.Contains(manufactureItem))
                            psResourceGroupList.Add(manufactureItem);
                    }
                    for (int index2 = 0; index2 < psResourceGroupList.Count; ++index2)
                    {
                        PSResourceGroup psResourceGroup = psResourceGroupList[index2];
                        ItemDesign itemDesignById = SingletonManager<ItemManager>.Instance.GetItemDesignByID(psResourceGroup.resourceId);
                        num += itemDesignById.ItemSpace * psResourceGroup.quantity;
                    }
                }
            }
            return num;
        }

        public int GetTotalItemsStorageUsedByItemDesign(ItemDesign itemDesign)
        {
            int num = 0;
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemType == itemDesign.ItemType && psItem.ItemDesign.Rank == itemDesign.Rank && (psItem.ItemDesign.RequiredResearchDesignId == 0 || psItem.ItemDesign.RequiredPsResearch != null && psItem.ItemDesign.RequiredPsResearch.ResearchState == ResearchState.Completed))
                    num += psItem.ItemDesign.ItemSpace * psItem.Quantity;
            }
            return num;
        }

        public int GetMaxCapacityByItemDesign(ItemDesign itemDesign)
        {
            int num = 0;
            for (int index = 0; index < this.Rooms.Length; ++index)
            {
                PSRoom room = this.Rooms[index];
                if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Building && (room.RoomDesign.ItemRank == itemDesign.Rank && room.RoomDesign.ManufactureType == itemDesign.ItemType))
                    num += room.RoomDesign.Capacity;
            }
            return num;
        }

        public int GetTotalItemsQueuedStorageUsedByRoomDesign(RoomDesign roomDesign)
        {
            int num = 0;
            for (int index1 = 0; index1 < this.Rooms.Length; ++index1)
            {
                PSMainRoom room = this.Rooms[index1] as PSMainRoom;
                if (room.RoomStatus != RoomStatus.Inventory && room.RoomStatus != RoomStatus.Inventory && (room.RoomDesign.ItemRank == roomDesign.ItemRank && room.RoomDesign.ManufactureType == roomDesign.ManufactureType) && room.ManufactureString != string.Empty)
                {
                    List<PSResourceGroup> psResourceGroupList = new List<PSResourceGroup>();
                    for (int index2 = 0; index2 < room.ManufactureItemQueue.Count; ++index2)
                    {
                        PSResourceGroup manufactureItem = room.ManufactureItemQueue[index2];
                        if (!psResourceGroupList.Contains(manufactureItem))
                            psResourceGroupList.Add(manufactureItem);
                    }
                    for (int index2 = 0; index2 < psResourceGroupList.Count; ++index2)
                    {
                        PSResourceGroup psResourceGroup = psResourceGroupList[index2];
                        ItemDesign itemDesignById = SingletonManager<ItemManager>.Instance.GetItemDesignByID(psResourceGroup.resourceId);
                        num += itemDesignById.ItemSpace * psResourceGroup.quantity;
                    }
                }
            }
            return num;
        }

        public int GetTotalItemStorageUsedByRoomDesign(RoomDesign roomDesign)
        {
            int num = 0;
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemType == roomDesign.ManufactureType && psItem.ItemDesign.MinRoomLevel <= roomDesign.Level && psItem.ItemDesign.Rank == roomDesign.ItemRank && (psItem.ItemDesign.RequiredResearchDesignId == 0 || psItem.ItemDesign.RequiredPsResearch != null && psItem.ItemDesign.RequiredPsResearch.ResearchState == ResearchState.Completed))
                    num += psItem.ItemDesign.ItemSpace * psItem.Quantity;
            }
            return num;
        }

        public int GetTotalItemsByItemType(ItemType itemType)
        {
            int num = 0;
            List<PSItem> itemsBelongingToShip = this.ItemsBelongingToShip;
            for (int index = 0; index < itemsBelongingToShip.Count; ++index)
            {
                PSItem psItem = itemsBelongingToShip[index];
                if (psItem.ItemDesign.ItemType == itemType && (psItem.ItemDesign.RequiredResearchDesignId == 0 || psItem.ItemDesign.RequiredPsResearch != null && psItem.ItemDesign.RequiredPsResearch.ResearchState == ResearchState.Completed))
                    num += psItem.Quantity;
            }
            return num;
        }

        public override PSItem AddItemToShip(PSItem item)
        {
            PSItem psItem = (PSItem)null;
            try
            {
                psItem = (PSItem)this.PSMainItems.First<PSMainItem>((Func<PSMainItem, bool>)(x => x.ItemId == item.ItemId));
            }
            catch
            {
            }
            if (psItem != null)
            {
                psItem.ItemDesignId = item.ItemDesignId;
                psItem.Quantity = item.Quantity;
                psItem.ShipId = item.ShipId;
            }
            else
            {
                this.DeleteParentItem(item);
                this.PSMainItems.Add(item as PSMainItem);
            }
            return psItem;
        }

        public override void DeleteParentItem(PSItem item)
        {
            try
            {
                this.PSMainItems.Remove(this.PSMainItems.First<PSMainItem>((Func<PSMainItem, bool>)(x =>
                {
                    if (x.ShipId == item.ShipId)
                        return item.ItemDesign.ParentItemDesignId == x.ItemDesignId;
                    return false;
                })));
            }
            catch
            {
            }
        }
    }
}