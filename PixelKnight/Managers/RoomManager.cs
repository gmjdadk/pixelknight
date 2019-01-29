using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.UI.WebControls;
using PixelKnight.Enums;
using PixelKnight.Models;
using PixelStarships;
using Image = System.Web.UI.WebControls.Image;

namespace PixelKnight.Managers
{
    public class RoomManager
    {
        public List<RoomDesign> roomDesignList = new List<RoomDesign>();
        public List<MissileDesign> missileDesignList = new List<MissileDesign>();
        public Dictionary<int, List<RoomDesignSprite>> roomDesignSpriteDictionary = new Dictionary<int, List<RoomDesignSprite>>();
        public List<PSRoomDesignPurchase> roomDesignPurchaseList = new List<PSRoomDesignPurchase>();
        public Stack craftDesignStack = new Stack();
        public float doorBackOffsetX = 3f;
        public float doorBackOffsetY = 1f;
        public float doorFrontOffsetX = 1f;
        public float doorFrontOffsetY = 1f;
        public float doorBackOffsetXNew = 6.5f;
        public float doorBackOffsetYNew = 6f;
        public float doorFrontOffsetXNew = 3f;
        public float doorFrontOffsetYNew = 6f;
        public bool downloaded;

        protected RoomManager()
        {
        }

        public bool CanPurchaseRoom(int rootRoomDesignId, int maxQuantity)
        {
            int num = 0;
            foreach (PSRoom room in SingletonManager<ShipManager>.Instance.PlayerShip.Rooms)
            {
                if (room.RoomDesign.RootRoomDesignId == rootRoomDesignId)
                    ++num;
            }
            return num < maxQuantity;
        }

        public RoomDesign GetRoomDesignByID(int roomDesignID)
        {
            foreach (RoomDesign roomDesign in this.roomDesignList.ToArray())
            {
                if (roomDesign.RoomDesignId == roomDesignID)
                    return roomDesign;
            }
            return (RoomDesign)null;
        }

        public RoomDesign GetMaxLevelRoomDesignRecursively(RoomDesign roomDesign)
        {
            RoomDesign roomDesign1 = roomDesign;
            foreach (RoomDesign roomDesign2 in this.roomDesignList.ToArray())
            {
                if (roomDesign2.RootRoomDesignId == roomDesign.RootRoomDesignId && roomDesign2.UpgradeFromRoomDesignId == roomDesign.RoomDesignId && roomDesign2.Level > roomDesign.Level)
                {
                    roomDesign1 = roomDesign2;
                    break;
                }
            }
            if (roomDesign1.RoomDesignId == roomDesign.RoomDesignId)
                return roomDesign1;
            return this.GetMaxLevelRoomDesignRecursively(roomDesign1);
        }

        public RoomDesign GetMaxLevelRoomDesign(int rootRoomDesignID)
        {
            RoomDesign roomDesign1 = (RoomDesign)null;
            foreach (RoomDesign roomDesign2 in this.roomDesignList.ToArray())
            {
                if (roomDesign2.RootRoomDesignId == rootRoomDesignID && (roomDesign1 == null || roomDesign2.Level > roomDesign1.Level))
                    roomDesign1 = roomDesign2;
            }
            return roomDesign1;
        }

        public bool RoomIsMaxLevel(RoomDesign roomDesign)
        {
            foreach (RoomDesign roomDesign1 in this.roomDesignList.ToArray())
            {
                if (roomDesign1.RootRoomDesignId == roomDesign.RootRoomDesignId && roomDesign1.Level > roomDesign.Level)
                    return false;
            }
            return true;
        }

        public HashSet<int> GetRequiredFileIDStackForRoomDesign(RoomDesign thisRoomDesign)
        {
            Stack stack = new Stack();
            HashSet<int> intSet = new HashSet<int>();
            stack.Push((object)thisRoomDesign.ImageSpriteId);
            stack.Push((object)thisRoomDesign.LogoSpriteId);
            stack.Push((object)thisRoomDesign.ConstructionSpriteId);
            foreach (RoomDesignSprite roomDesignSprite in this.roomDesignSpriteDictionary[thisRoomDesign.RoomDesignId])
            {
                stack.Push((object)roomDesignSprite.SpriteId);
                if (roomDesignSprite.AnimationId != 0)
                {
                    foreach (SpriteDesign spriteDesign in SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId).AnimationSprites.ToArray())
                        intSet.Add(spriteDesign.ImageFileId);
                }
            }
            if (thisRoomDesign.MissileDesignId != 0)
            {
                PSAnimation launchPsAnimation = thisRoomDesign.MissileDesign.LaunchPsAnimation;
                if (launchPsAnimation != null)
                {
                    foreach (SpriteDesign spriteDesign in launchPsAnimation.AnimationSprites.ToArray())
                        intSet.Add(spriteDesign.ImageFileId);
                }
                PSAnimation psAnimation = thisRoomDesign.MissileDesign.PsAnimation;
                if (psAnimation != null)
                {
                    foreach (SpriteDesign spriteDesign in psAnimation.AnimationSprites.ToArray())
                        intSet.Add(spriteDesign.ImageFileId);
                }
                PSAnimation hitPsAnimation = thisRoomDesign.MissileDesign.HitPsAnimation;
                if (hitPsAnimation != null)
                {
                    foreach (SpriteDesign spriteDesign in hitPsAnimation.AnimationSprites.ToArray())
                        intSet.Add(spriteDesign.ImageFileId);
                }
            }
            foreach (int id in stack.ToArray())
                intSet.Add(SingletonManager<SpriteManager>.Instance.GetSpriteById(id).ImageFileId);
            return intSet;
        }

        public void AddRoomEventTriggers(GameObject room, RoomManager.RoomButtonClickDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            RoomManager.\u003CAddRoomEventTriggers\u003Ec__AnonStorey1C triggersCAnonStorey1C = new RoomManager.\u003CAddRoomEventTriggers\u003Ec__AnonStorey1C();
            // ISSUE: reference to a compiler-generated field
            triggersCAnonStorey1C.del = del;
            // ISSUE: reference to a compiler-generated field
            triggersCAnonStorey1C.room = room;
            // ISSUE: reference to a compiler-generated field
            triggersCAnonStorey1C.room.AddComponent<EventTrigger>();
            // ISSUE: reference to a compiler-generated field
            triggersCAnonStorey1C.room.AddComponent<ButtonData>();
            // ISSUE: reference to a compiler-generated field
            ((ButtonData)triggersCAnonStorey1C.room.GetComponent<ButtonData>()).buttonType = ButtonType.Room;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)4, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__0)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)13, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__1)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)5, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__2)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)14, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__3)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)0, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__4)));
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            Singleton<SharedManager>.Instance.SetEventTrigger(triggersCAnonStorey1C.room, (EventTriggerType)1, new UnityAction<BaseEventData>((object)triggersCAnonStorey1C, __methodptr(\u003C\u003Em__5)));
        }

        public RoomEntity CreateRoom(bool battleEntity, ShipEntity shipEntity, PSRoom psRoom, RoomDesign roomDesign, RoomManager.RoomButtonClickDelegate del, bool constructing = false, bool createSmoke = true)
        {
            PSShip psShip = shipEntity.PSShip;
            PSAnimation psAnimation1 = (PSAnimation)null;
            PSAnimation psAnimation2 = (PSAnimation)null;
            PSAnimation psAnimation3 = (PSAnimation)null;
            int rows = roomDesign.Rows;
            int columns = roomDesign.Columns;
            SpriteDesign spriteDesign1 = (SpriteDesign)null;
            SpriteDesign spriteDesign2 = (SpriteDesign)null;
            SpriteDesign spriteDesign3 = (SpriteDesign)null;
            SpriteDesign spriteDesign4 = (SpriteDesign)null;
            List<RoomDesignSprite> list = this.roomDesignSpriteDictionary[roomDesign.RoomDesignId].OrderByDescending<RoomDesignSprite, int>((Func<RoomDesignSprite, int>)(rds => rds.RaceId)).ToList<RoomDesignSprite>();
            int num1 = 0;
            int length = Enum.GetNames(typeof(RoomSpriteType)).Length;
            foreach (RoomDesignSprite roomDesignSprite in list)
            {
                if (roomDesignSprite.RaceId == 0 || psShip.ShipDesign.RaceId == roomDesignSprite.RaceId)
                {
                    switch (roomDesignSprite.RoomSpriteType)
                    {
                        case RoomSpriteType.Interior:
                            if (spriteDesign2 == null)
                            {
                                spriteDesign2 = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                                if (roomDesignSprite.AnimationId != 0)
                                    psAnimation1 = SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId);
                                ++num1;
                                break;
                            }
                            break;
                        case RoomSpriteType.Exterior:
                            if (spriteDesign1 == null)
                            {
                                spriteDesign1 = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                                ++num1;
                                break;
                            }
                            break;
                        case RoomSpriteType.InteriorDestroyed:
                            if (spriteDesign4 == null)
                            {
                                spriteDesign4 = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                                ++num1;
                                break;
                            }
                            break;
                        case RoomSpriteType.ExteriorDestroyed:
                            if (spriteDesign3 == null)
                            {
                                spriteDesign3 = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                                ++num1;
                                break;
                            }
                            break;
                        case RoomSpriteType.InteriorActivate:
                            if (psAnimation2 == null)
                            {
                                if (roomDesignSprite.AnimationId != 0)
                                    psAnimation2 = SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId);
                                ++num1;
                                break;
                            }
                            break;
                        case RoomSpriteType.ExteriorActivate:
                            if (psAnimation3 == null)
                            {
                                if (roomDesignSprite.AnimationId != 0)
                                    psAnimation3 = SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId);
                                ++num1;
                                break;
                            }
                            break;
                    }
                    if (num1 >= length)
                        break;
                }
            }
            SpriteDesign constructionSprite = roomDesign.ConstructionSprite;
            SpriteDesign logoSprite = roomDesign.LogoSprite;
            GameObject room1 = (GameObject)Object.Instantiate<GameObject>((M0)this.roomPrefabNew, shipEntity.roomLayer.get_transform());
            ((SpriteRenderer)room1.GetComponent<SpriteRenderer>()).set_sprite(spriteDesign2.UnitySprite);
            float gridWidth = (float)Grid.GRID_WIDTH;
            Vector2 vector2_1;
            // ISSUE: explicit reference operation
            ((Vector2)@vector2_1).\u002Ector((float)columns * gridWidth, (float)rows * gridWidth);
            ((RectTransform)room1.GetComponent<RectTransform>()).set_sizeDelta(vector2_1);
            ((BoxCollider)room1.GetComponent<BoxCollider>()).set_size(new Vector3(Mathf.Abs((float)vector2_1.x), Mathf.Abs((float)vector2_1.y), 1f));
            ((LayoutElement)room1.GetComponent<LayoutElement>()).set_minWidth((float)vector2_1.x);
            ((LayoutElement)room1.GetComponent<LayoutElement>()).set_minHeight((float)vector2_1.y);
            if (psAnimation1 != null)
            {
                ((SpriteRenderer)room1.GetComponent<SpriteRenderer>()).set_sprite(psAnimation1.AnimationSprites[0].UnitySprite);
                ((AnimationObjectNew)room1.AddComponent<AnimationObjectNew>()).anim = psAnimation1;
            }
            RoomMainEntity roomMainEntity = (RoomMainEntity)null;
            RoomEntity roomEntity;
            if (battleEntity)
            {
                RoomType roomType = roomDesign.RoomType;
                RoomBattleEntity roomBattleEntity;
                switch (roomType)
                {
                    case RoomType.Carrier:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<CarrierRoomEntity>();
                        break;
                    case RoomType.AntiCraft:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<AntiCraftRoomEntity>();
                        break;
                    case RoomType.Medical:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<MedicalRoomEntity>();
                        break;
                    case RoomType.Trap:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<TrapRoomEntity>();
                        break;
                    case RoomType.Radar:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<RadarRoomEntity>();
                        break;
                    case RoomType.Stealth:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<CloakingRoomEntity>();
                        break;
                    case RoomType.Cannon:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<MissileRoomEntity>();
                        break;
                    case RoomType.StationMissile:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<WeaponRoomEntity>();
                        break;
                    case RoomType.Android:
                        roomBattleEntity = (RoomBattleEntity)room1.AddComponent<AndroidRoomEntity>();
                        break;
                    default:
                        switch (roomType - 1)
                        {
                            case RoomType.None:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<WeaponRoomEntity>();
                                break;
                            case RoomType.Bridge:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<EngineRoomEntity>();
                                break;
                            case RoomType.Bedroom:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<ShieldRoomEntity>();
                                break;
                            case RoomType.Storage:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<ReactorRoomEntity>();
                                break;
                            case RoomType.Reactor:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<MissileRoomEntity>();
                                break;
                            case RoomType.Missile:
                                TeleportRoomEntity teleportRoomEntity = (TeleportRoomEntity)room1.AddComponent<TeleportRoomEntity>();
                                teleportRoomEntity.teleportGrid = psRoom.RoomStatus != RoomStatus.Normal ? (Grid)null : shipEntity.gridMap.grid[psRoom.Column + 1, psRoom.Row + 1];
                                roomBattleEntity = (RoomBattleEntity)teleportRoomEntity;
                                break;
                            default:
                                roomBattleEntity = (RoomBattleEntity)room1.AddComponent<RoomBattleEntity>();
                                break;
                        }
                }
                roomBattleEntity.friendlyTargetingCharacters.Clear();
                roomBattleEntity.enemyTargetingCharacters.Clear();
                roomBattleEntity.characterQueue.Clear();
                roomEntity = (RoomEntity)roomBattleEntity;
            }
            else
            {
                switch (roomDesign.RoomType)
                {
                    case RoomType.Laser:
                        roomMainEntity = (RoomMainEntity)room1.AddComponent<RoomMainEntity>();
                        TutorialObject tutorialObject1 = (TutorialObject)room1.AddComponent<TutorialObject>();
                        tutorialObject1.tutorialIds = new List<int>((IEnumerable<int>)new int[2]
                        {
              5,
              6
                        });
                        tutorialObject1.des1 = new List<string>((IEnumerable<string>)new string[1]
                        {
              "Wait for the new room to finish construction."
                        });
                        tutorialObject1.des2 = new List<string>((IEnumerable<string>)new string[1]
                        {
              "This room will produce minerals. WAIT for a mineral to be produced."
                        });
                        tutorialObject1.types = new List<TutorialType>((IEnumerable<TutorialType>)new TutorialType[2]
                        {
              TutorialType.Wait,
              TutorialType.Wait
                        });
                        break;
                    case RoomType.Bedroom:
                        roomMainEntity = (RoomMainEntity)room1.AddComponent<RoomMainEntity>();
                        TutorialObject tutorialObject2 = (TutorialObject)room1.AddComponent<TutorialObject>();
                        tutorialObject2.tutorialIds = new List<int>((IEnumerable<int>)new int[2]
                        {
              25,
              29
                        });
                        tutorialObject2.des1 = new List<string>((IEnumerable<string>)new string[1]
                        {
              "TAP the bedroom to select it"
                        });
                        tutorialObject2.des2 = new List<string>((IEnumerable<string>)new string[1]
                        {
              "Have a biscuit while you wait for the room to upgrade."
                        });
                        tutorialObject2.types = new List<TutorialType>((IEnumerable<TutorialType>)new TutorialType[2]
                        {
              TutorialType.Tap,
              TutorialType.Wait
                        });
                        break;
                    default:
                        roomMainEntity = (RoomMainEntity)room1.AddComponent<RoomMainEntity>();
                        break;
                }
                roomEntity = (RoomEntity)roomMainEntity;
            }
            roomEntity.shipEntity = shipEntity;
            roomEntity.moduleContainer = ((Component)((Component)roomEntity).get_gameObject().get_transform().GetChild(0)).get_gameObject();
            roomEntity.Room = psRoom;
            roomEntity.Room.SystemPower = roomDesign.MaxSystemPower;
            roomEntity.Room.PowerGenerated = roomDesign.MaxPowerGenerated;
            roomEntity.positionNum = roomDesign.Columns;
            roomEntity.friendlyCharactersInRoom.Clear();
            roomEntity.Room.constructing = constructing;
            roomEntity.interiorSprite = spriteDesign2;
            roomEntity.exteriorSprite = spriteDesign1;
            roomEntity.interiorDestroyedSprite = spriteDesign4;
            roomEntity.exteriorDestroyedSprite = spriteDesign3;
            roomEntity.interiorAnim = psAnimation1;
            roomEntity.interiorActivateAnim = psAnimation2;
            roomEntity.exteriorActivateAnim = psAnimation3;
            roomEntity.UpdateSurroundingGrids();
            roomEntity.Position = new Vector2Double(((Component)shipEntity.gridMap.grid[Mathf.Min(psRoom.Column, shipEntity.PSShip.ShipDesign.Columns - 1), Mathf.Min(psRoom.Row, shipEntity.PSShip.ShipDesign.Rows - 1)]).get_gameObject().get_transform().get_localPosition());
            Rect rect1 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            float num2 = ((Rect)@rect1).get_width() / 2f;
            Rect rect2 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            float num3 = ((Rect)@rect2).get_height() / 2f;
            room1.get_transform().set_localPosition(new Vector3((float)roomEntity.LocalPosition.x, (float)roomEntity.LocalPosition.y, 0.0f));
            room1.get_transform().set_localScale(Vector3.get_one());
            if (psAnimation1 != null)
                ((AnimationObjectNew)room1.GetComponent<AnimationObjectNew>()).anim = psAnimation1;
            this.AddRoomEventTriggers(room1, del);
            ((Object)room1).set_name(((Object)shipEntity).get_name() + roomDesign.RoomName + "-" + (object)roomEntity.Room.RoomId);
            ((Object)roomEntity).set_name(((Object)room1).get_name());
            if (roomDesign.Rows * roomDesign.Columns > 1)
            {
                if (constructing)
                    ((SpriteRenderer)room1.GetComponent<SpriteRenderer>()).set_sprite(constructionSprite.UnitySprite);
                Vector2 sizeDelta = ((RectTransform)room1.GetComponent<RectTransform>()).get_sizeDelta();
                GameObject gameObject1 = (GameObject)Object.Instantiate<GameObject>((M0)this.roomFramePrefab, shipEntity.frameLayer.get_transform());
                ((RectTransform)gameObject1.GetComponent<RectTransform>()).set_sizeDelta(sizeDelta);
                ((SpriteRenderer)gameObject1.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.RoomFrameSprite.UnitySprite);
                gameObject1.get_transform().set_position(room1.get_transform().get_position());
                gameObject1.get_transform().set_localScale(Vector3.get_one());
                ((Object)gameObject1).set_name(((Object)room1).get_name() + "Frame");
                ((Renderer)gameObject1.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
                ((SpriteRenderer)gameObject1.GetComponent<SpriteRenderer>()).set_drawMode((SpriteDrawMode)1);
                ((SpriteRenderer)gameObject1.GetComponent<SpriteRenderer>()).set_size(sizeDelta);
                ((Renderer)gameObject1.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                roomEntity.frame = gameObject1;
                if (psRoom.RoomName != "Rock")
                {
                    GameObject gameObject2 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, room1.get_transform());
                    ((Object)gameObject2).set_name(((Object)room1).get_name() + "DoorBack");
                    ((SpriteRenderer)gameObject2.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.DoorFrameRightSprite.UnitySprite);
                    ((Renderer)gameObject2.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomInner");
                    ((Renderer)gameObject2.GetComponent<SpriteRenderer>()).set_sortingOrder(1);
                    gameObject2.get_transform().SetAsFirstSibling();
                    gameObject2.get_transform().set_localScale(Vector3.get_one());
                    gameObject2.get_transform().set_position(room1.get_transform().get_position());
                    ((RectTransform)gameObject2.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameRightSprite.Width, (float)spriteDesign2.Height - 13f));
                    ((SpriteRenderer)gameObject2.GetComponent<SpriteRenderer>()).set_drawMode((SpriteDrawMode)1);
                    ((SpriteRenderer)gameObject2.GetComponent<SpriteRenderer>()).set_size(((RectTransform)gameObject2.GetComponent<RectTransform>()).get_sizeDelta());
                    gameObject2.get_transform().set_localPosition(new Vector3((float)gameObject2.get_transform().get_localPosition().x - num2 + this.doorBackOffsetXNew, (float)gameObject2.get_transform().get_localPosition().y - this.doorBackOffsetYNew, 0.0f));
                    ((Renderer)gameObject2.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
                    roomEntity.door[0] = gameObject2;
                    GameObject gameObject3 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, gameObject1.get_transform());
                    ((Object)gameObject3).set_name(((Object)room1).get_name() + "DoorFront");
                    ((SpriteRenderer)gameObject3.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.DoorFrameLeftSprite.UnitySprite);
                    ((Renderer)gameObject3.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                    gameObject3.get_transform().SetAsFirstSibling();
                    gameObject3.get_transform().set_localScale(Vector3.get_one());
                    gameObject3.get_transform().set_position(room1.get_transform().get_position());
                    ((RectTransform)gameObject3.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameLeftSprite.Width, (float)spriteDesign2.Height - 13f));
                    ((SpriteRenderer)gameObject3.GetComponent<SpriteRenderer>()).set_drawMode((SpriteDrawMode)1);
                    ((SpriteRenderer)gameObject3.GetComponent<SpriteRenderer>()).set_size(((RectTransform)gameObject3.GetComponent<RectTransform>()).get_sizeDelta());
                    gameObject3.get_transform().set_localPosition(new Vector3((float)gameObject3.get_transform().get_localPosition().x - num2 + this.doorFrontOffsetXNew, (float)gameObject3.get_transform().get_localPosition().y - this.doorFrontOffsetYNew, 0.0f));
                    ((Renderer)gameObject3.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
                    roomEntity.door[1] = gameObject3;
                }
                GameObject gameObject4 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, room1.get_transform());
                ((SpriteRenderer)gameObject4.GetComponent<SpriteRenderer>()).set_sprite(SingletonManager<SpriteManager>.Instance.GetSpriteByKey("RoomStatesBar").UnitySprite);
                gameObject4.get_transform().set_localScale(Vector3.get_one());
                Transform transform1 = gameObject4.get_transform();
                Rect rect3 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null x1 = ((Rect)@rect3).get_center().x;
                Rect rect4 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num4 = ((Rect)@rect4).get_center().y - 4.0;
                double num5 = 0.0;
                Vector3 vector3_1 = new Vector3((float)x1, (float)num4, (float)num5);
                transform1.set_localPosition(vector3_1);
                ((Object)gameObject4).set_name(((Object)room1).get_name() + "Notification");
                ((Renderer)gameObject4.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                roomEntity.notification = gameObject4;
                GameObject gameObject5 = (GameObject)Object.Instantiate<GameObject>((M0)this.notificationTextPrefabNew, gameObject4.get_transform());
                ((Object)gameObject5).set_name(((Object)room1).get_name() + "NotificationText");
                gameObject5.get_transform().set_localPosition(Vector3.get_zero());
                gameObject5.get_transform().set_localScale(Vector3.get_one());
                ((RectTransform)gameObject5.GetComponent<RectTransform>()).set_offsetMin(Vector2.get_zero());
                ((RectTransform)gameObject5.GetComponent<RectTransform>()).set_offsetMax(Vector2.get_zero());
                ((Renderer)gameObject5.GetComponent<MeshRenderer>()).set_sortingLayerName("Text");
                roomEntity.notificationText = gameObject5;
                gameObject4.SetActive(false);
                if (logoSprite != null)
                {
                    GameObject gameObject2 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBannerPrefabNew, gameObject1.get_transform());
                    gameObject2.get_transform().set_localScale(Vector3.get_one());
                    GameObject gameObject3 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, gameObject2.get_transform());
                    ((SpriteRenderer)gameObject3.GetComponent<SpriteRenderer>()).set_sprite(logoSprite.UnitySprite);
                    ((Renderer)gameObject3.GetComponent<SpriteRenderer>()).set_sortingOrder(1);
                    ((LayoutElement)gameObject3.AddComponent<LayoutElement>()).set_minWidth((float)logoSprite.Width);
                    ((LayoutElement)gameObject3.GetComponent<LayoutElement>()).set_minHeight((float)logoSprite.Height);
                    gameObject3.get_transform().set_localScale(Vector3.get_one());
                    gameObject3.get_transform().set_localPosition(new Vector3(3f, -2f, 0.0f));
                    ((Renderer)gameObject3.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                    ((Object)gameObject3).set_name(((Object)room1).get_name() + "Logo");
                    roomEntity.logo = gameObject3;
                    GameObject gameObject6 = (GameObject)Object.Instantiate<GameObject>((M0)this.textPrefabNew, gameObject2.get_transform());
                    gameObject6.get_transform().set_position(room1.get_transform().get_position());
                    gameObject6.get_transform().set_localScale(Vector3.get_one());
                    ((RectTransform)gameObject6.GetComponent<RectTransform>()).set_anchoredPosition(new Vector2(15f, 0.0f));
                    ((Object)gameObject6).set_name(((Object)room1).get_name() + "Text");
                    ((TextMesh)gameObject6.GetComponent<TextMesh>()).set_text(roomDesign.RoomShortName);
                    roomEntity.text = gameObject6;
                    GameObject gameObject7 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBarContainerPrefabNew, gameObject2.get_transform());
                    gameObject7.get_transform().set_localScale(Vector3.get_one());
                    roomEntity.frameBarContainer = gameObject7;
                    int num6 = (int)roomEntity.Room.MaxHp / 100;
                    for (int index = 0; index < num6; ++index)
                    {
                        GameObject gameObject8 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, gameObject7.get_transform());
                        ((SpriteRenderer)gameObject8.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.HpBarSprite.UnitySprite);
                        ((Renderer)gameObject8.GetComponent<SpriteRenderer>()).set_sortingOrder(1);
                        ((SpriteRenderer)gameObject8.GetComponent<SpriteRenderer>()).set_drawMode((SpriteDrawMode)1);
                        ((SpriteRenderer)gameObject8.GetComponent<SpriteRenderer>()).set_size(new Vector2(2f, 5f));
                        ((Renderer)gameObject8.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                        gameObject8.get_transform().set_localPosition(Vector3.get_zero());
                        gameObject8.get_transform().set_localScale(Vector3.get_one());
                        ((Object)gameObject8).set_name(((Object)room1).get_name() + "HpBar" + (object)index);
                        gameObject8.get_transform().SetAsFirstSibling();
                        roomEntity.hpBars.Add(gameObject8);
                    }
                    roomEntity.Room.Hp = roomEntity.Room.MaxHp;
                }
                GameObject spriteObject = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, shipEntity.overlayLayer.get_transform());
                M0 component1 = spriteObject.GetComponent<RectTransform>();
                Rect rect5 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double width = (double)((Rect)@rect5).get_width();
                Rect rect6 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double height = (double)((Rect)@rect6).get_height();
                Vector2 vector2_2 = new Vector2((float)width, (float)height);
                ((RectTransform)component1).set_sizeDelta(vector2_2);
                Color black = Color.get_black();
                black.a = (__Null)0.5;
                SpriteRendererOverride.SetColor(spriteObject, black);
                spriteObject.get_transform().set_localScale(Vector3.get_one());
                spriteObject.get_transform().set_position(room1.get_transform().get_position());
                ((Object)spriteObject).set_name(((Object)room1).get_name() + "Overlay");
                spriteObject.SetActive(false);
                roomEntity.overlay = spriteObject;
                GameObject gameObject9 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefabNew, spriteObject.get_transform());
                ((SpriteRenderer)gameObject9.GetComponent<SpriteRenderer>()).set_sprite(SingletonManager<SpriteManager>.Instance.GetSpriteByKey("Weapon_Stun_Icon").UnitySprite);
                gameObject9.get_transform().set_localScale(Vector3.get_one());
                ((RectTransform)gameObject9.GetComponent<RectTransform>()).set_anchoredPosition(Vector2.get_zero());
                ((Object)gameObject9).set_name(((Object)room1).get_name() + "OverlayIcon");
                roomEntity.overlayIcon = gameObject9;
                GameObject gameObject10 = (GameObject)Object.Instantiate<GameObject>((M0)this.timerTextPrefabNew, gameObject1.get_transform());
                ((Object)gameObject10).set_name(((Object)room1).get_name() + "Timer");
                gameObject10.get_transform().set_localScale(Vector3.get_one());
                Transform transform2 = gameObject10.get_transform();
                Rect rect7 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null x2 = ((Rect)@rect7).get_center().x;
                Rect rect8 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null y = ((Rect)@rect8).get_center().y;
                double num7 = 0.0;
                Vector3 vector3_2 = new Vector3((float)x2, (float)y, (float)num7);
                transform2.set_localPosition(vector3_2);
                RectTransform component2 = (RectTransform)gameObject10.GetComponent<RectTransform>();
                component2.set_offsetMin(new Vector2(2f, (float)component2.get_offsetMin().y));
                component2.set_offsetMax(new Vector2(2f, (float)component2.get_offsetMax().y));
                roomEntity.timerText = gameObject10;
                gameObject10.SetActive(false);
                if (constructing && Object.op_Inequality((Object)roomEntity.timerText, (Object)null))
                {
                    RoomDesign roomDesign1 = psRoom.RoomStatus != RoomStatus.Building ? this.GetRoomDesignByID(psRoom.UpgradeRoomDesignId) : psRoom.RoomDesign;
                    roomEntity.Room.UpgradeToRoomDesign = roomDesign1;
                    if (roomMainEntity != null)
                        roomMainEntity.StartRoomConstructionTimer();
                }
            }
            else
            {
                ((Renderer)room1.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
                if (constructing)
                    roomEntity.Room.RoomStatus = RoomStatus.Normal;
            }
            if (spriteDesign1 != null)
            {
                Rect rect3 = ((RectTransform)room1.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                Vector2 center = ((Rect)@rect3).get_center();
                GameObject spriteObject = (GameObject)Object.Instantiate<GameObject>((M0)this.exteriorPrefabNew, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.get_identity());
                spriteObject.get_transform().SetParent(room1.get_transform());
                spriteObject.get_transform().set_localScale(Vector3.get_one());
                spriteObject.get_transform().set_localPosition(new Vector3((float)center.x, (float)center.y, 0.0f));
                spriteObject.get_transform().SetParent(shipEntity.exteriorWeaponsLayer.get_transform());
                ((Object)spriteObject).set_name(((Object)room1).get_name() + "Exterior");
                ((SpriteRenderer)spriteObject.GetComponent<SpriteRenderer>()).set_sprite(spriteDesign1.UnitySprite);
                ((Renderer)spriteObject.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
                SpriteRendererOverride.SetAlpha(spriteObject, 0.0f);
                RoomExterior component = (RoomExterior)spriteObject.GetComponent<RoomExterior>();
                roomEntity.exterior = spriteObject;
                ((SpriteRendererOverride)spriteObject.GetComponent<SpriteRendererOverride>()).UpdateColor(Color.get_white());
            }
            GameObject gameObject = (GameObject)Object.Instantiate<GameObject>((M0)this.quantityTextPrefabNew, room1.get_transform());
            gameObject.get_transform().set_localScale(Vector3.get_one());
            ((Object)gameObject).set_name(((Object)room1).get_name() + "QuantityText");
            roomEntity.quantityText = gameObject;
            int num8 = roomEntity.Room.Row + rows;
            int num9 = roomEntity.Room.Column + columns;
            for (int column = roomEntity.Room.Column; column < num9; ++column)
            {
                for (int row = roomEntity.Room.Row; row < num8; ++row)
                    shipEntity.gridMap.grid[Mathf.Min(column, shipEntity.PSShip.ShipDesign.Columns - 1), Mathf.Min(row, shipEntity.PSShip.ShipDesign.Rows - 1)].room = room1;
            }
            if (!constructing)
            {
                if (Object.op_Inequality((Object)room1.GetComponent<AnimationObjectNew>(), (Object)null) && roomDesign.RoomType != RoomType.Storage)
                    ((AnimationObjectNew)room1.GetComponent<AnimationObjectNew>()).PlayAnimation(true, false, (Action)null);
            }
            else if (createSmoke)
                roomEntity.CreateBuildSmokeParticles();
            roomEntity.ArrangeModules();
            roomEntity.CheckNotifications();
            RoomManufactureInterface room2 = roomEntity.Room as RoomManufactureInterface;
            if (room2 != null && room2.ManufactureTimeRemaining > 0.0)
                roomEntity.DelayedNotificationCheck(room2.ManufactureTimeRemaining);
            if (roomEntity.Room.RoomStatus == RoomStatus.Normal)
                roomEntity.SetRoomAnimationFrame();
            shipEntity.roomEntities.Add(roomEntity);
            return roomEntity;
        }

        public void CreateRoomInInventory(ShipEntity shipEntity, PSRoom thisPSRoom, RoomDesign roomDesign, RoomManager.RoomButtonClickDelegate del, GameObject roomInventory, bool setAsFirstSibling = false)
        {
            bool flag = false;
            IEnumerator enumerator = roomInventory.get_transform().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    RoomUIEntity component = (RoomUIEntity)((Component)enumerator.Current).GetComponent<RoomUIEntity>();
                    if (component.inventoryRoomDesign != null && component.inventoryRoomDesign.RoomDesignId == thisPSRoom.RoomDesignId)
                    {
                        flag = true;
                        component.CheckNotifications();
                        break;
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
            if (flag)
                return;
            PSShip psShip = shipEntity.PSShip;
            int rows = roomDesign.Rows;
            int columns = roomDesign.Columns;
            SpriteDesign sprite = (SpriteDesign)null;
            foreach (RoomDesignSprite roomDesignSprite in this.roomDesignSpriteDictionary[roomDesign.RoomDesignId].OrderByDescending<RoomDesignSprite, int>((Func<RoomDesignSprite, int>)(rds => rds.RaceId)).ToList<RoomDesignSprite>())
            {
                if (roomDesignSprite.RoomSpriteType == RoomSpriteType.Interior && (roomDesignSprite.RaceId == 0 || psShip.ShipDesign.RaceId == roomDesignSprite.RaceId))
                {
                    if (sprite == null)
                        sprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                    else
                        break;
                }
            }
            SpriteDesign logoSprite = roomDesign.LogoSprite;
            GameObject room = (GameObject)Object.Instantiate<GameObject>((M0)this.roomPrefab, shipEntity.roomLayer.get_transform());
            Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)room.GetComponent<Image>(), sprite, true, (Image.Type)0, false);
            RoomUIEntity roomUiEntity = (RoomUIEntity)room.AddComponent<RoomUIEntity>();
            roomUiEntity.shipEntity = shipEntity;
            roomUiEntity.inventoryRoomDesign = roomDesign;
            roomUiEntity.moduleContainer = ((Component)((Component)roomUiEntity).get_gameObject().get_transform().GetChild(0)).get_gameObject();
            roomUiEntity.positionNum = roomDesign.Columns;
            room.get_transform().set_localPosition(Vector3.get_zero());
            room.get_transform().set_localScale(Vector3.get_one());
            this.AddRoomEventTriggers(room, del);
            ((Object)room).set_name(((Object)shipEntity).get_name() + "Inventory" + roomDesign.RoomName + "-" + (object)roomDesign.RoomDesignId);
            ((Object)roomUiEntity).set_name(((Object)room).get_name());
            if (roomDesign.Rows * roomDesign.Columns > 1)
            {
                GameObject gameObject1 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopLeftPrefab, shipEntity.frameLayer.get_transform());
                M0 component1 = gameObject1.GetComponent<RectTransform>();
                Rect rect1 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double width = (double)((Rect)@rect1).get_width();
                Rect rect2 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double height1 = (double)((Rect)@rect2).get_height();
                Vector2 vector2 = new Vector2((float)width, (float)height1);
                ((RectTransform)component1).set_sizeDelta(vector2);
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject1.GetComponent<Image>(), psShip.ShipDesign.RoomFrameSprite, false, (Image.Type)0, false);
                gameObject1.get_transform().set_position(room.get_transform().get_position());
                gameObject1.get_transform().set_localScale(Vector3.get_one());
                ((Object)gameObject1).set_name(((Object)room).get_name() + "Frame");
                ((Graphic)gameObject1.GetComponent<Image>()).set_material(shipEntity.sharedMaterial);
                ((Graphic)gameObject1.GetComponent<Image>()).set_raycastTarget(false);
                roomUiEntity.frame = gameObject1;
                GameObject gameObject2 = (GameObject)Object.Instantiate<GameObject>((M0)this.doorPrefab, room.get_transform());
                ((Object)gameObject2).set_name(((Object)room).get_name() + "DoorBack");
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject2.GetComponent<Image>(), psShip.ShipDesign.DoorFrameRightSprite, true, (Image.Type)0, false);
                ((Graphic)gameObject2.GetComponent<Image>()).set_raycastTarget(false);
                gameObject2.get_transform().SetAsFirstSibling();
                gameObject2.get_transform().set_localScale(Vector3.get_one());
                gameObject2.get_transform().set_position(room.get_transform().get_position());
                Transform transform1 = gameObject2.get_transform();
                double num1 = gameObject2.get_transform().get_localPosition().x + (double)this.doorBackOffsetX;
                // ISSUE: variable of the null type
                __Null y1 = gameObject2.get_transform().get_localPosition().y;
                Rect rect3 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double height2 = (double)((Rect)@rect3).get_height();
                double num2 = y1 - height2 + (double)this.doorBackOffsetY;
                double num3 = 0.0;
                Vector3 vector3_1 = new Vector3((float)num1, (float)num2, (float)num3);
                transform1.set_localPosition(vector3_1);
                ((RectTransform)gameObject2.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameRightSprite.Width, (float)sprite.Height - 12f));
                ((Graphic)gameObject2.GetComponent<Image>()).set_material(shipEntity.sharedMaterial);
                roomUiEntity.door[0] = gameObject2;
                GameObject gameObject3 = (GameObject)Object.Instantiate<GameObject>((M0)this.doorPrefab, gameObject1.get_transform());
                ((Object)gameObject3).set_name(((Object)room).get_name() + "DoorFront");
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject3.GetComponent<Image>(), psShip.ShipDesign.DoorFrameLeftSprite, true, (Image.Type)0, false);
                ((Graphic)gameObject3.GetComponent<Image>()).set_raycastTarget(false);
                gameObject3.get_transform().SetAsFirstSibling();
                gameObject3.get_transform().set_localScale(Vector3.get_one());
                gameObject3.get_transform().set_position(room.get_transform().get_position());
                Transform transform2 = gameObject3.get_transform();
                double num4 = gameObject3.get_transform().get_localPosition().x + (double)this.doorFrontOffsetX;
                // ISSUE: variable of the null type
                __Null y2 = gameObject3.get_transform().get_localPosition().y;
                Rect rect4 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double height3 = (double)((Rect)@rect4).get_height();
                double num5 = y2 - height3 + (double)this.doorFrontOffsetY;
                double num6 = 0.0;
                Vector3 vector3_2 = new Vector3((float)num4, (float)num5, (float)num6);
                transform2.set_localPosition(vector3_2);
                ((RectTransform)gameObject3.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameLeftSprite.Width, (float)sprite.Height - 12f));
                ((Graphic)gameObject3.GetComponent<Image>()).set_material(shipEntity.sharedMaterial);
                roomUiEntity.door[1] = gameObject3;
                GameObject gameObject4 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericPrefab, room.get_transform());
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject4.GetComponent<Image>(), SingletonManager<SpriteManager>.Instance.GetSpriteByKey("RoomStatesBar"), true, (Image.Type)0, false);
                gameObject4.get_transform().set_localScale(Vector3.get_one());
                Transform transform3 = gameObject4.get_transform();
                Rect rect5 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null x1 = ((Rect)@rect5).get_center().x;
                Rect rect6 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num7 = ((Rect)@rect6).get_center().y - 4.0;
                double num8 = 0.0;
                Vector3 vector3_3 = new Vector3((float)x1, (float)num7, (float)num8);
                transform3.set_localPosition(vector3_3);
                ((Object)gameObject4).set_name(((Object)room).get_name() + "Notification");
                ((Graphic)gameObject4.GetComponent<Image>()).set_raycastTarget(false);
                roomUiEntity.notification = gameObject4;
                GameObject gameObject5 = (GameObject)Object.Instantiate<GameObject>((M0)this.notificationTextPrefab, gameObject4.get_transform());
                ((Object)gameObject5).set_name(((Object)room).get_name() + "NotificationText");
                gameObject5.get_transform().set_localPosition(Vector3.get_zero());
                gameObject5.get_transform().set_localScale(Vector3.get_one());
                ((RectTransform)gameObject5.GetComponent<RectTransform>()).set_offsetMin(Vector2.get_zero());
                ((RectTransform)gameObject5.GetComponent<RectTransform>()).set_offsetMax(Vector2.get_zero());
                ((Graphic)gameObject5.GetComponent<MediaTypeNames.Text>()).set_raycastTarget(false);
                roomUiEntity.notificationText = gameObject5;
                gameObject4.SetActive(false);
                if (logoSprite != null)
                {
                    GameObject gameObject6 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBannerPrefab, gameObject1.get_transform());
                    gameObject6.get_transform().set_localScale(Vector3.get_one());
                    GameObject gameObject7 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopLeftPrefab, gameObject6.get_transform());
                    Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject7.GetComponent<Image>(), logoSprite, true, (Image.Type)0, false);
                    ((Graphic)gameObject7.GetComponent<Image>()).set_raycastTarget(false);
                    gameObject7.get_transform().set_localScale(Vector3.get_one());
                    gameObject7.get_transform().set_localPosition(new Vector3(3f, -2f, 0.0f));
                    ((Object)gameObject7).set_name(((Object)room).get_name() + "Logo");
                    roomUiEntity.logo = gameObject7;
                    GameObject gameObject8 = (GameObject)Object.Instantiate<GameObject>((M0)this.textPrefab, gameObject6.get_transform());
                    gameObject8.get_transform().set_position(room.get_transform().get_position());
                    gameObject8.get_transform().set_localScale(Vector3.get_one());
                    ((RectTransform)gameObject8.GetComponent<RectTransform>()).set_anchoredPosition(new Vector2(15f, 0.0f));
                    ((Object)gameObject8).set_name(((Object)room).get_name() + "Text");
                    ((MediaTypeNames.Text)gameObject8.GetComponent<MediaTypeNames.Text>()).set_text(roomDesign.RoomShortName);
                    ((Graphic)gameObject8.GetComponent<MediaTypeNames.Text>()).set_raycastTarget(false);
                    roomUiEntity.text = gameObject8;
                    GameObject gameObject9 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBarContainerPrefab, gameObject6.get_transform());
                    gameObject9.get_transform().set_localScale(Vector3.get_one());
                    roomUiEntity.frameBarContainer = gameObject9;
                    int num9 = (int)thisPSRoom.MaxHp / 100;
                    for (int index = 0; index < num9; ++index)
                    {
                        GameObject gameObject10 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopRightPrefab, gameObject9.get_transform());
                        Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject10.GetComponent<Image>(), psShip.ShipDesign.HpBarSprite, true, (Image.Type)0, false);
                        gameObject10.get_transform().set_localPosition(Vector3.get_zero());
                        gameObject10.get_transform().set_localScale(Vector3.get_one());
                        ((Object)gameObject10).set_name(((Object)room).get_name() + "HpBar" + (object)index);
                        ((Graphic)gameObject10.GetComponent<Image>()).set_raycastTarget(false);
                        gameObject10.get_transform().SetAsFirstSibling();
                        roomUiEntity.hpBars.Add(gameObject10);
                    }
                }
                GameObject gameObject11 = (GameObject)Object.Instantiate<GameObject>((M0)this.timerTextPrefab, gameObject1.get_transform());
                ((Object)gameObject11).set_name(((Object)room).get_name() + "Timer");
                ((Graphic)gameObject11.GetComponent<MediaTypeNames.Text>()).set_raycastTarget(false);
                gameObject11.get_transform().set_localScale(Vector3.get_one());
                Transform transform4 = gameObject11.get_transform();
                Rect rect7 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null x2 = ((Rect)@rect7).get_center().x;
                Rect rect8 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                // ISSUE: variable of the null type
                __Null y3 = ((Rect)@rect8).get_center().y;
                double num10 = 0.0;
                Vector3 vector3_4 = new Vector3((float)x2, (float)y3, (float)num10);
                transform4.set_localPosition(vector3_4);
                RectTransform component2 = (RectTransform)gameObject11.GetComponent<RectTransform>();
                component2.set_offsetMin(new Vector2(2f, (float)component2.get_offsetMin().y));
                component2.set_offsetMax(new Vector2(2f, (float)component2.get_offsetMax().y));
                roomUiEntity.timerText = gameObject11;
                gameObject11.SetActive(false);
            }
            else
                ((Graphic)room.GetComponent<Image>()).set_material(shipEntity.sharedMaterial);
            GameObject gameObject = (GameObject)Object.Instantiate<GameObject>((M0)this.quantityTextPrefab, room.get_transform());
            gameObject.get_transform().set_localScale(Vector3.get_one());
            ((Object)gameObject).set_name(((Object)room).get_name() + "QuantityText");
            roomUiEntity.quantityText = gameObject;
            room.get_transform().SetParent(roomInventory.get_transform(), false);
            room.get_transform().set_localPosition(Vector3.get_zero());
            if (setAsFirstSibling)
                room.get_transform().SetAsFirstSibling();
            if (roomDesign.Rows * roomDesign.Columns > 1)
            {
                GameObject[] gameObjectArray = new GameObject[2] { roomUiEntity.door[0], null };
                gameObjectArray[0].get_transform().SetParent(room.get_transform(), false);
                Transform transform1 = gameObjectArray[0].get_transform();
                Rect rect1 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num1 = (double)((Rect)@rect1).get_xMin() + (double)this.doorBackOffsetX;
                Rect rect2 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num2 = (double)((Rect)@rect2).get_yMin() + (double)this.doorBackOffsetY;
                double num3 = 0.0;
                Vector3 vector3_1 = new Vector3((float)num1, (float)num2, (float)num3);
                transform1.set_localPosition(vector3_1);
                gameObjectArray[1] = roomUiEntity.door[1];
                gameObjectArray[1].get_transform().SetParent(room.get_transform(), false);
                Transform transform2 = gameObjectArray[1].get_transform();
                Rect rect3 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num4 = (double)((Rect)@rect3).get_xMin() + (double)this.doorFrontOffsetX;
                Rect rect4 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double num5 = (double)((Rect)@rect4).get_yMin() + (double)this.doorFrontOffsetY;
                double num6 = 0.0;
                Vector3 vector3_2 = new Vector3((float)num4, (float)num5, (float)num6);
                transform2.set_localPosition(vector3_2);
                GameObject frame = roomUiEntity.frame;
                frame.get_transform().SetParent(room.get_transform(), false);
                frame.get_transform().set_localPosition(Vector3.get_zero());
            }
            roomUiEntity.CheckNotifications();
            shipEntity.roomEntities.Add((RoomEntity)roomUiEntity);
        }

        public RoomPrefab CreateRoomPrefab(RoomDesign thisRoomDesign, PSShip psShip, ShipEntity shipEntity, bool ghostRoom = false)
        {
            RoomPrefab roomPrefab = new RoomPrefab();
            roomPrefab.roomDesign = thisRoomDesign;
            roomPrefab.rows = thisRoomDesign.Rows;
            roomPrefab.columns = thisRoomDesign.Columns;
            roomPrefab.category = thisRoomDesign.CategoryType;
            roomPrefab.constructionTime = (double)thisRoomDesign.ConstructionTime;
            roomPrefab.minimumLevel = thisRoomDesign.Level;
            int raceId = psShip.ShipDesign.RaceId;
            roomPrefab.raceId = raceId;
            SpriteDesign sprite = (SpriteDesign)null;
            List<RoomDesignSprite> roomDesignSpriteList = new List<RoomDesignSprite>();
            if (this.roomDesignSpriteDictionary.ContainsKey(thisRoomDesign.RoomDesignId))
                roomDesignSpriteList = this.roomDesignSpriteDictionary[thisRoomDesign.RoomDesignId];
            foreach (RoomDesignSprite roomDesignSprite in roomDesignSpriteList)
            {
                if (roomDesignSprite.RaceId == 0 || raceId == roomDesignSprite.RaceId)
                {
                    switch (roomDesignSprite.RoomSpriteType)
                    {
                        case RoomSpriteType.Interior:
                            sprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                            if (roomDesignSprite.AnimationId != 0)
                            {
                                roomPrefab.anim = SingletonManager<AnimationManager>.Instance.GetAnimationByID(roomDesignSprite.AnimationId);
                                break;
                            }
                            break;
                        case RoomSpriteType.Exterior:
                            roomPrefab.exteriorSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                            break;
                        case RoomSpriteType.InteriorDestroyed:
                            roomPrefab.interiorDestroyedSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                            break;
                        case RoomSpriteType.ExteriorDestroyed:
                            roomPrefab.exteriorDestroyedSprite = SingletonManager<SpriteManager>.Instance.GetSpriteById(roomDesignSprite.SpriteId);
                            break;
                    }
                    if (sprite != null && roomPrefab.exteriorSprite != null && roomPrefab.interiorDestroyedSprite != null)
                    {
                        if (roomPrefab.exteriorDestroyedSprite != null)
                            break;
                    }
                }
            }
            SpriteDesign logoSprite = thisRoomDesign.LogoSprite;
            GameObject gameObject1 = (GameObject)Object.Instantiate<GameObject>((M0)this.containerPrefab, ((Component)this).get_transform());
            gameObject1.get_transform().set_localScale(Vector3.get_one());
            ((LayoutElement)gameObject1.GetComponent<LayoutElement>()).set_minWidth((float)sprite.Width);
            ((LayoutElement)gameObject1.GetComponent<LayoutElement>()).set_minHeight((float)sprite.Height);
            ((LayoutElement)gameObject1.GetComponent<LayoutElement>()).set_preferredWidth((float)sprite.Width);
            ((LayoutElement)gameObject1.GetComponent<LayoutElement>()).set_preferredHeight((float)sprite.Height);
            ((Object)gameObject1).set_name(thisRoomDesign.RoomName + "Container(Race: " + (object)psShip.ShipDesign.RaceId + ", ID: " + (object)thisRoomDesign.RoomDesignId + ")");
            ((RectTransform)gameObject1.GetComponent<RectTransform>()).set_anchorMin(new Vector2(0.5f, 0.5f));
            roomPrefab.container = gameObject1;
            GameObject gameObject2 = (GameObject)Object.Instantiate<GameObject>((M0)this.roomPrefab);
            gameObject2.get_transform().SetParent(roomPrefab.container.get_transform(), false);
            ((Object)gameObject2).set_name(thisRoomDesign.RoomName);
            Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject2.GetComponent<Image>(), sprite, true, (Image.Type)0, false);
            if (roomPrefab.anim != null)
            {
                gameObject2.AddComponent<AnimationObject>();
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject2.GetComponent<Image>(), roomPrefab.anim.AnimationSprites[0], true, (Image.Type)0, false);
                ((AnimationObject)gameObject2.GetComponent<AnimationObject>()).anim = roomPrefab.anim;
            }
            roomPrefab.room = gameObject2;
            if (thisRoomDesign.Rows * thisRoomDesign.Columns > 1)
            {
                GameObject gameObject3 = (GameObject)Object.Instantiate<GameObject>((M0)this.doorPrefab, Vector3.get_zero(), Quaternion.get_identity());
                gameObject3.get_transform().SetParent(gameObject2.get_transform());
                gameObject3.get_transform().set_localScale(Vector3.get_one());
                ((Object)gameObject3).set_name(((Object)gameObject2).get_name() + "DoorBack");
                gameObject3.get_transform().set_localPosition(Vector2.op_Implicit(new Vector2(this.doorBackOffsetX, this.doorBackOffsetY)));
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject3.GetComponent<Image>(), psShip.ShipDesign.DoorFrameRightSprite, true, (Image.Type)0, false);
                ((RectTransform)gameObject3.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameRightSprite.Width, (float)sprite.Height - 12f));
                ((Graphic)gameObject3.GetComponent<Image>()).set_raycastTarget(false);
                gameObject3.get_transform().SetParent(roomPrefab.container.get_transform());
                ((Graphic)gameObject3.GetComponent<Image>()).set_material(shipEntity != null ? shipEntity.sharedMaterial : (Material)null);
                roomPrefab.door[0] = gameObject3;
                GameObject gameObject4 = (GameObject)Object.Instantiate<GameObject>((M0)this.doorPrefab, Vector3.get_zero(), Quaternion.get_identity());
                gameObject4.get_transform().SetParent(gameObject2.get_transform());
                gameObject4.get_transform().set_localScale(Vector3.get_one());
                ((Object)gameObject4).set_name(((Object)gameObject2).get_name() + "DoorFront");
                gameObject4.get_transform().set_localPosition(Vector2.op_Implicit(new Vector2(this.doorFrontOffsetX, this.doorFrontOffsetY)));
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject4.GetComponent<Image>(), psShip.ShipDesign.DoorFrameLeftSprite, true, (Image.Type)0, false);
                ((RectTransform)gameObject4.GetComponent<RectTransform>()).set_sizeDelta(new Vector2((float)psShip.ShipDesign.DoorFrameLeftSprite.Width, (float)sprite.Height - 12f));
                ((Graphic)gameObject4.GetComponent<Image>()).set_raycastTarget(false);
                gameObject4.get_transform().SetParent(roomPrefab.container.get_transform());
                ((Graphic)gameObject4.GetComponent<Image>()).set_material(shipEntity != null ? shipEntity.sharedMaterial : (Material)null);
                roomPrefab.door[1] = gameObject4;
                GameObject gameObject5 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopLeftPrefab, gameObject2.get_transform().get_position(), Quaternion.get_identity());
                gameObject5.get_transform().SetParent(gameObject2.get_transform());
                M0 component = gameObject5.GetComponent<RectTransform>();
                Rect rect1 = ((RectTransform)gameObject2.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double width = (double)((Rect)@rect1).get_width();
                Rect rect2 = ((RectTransform)gameObject2.GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                double height = (double)((Rect)@rect2).get_height();
                Vector2 vector2 = new Vector2((float)width, (float)height);
                ((RectTransform)component).set_sizeDelta(vector2);
                gameObject5.get_transform().set_localScale(Vector3.get_one());
                ((Object)gameObject5).set_name(((Object)gameObject2).get_name() + "Frame");
                Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject5.GetComponent<Image>(), psShip.ShipDesign.RoomFrameSprite, false, (Image.Type)0, false);
                gameObject5.get_transform().SetParent(roomPrefab.container.get_transform());
                gameObject5.get_transform().set_localPosition(Vector3.get_zero());
                ((Graphic)gameObject5.GetComponent<Image>()).set_raycastTarget(false);
                ((Graphic)gameObject5.GetComponent<Image>()).set_material(shipEntity != null ? shipEntity.sharedMaterial : (Material)null);
                roomPrefab.frame = gameObject5;
                if (logoSprite != null)
                {
                    GameObject gameObject6 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBannerPrefab, gameObject5.get_transform());
                    gameObject6.get_transform().set_localScale(Vector3.get_one());
                    GameObject gameObject7 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopLeftPrefab, gameObject6.get_transform());
                    Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject7.GetComponent<Image>(), logoSprite, true, (Image.Type)0, false);
                    ((Graphic)gameObject7.GetComponent<Image>()).set_raycastTarget(false);
                    gameObject7.get_transform().set_localScale(Vector3.get_one());
                    gameObject7.get_transform().set_localPosition(new Vector3(3f, -2f, 0.0f));
                    ((Object)gameObject7).set_name(((Object)gameObject2).get_name() + "Logo");
                    roomPrefab.logo = gameObject7;
                    GameObject gameObject8 = (GameObject)Object.Instantiate<GameObject>((M0)this.textPrefab, gameObject6.get_transform());
                    gameObject8.get_transform().set_position(gameObject2.get_transform().get_position());
                    gameObject8.get_transform().set_localScale(Vector3.get_one());
                    ((RectTransform)gameObject8.GetComponent<RectTransform>()).set_anchoredPosition(new Vector2(15f, 0.0f));
                    ((Object)gameObject8).set_name(((Object)gameObject2).get_name() + "Text");
                    ((MediaTypeNames.Text)gameObject8.GetComponent<MediaTypeNames.Text>()).set_text(thisRoomDesign.RoomShortName);
                    ((Graphic)gameObject8.GetComponent<MediaTypeNames.Text>()).set_raycastTarget(false);
                    roomPrefab.text = gameObject8;
                    GameObject gameObject9 = (GameObject)Object.Instantiate<GameObject>((M0)this.frameBarContainerPrefab, gameObject6.get_transform());
                    gameObject9.get_transform().set_localScale(Vector3.get_one());
                    int num = (thisRoomDesign.MaxSystemPower + thisRoomDesign.MaxPowerGenerated) / 100;
                    for (int index = 0; index < num; ++index)
                    {
                        GameObject gameObject10 = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopRightPrefab, gameObject9.get_transform());
                        Singleton<SharedManager>.Instance.SetImageWithUnitySprite((Image)gameObject10.GetComponent<Image>(), psShip.ShipDesign.HpBarSprite, true, (Image.Type)0, false);
                        gameObject10.get_transform().set_localPosition(Vector3.get_zero());
                        gameObject10.get_transform().set_localScale(Vector3.get_one());
                        ((Object)gameObject10).set_name(((Object)gameObject2).get_name() + "HpBar" + (object)index);
                        ((Graphic)gameObject10.GetComponent<Image>()).set_raycastTarget(false);
                        gameObject10.get_transform().SetAsFirstSibling();
                        roomPrefab.hpBars.Add(gameObject10);
                    }
                    if (ghostRoom)
                    {
                        gameObject5.SetActive(false);
                        Material material = Object.Instantiate(Resources.Load("Materials/GhostRoomHologram")) as Material;
                        ((Graphic)gameObject2.GetComponent<Image>()).set_material(material);
                        ((Graphic)gameObject3.GetComponent<Image>()).set_material(material);
                        ((Graphic)gameObject4.GetComponent<Image>()).set_material(material);
                    }
                }
                gameObject5.get_transform().SetAsFirstSibling();
                gameObject2.get_transform().SetAsFirstSibling();
            }
            else
                ((Graphic)gameObject2.GetComponent<Image>()).set_material(shipEntity != null ? shipEntity.sharedMaterial : (Material)null);
            return roomPrefab;
        }

        public void ReplaceRoomNew(PSShip psShip, RoomMainEntity thisRoomEntity, RoomDesign replacingRoomDesign, bool constructing = true)
        {
            if (replacingRoomDesign == null)
                return;
            RoomDesignSpriteHelper designSpriteHelper = new RoomDesignSpriteHelper();
            designSpriteHelper.SetRoomDesignSprites(psShip, (RoomEntity)thisRoomEntity, replacingRoomDesign);
            if (!constructing || replacingRoomDesign.Rows * replacingRoomDesign.Columns == 1)
            {
                thisRoomEntity.Room.SystemPower = replacingRoomDesign.MaxSystemPower * 100;
                thisRoomEntity.Room.PowerGenerated = replacingRoomDesign.MaxPowerGenerated * 100;
                thisRoomEntity.positionNum = replacingRoomDesign.Columns;
                thisRoomEntity.Room.RoomDesignId = replacingRoomDesign.RoomDesignId;
                thisRoomEntity.Room.MissileDesign = SingletonManager<RoomManager>.Instance.GetMissileDesignById(replacingRoomDesign.MissileDesignId);
                ((SpriteRenderer)((Component)thisRoomEntity).GetComponent<SpriteRenderer>()).set_sprite(designSpriteHelper.interiorSprite == null ? (Sprite)null : designSpriteHelper.interiorSprite.UnitySprite);
                if (designSpriteHelper.interiorAnim != null)
                    ((AnimationObjectNew)((Component)thisRoomEntity).GetComponent<AnimationObjectNew>()).anim = designSpriteHelper.interiorAnim;
                ((Object)thisRoomEntity).set_name(replacingRoomDesign.RoomName + (object)thisRoomEntity.Room.RoomId);
            }
            thisRoomEntity.UpdateSurroundingGrids();
            if (replacingRoomDesign.Rows * replacingRoomDesign.Columns > 1)
            {
                thisRoomEntity.Room.ManufactureStartDate = Singleton<SharedManager>.Instance.CurrentTime(false);
                if (constructing)
                {
                    if (designSpriteHelper.interiorAnim != null)
                    {
                        ((AnimationObjectNew)((Component)thisRoomEntity).GetComponent<AnimationObjectNew>()).StopAnimation();
                        ((AnimationObjectNew)((Component)thisRoomEntity).GetComponent<AnimationObjectNew>()).overrideSprite = designSpriteHelper.constructionSprite == null ? (Sprite)null : designSpriteHelper.constructionSprite.UnitySprite;
                    }
                  ((SpriteRenderer)((Component)thisRoomEntity).GetComponent<SpriteRenderer>()).set_sprite(designSpriteHelper.constructionSprite == null ? (Sprite)null : designSpriteHelper.constructionSprite.UnitySprite);
                    thisRoomEntity.Room.ConstructionStartDate = DateTime.UtcNow.Add(SingletonManager<UserManager>.Instance.user.serverTimeDiff);
                    thisRoomEntity.Room.UpgradeToRoomDesign = replacingRoomDesign;
                    thisRoomEntity.StartRoomConstructionTimer();
                    thisRoomEntity.StopResourceGeneration();
                }
                else
                {
                    ((SpriteRenderer)thisRoomEntity.frame.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.RoomFrameSprite == null ? (Sprite)null : psShip.ShipDesign.RoomFrameSprite.UnitySprite);
                    ((Object)thisRoomEntity.frame).set_name(((Object)thisRoomEntity).get_name() + "Frame");
                    ((SpriteRenderer)thisRoomEntity.door[0].GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.DoorFrameRightSprite.UnitySprite);
                    ((Object)thisRoomEntity.door[0]).set_name(((Object)thisRoomEntity).get_name() + "DoorBack");
                    ((SpriteRenderer)thisRoomEntity.door[1].GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.DoorFrameLeftSprite.UnitySprite);
                    ((Object)thisRoomEntity.door[1]).set_name(((Object)thisRoomEntity).get_name() + "DoorFront");
                    if (designSpriteHelper.logoSprite != null)
                    {
                        ((SpriteRenderer)thisRoomEntity.logo.GetComponent<SpriteRenderer>()).set_sprite(designSpriteHelper.logoSprite.UnitySprite);
                        ((Object)thisRoomEntity.logo).set_name(((Object)thisRoomEntity).get_name() + "Logo");
                        ((TextMesh)thisRoomEntity.text.GetComponent<TextMesh>()).set_text(replacingRoomDesign.RoomShortName);
                        ((Object)thisRoomEntity.text).set_name(((Object)thisRoomEntity).get_name() + "Text");
                        int num = (int)thisRoomEntity.Room.MaxHp / 100;
                        thisRoomEntity.Room.Hp = thisRoomEntity.Room.MaxHp;
                        foreach (Object @object in thisRoomEntity.hpBars.ToArray())
                            Object.Destroy(@object);
                        thisRoomEntity.hpBars.Clear();
                        for (int index = 0; index < num; ++index)
                        {
                            GameObject gameObject = (GameObject)Object.Instantiate<GameObject>((M0)this.genericTopRightPrefabNew, thisRoomEntity.frameBarContainer.get_transform());
                            ((SpriteRenderer)gameObject.GetComponent<SpriteRenderer>()).set_sprite(psShip.ShipDesign.HpBarSprite.UnitySprite);
                            gameObject.get_transform().set_localScale(Vector3.get_one());
                            ((Object)gameObject).set_name(((Object)thisRoomEntity).get_name() + "HpBar" + (object)index);
                            ((Renderer)gameObject.GetComponent<SpriteRenderer>()).set_sortingOrder(1);
                            ((SpriteRenderer)gameObject.GetComponent<SpriteRenderer>()).set_drawMode((SpriteDrawMode)1);
                            ((SpriteRenderer)gameObject.GetComponent<SpriteRenderer>()).set_size(new Vector2(2f, 5f));
                            ((Renderer)gameObject.GetComponent<SpriteRenderer>()).set_sortingLayerName("RoomOuter");
                            gameObject.get_transform().set_localPosition(Vector3.get_zero());
                            gameObject.get_transform().set_localScale(Vector3.get_one());
                            gameObject.get_transform().SetAsFirstSibling();
                            thisRoomEntity.hpBars.Add(gameObject);
                        }
                        thisRoomEntity.frameBarContainer.SetActive(false);
                        thisRoomEntity.frameBarContainer.SetActive(true);
                    }
                    if (thisRoomEntity.Room != null)
                    {
                        PSRoom room = thisRoomEntity.Room;
                        RoomStatus? nullable = room != null ? new RoomStatus?(room.RoomStatus) : new RoomStatus?();
                        if ((nullable.GetValueOrDefault() != RoomStatus.Inventory ? 1 : (!nullable.HasValue ? 1 : 0)) != 0)
                        {
                            thisRoomEntity.StopRoomConstructionTimer();
                            if (Object.op_Inequality((Object)((Component)thisRoomEntity).GetComponent<AnimationObjectNew>(), (Object)null))
                                thisRoomEntity.SetRoomAnimationFrame();
                            thisRoomEntity.Room.RoomStatus = RoomStatus.Normal;
                            thisRoomEntity.CheckRoomResources();
                        }
                    }
                }
            }
            else
                thisRoomEntity.Room.RoomStatus = RoomStatus.Normal;
            if (designSpriteHelper.exteriorSprite != null)
            {
                SpriteDesign spriteByKey = SingletonManager<SpriteManager>.Instance.GetSpriteByKey("blankPixel");
                if (Object.op_Equality((Object)thisRoomEntity.exterior, (Object)null))
                {
                    Rect rect = ((RectTransform)((Component)thisRoomEntity).GetComponent<RectTransform>()).get_rect();
                    // ISSUE: explicit reference operation
                    Vector2 center = ((Rect)@rect).get_center();
                    GameObject gameObject = (GameObject)Object.Instantiate<GameObject>((M0)this.exteriorPrefabNew, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.get_identity());
                    gameObject.get_transform().SetParent(((Component)thisRoomEntity).get_transform());
                    gameObject.get_transform().set_localScale(Vector3.get_one());
                    gameObject.get_transform().set_localPosition(new Vector3((float)center.x, (float)center.y, 0.0f));
                    gameObject.get_transform().SetParent(thisRoomEntity.shipEntity.exteriorWeaponsLayer.get_transform());
                    ((Object)gameObject).set_name(((Object)thisRoomEntity).get_name() + "Exterior");
                    ((SpriteRenderer)gameObject.GetComponent<SpriteRenderer>()).set_sprite(designSpriteHelper.exteriorSprite.UnitySprite);
                    ((Renderer)gameObject.GetComponent<SpriteRenderer>()).set_material(thisRoomEntity.shipEntity.sharedMaterial);
                    SpriteRendererOverride.SetAlpha(gameObject, 0.0f);
                    RoomExterior component = (RoomExterior)gameObject.GetComponent<RoomExterior>();
                    thisRoomEntity.exterior = gameObject;
                    this.StartCoroutine(Singleton<SharedManager>.Instance.FadeSpriteCoroutine(gameObject, 1f, 0.5f, false, 0.0f, true, (Queue<Action>)null));
                }
                else
                {
                    ((SpriteRenderer)thisRoomEntity.exterior.GetComponent<SpriteRenderer>()).set_sprite(designSpriteHelper.exteriorSprite.UnitySprite);
                    ((Object)thisRoomEntity.exterior).set_name(((Object)thisRoomEntity).get_name() + "Exterior");
                }
              ((SpriteRenderer)((RoomExterior)thisRoomEntity.exterior.GetComponent<RoomExterior>()).skin.GetComponent<SpriteRenderer>()).set_sprite(spriteByKey.UnitySprite);
            }
            thisRoomEntity.CheckNotifications();
            if (thisRoomEntity.PSMainRoom.ManufactureTimeRemaining > 0.0)
                thisRoomEntity.DelayedNotificationCheck(thisRoomEntity.PSMainRoom.ManufactureTimeRemaining);
            thisRoomEntity.ArrangeModules();
        }

        public void CreateRoomsNew(bool isBattle, ShipEntity shipEntity, ShipEntity.ShipDelegate del, RoomManager.RoomButtonClickDelegate buttonDel, GameObject roomInventory, bool createSmoke = true)
        {
            if (!Object.op_Inequality((Object)shipEntity, (Object)null))
                return;
            foreach (PSRoom psRoom in ((IEnumerable<PSRoom>)shipEntity.PSShip.Rooms).ToArray<PSRoom>())
            {
                if (psRoom.RoomStatus != RoomStatus.Inventory)
                    this.CreateRoom(isBattle, shipEntity, psRoom, psRoom.RoomDesign, buttonDel, psRoom.RoomStatus == RoomStatus.Building || psRoom.RoomStatus == RoomStatus.Upgrading, createSmoke);
                else if (Object.op_Inequality((Object)roomInventory, (Object)null))
                    this.CreateRoomInInventory(shipEntity, psRoom, psRoom.RoomDesign, buttonDel, roomInventory, false);
            }
            shipEntity.SortRoomsSiblingOrder();
            del(shipEntity);
        }

        public bool GridAreaIsInvalid(Grid grid, RoomDesign roomDesign, ShipMainEntity shipEntity)
        {
            int num1 = grid.row + roomDesign.Rows;
            int num2 = grid.column + roomDesign.Columns;
            if (num1 > shipEntity.PSShip.rows || num2 > shipEntity.PSShip.columns)
                return true;
            for (int column = grid.column; column < num2; ++column)
            {
                for (int row = grid.row; row < num1; ++row)
                {
                    if (Object.op_Inequality((Object)shipEntity.gridMap.grid[column, row].room, (Object)null) || shipEntity.PSShip.mask[column, row] == 0 || (roomDesign.SupportedGridTypes & shipEntity.PSShip.mask[column, row]) == 0)
                        return true;
                }
            }
            return roomDesign.RoomType == RoomType.Lift && (this.GridContainsLift(shipEntity.gridMap.grid[grid.column + 1, grid.row]) || this.GridContainsLift(shipEntity.gridMap.grid[grid.column - 1, grid.row]));
        }

        public bool GridContainsLift(Grid grid)
        {
            if (Object.op_Inequality((Object)grid.room, (Object)null))
                return ((RoomEntity)grid.room.GetComponent<RoomMainEntity>()).Room.RoomDesign.RoomType == RoomType.Lift;
            return false;
        }

        public RoomMainEntity CreateFakeRoom(ShipMainEntity shipEntity, int roomDesignID, int row, int column, RoomManager.RoomButtonClickDelegate buttonDel)
        {
            PSMainRoom psMainRoom = new PSMainRoom();
            psMainRoom.RoomId = -Mathf.Abs((row << 8) + column);
            psMainRoom.locked = true;
            psMainRoom.Row = row;
            psMainRoom.Column = column;
            psMainRoom.RoomDesignId = roomDesignID;
            psMainRoom.ShipId = shipEntity.PSShip.ShipId;
            psMainRoom.ConstructionStartDate = Singleton<SharedManager>.Instance.CurrentTime(false);
            psMainRoom.ManufactureString = string.Empty;
            psMainRoom.ManufactureStartDate = Singleton<SharedManager>.Instance.CurrentTime(false);
            psMainRoom.RoomStatus = RoomStatus.Building;
            shipEntity.PSMainShip.PSMainRooms.Add(psMainRoom);
            return this.CreateRoom(false, (ShipEntity)shipEntity, (PSRoom)psMainRoom, this.GetRoomDesignByID(roomDesignID), buttonDel, true, true) as RoomMainEntity;
        }

        public void BuildNewRoom(ShipMainEntity shipEntity, int roomDesignID, int row, int column, RoomManager.RoomButtonClickDelegate buttonDel, Action<PSServerMessage> endAction = null)
        {
            RoomMainEntity fakeRoom = this.CreateFakeRoom(shipEntity, roomDesignID, row, column, buttonDel);
            this.BuyRoom(roomDesignID, column, row, fakeRoom.Room.ConstructionStartDate.ToString("s"), fakeRoom.Room.RoomId, shipEntity, endAction);
            if (fakeRoom.Room.RoomType != RoomType.Lift)
                return;
            shipEntity.BuildLift();
        }

        public void MoveRoomToGrid(GameObject room, GameObject grid, ShipMainEntity shipEntity, bool playSmokeAnim = true)
        {
            if (!Object.op_Inequality((Object)room, (Object)null) || !Object.op_Inequality((Object)grid, (Object)null) || !Object.op_Inequality((Object)shipEntity, (Object)null))
                return;
            RoomEntity component1 = (RoomEntity)room.GetComponent<RoomEntity>();
            room.get_transform().SetParent(shipEntity.roomLayer.get_transform(), false);
            Transform transform = room.get_transform();
            // ISSUE: variable of the null type
            __Null x = grid.get_transform().get_localPosition().x;
            Rect rect1 = ((RectTransform)((Component)room.get_transform()).GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            double num1 = (double)((Rect)@rect1).get_width() / 2.0;
            double num2 = x + num1;
            // ISSUE: variable of the null type
            __Null y = grid.get_transform().get_localPosition().y;
            Rect rect2 = ((RectTransform)((Component)room.get_transform()).GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            double num3 = (double)((Rect)@rect2).get_height() / 2.0;
            double num4 = y - num3;
            Vector3 vector3 = Vector2.op_Implicit(new Vector2((float)num2, (float)num4));
            transform.set_localPosition(vector3);
            ((Behaviour)room.GetComponent<EventTrigger>()).set_enabled(true);
            GameObject[] gameObjectArray = new GameObject[2];
            GameObject frame = component1.frame;
            float gridWidth = (float)Grid.GRID_WIDTH;
            Vector2 vector2;
            // ISSUE: explicit reference operation
            ((Vector2)@vector2).\u002Ector((float)component1.RoomDesign.Columns * gridWidth, (float)component1.RoomDesign.Rows * gridWidth);
            ((RectTransform)room.GetComponent<RectTransform>()).set_sizeDelta(vector2);
            ((BoxCollider)room.GetComponent<BoxCollider>()).set_size(new Vector3(Mathf.Abs((float)vector2.x), Mathf.Abs((float)vector2.y), 1f));
            Grid component2 = (Grid)grid.GetComponent<Grid>();
            component1.Position = new Vector2Double(grid.get_transform().get_localPosition());
            if (component1.Room.RoomDesign.Rows * component1.Room.RoomDesign.Columns > 1)
            {
                if (Object.op_Inequality((Object)component1.door[0], (Object)null))
                {
                    gameObjectArray[0] = component1.door[0];
                    gameObjectArray[0].get_transform().SetParent(((Component)component1).get_transform(), false);
                    gameObjectArray[0].get_transform().SetAsFirstSibling();
                }
                if (Object.op_Inequality((Object)component1.door[1], (Object)null))
                {
                    gameObjectArray[1] = component1.door[1];
                    gameObjectArray[1].get_transform().SetParent(component1.frame.get_transform(), false);
                    gameObjectArray[1].get_transform().SetAsFirstSibling();
                }
                if (Object.op_Inequality((Object)frame, (Object)null))
                    frame.get_transform().SetParent(shipEntity.frameLayer.get_transform(), false);
            }
            Rect rect3 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            ((Rect)@rect3).get_width();
            Rect rect4 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            ((Rect)@rect4).get_height();
            room.get_transform().set_localPosition(new Vector3((float)component1.LocalPosition.x, (float)component1.LocalPosition.y, 0.0f));
            room.get_transform().set_localScale(Vector3.get_one());
            Rect rect5 = ((RectTransform)room.GetComponent<RectTransform>()).get_rect();
            // ISSUE: explicit reference operation
            float num5 = ((Rect)@rect5).get_width() / 2f;
            if (component1.Room.RoomDesign.Rows * component1.Room.RoomDesign.Columns > 1)
            {
                if (Object.op_Inequality((Object)component1.door[0], (Object)null))
                {
                    gameObjectArray[0].get_transform().set_position(room.get_transform().get_position());
                    gameObjectArray[0].get_transform().set_localPosition(new Vector3((float)gameObjectArray[0].get_transform().get_localPosition().x - num5 + this.doorBackOffsetXNew, (float)gameObjectArray[0].get_transform().get_localPosition().y - this.doorBackOffsetYNew, 0.0f));
                    ((Renderer)gameObjectArray[0].GetComponent<SpriteRenderer>()).set_sortingOrder(1);
                }
                if (Object.op_Inequality((Object)frame, (Object)null))
                    frame.get_transform().set_position(room.get_transform().get_position());
                if (Object.op_Inequality((Object)component1.door[1], (Object)null))
                {
                    gameObjectArray[1].get_transform().set_position(room.get_transform().get_position());
                    gameObjectArray[1].get_transform().set_localPosition(new Vector3((float)gameObjectArray[1].get_transform().get_localPosition().x - num5 + this.doorFrontOffsetXNew, (float)gameObjectArray[1].get_transform().get_localPosition().y - this.doorFrontOffsetYNew, 0.0f));
                    ((Renderer)gameObjectArray[1].GetComponent<SpriteRenderer>()).set_sortingOrder(2);
                }
            }
            foreach (CharacterEntity characterEntity in component1.friendlyCharactersInRoom.ToArray())
            {
                SingletonManager<CharacterManager>.Instance.ResetCrewPosition(characterEntity, (ShipEntity)TempSingleton<MainSceneController>.Instance.playerShipEntity);
                characterEntity.InitializeAnimation();
            }
            RoomDesign roomDesign = component1.Room.RoomDesign;
            if (Object.op_Inequality((Object)component1.exterior, (Object)null))
            {
                Rect rect6 = ((RectTransform)((Component)component1).GetComponent<RectTransform>()).get_rect();
                // ISSUE: explicit reference operation
                Vector2 center = ((Rect)@rect6).get_center();
                component1.exterior.get_transform().SetParent(((Component)component1).get_transform());
                component1.exterior.get_transform().set_localScale(Vector3.get_one());
                component1.exterior.get_transform().set_localPosition(new Vector3((float)center.x, (float)center.y, 0.0f));
                component1.exterior.get_transform().SetParent(shipEntity.exteriorWeaponsLayer.get_transform());
                component1.exterior.SetActive(true);
            }
            int num6 = roomDesign != null ? roomDesign.Rows : 0;
            int num7 = roomDesign != null ? roomDesign.Columns : 0;
            int num8 = component1.Room.Row + num6;
            int num9 = component1.Room.Column + num7;
            if (num8 <= shipEntity.PSShip.rows && num9 <= shipEntity.PSShip.columns)
            {
                for (int column = component1.Room.Column; column < num9; ++column)
                {
                    for (int row = component1.Room.Row; row < num8; ++row)
                        shipEntity.gridMap.grid[column, row].room = (GameObject)null;
                }
            }
            RoomMainEntity roomMainEntity = component1 as RoomMainEntity;
            if (roomMainEntity != null)
                roomMainEntity.CheckRoomResources();
            int num10 = component2.row + num6;
            int num11 = component2.column + num7;
            if (num10 <= shipEntity.PSShip.rows && num11 <= shipEntity.PSShip.columns)
            {
                for (int column = component2.column; column < num11; ++column)
                {
                    for (int row = component2.row; row < num10; ++row)
                        shipEntity.gridMap.grid[column, row].room = room;
                }
            }
            component1.Room.Row = component2.row;
            component1.Room.Column = component2.column;
            if (Object.op_Inequality((Object)room.GetComponent<AnimationObjectNew>(), (Object)null))
            {
                if (component1.Room.RoomType != RoomType.Storage && component1.Room.RoomStatus == RoomStatus.Normal)
                    ((AnimationObjectNew)room.GetComponent<AnimationObjectNew>()).PlayAnimation(true, false, (Action)null);
                else
                    component1.SetRoomAnimationFrame();
            }
            component1.CheckNotifications();
            double? nullable = roomMainEntity != null ? new double?(roomMainEntity.PSMainRoom.ManufactureTimeRemaining) : new double?();
            if ((!nullable.HasValue ? 0 : (nullable.GetValueOrDefault() > 0.0 ? 1 : 0)) != 0)
                component1.DelayedNotificationCheck(roomMainEntity.PSMainRoom.ManufactureTimeRemaining);
            if (roomDesign != null && roomDesign.RoomType == RoomType.Lift)
                shipEntity.BuildLift();
            if (playSmokeAnim)
                component1.CreateBuildSmokeParticles();
            component1.UpdateSurroundingGrids();
        }

        public void StoreRoom(RoomMainEntity roomEntity, ShipEntity shipEntity)
        {
            roomEntity.CollectResources(false, false, true);
            RoomDesign roomDesign = roomEntity.Room.RoomDesign;
            int num1 = roomEntity.Room.Row + roomDesign.Rows;
            int num2 = roomEntity.Room.Column + roomDesign.Columns;
            if (num1 <= shipEntity.PSShip.rows && num2 <= shipEntity.PSShip.columns)
            {
                for (int column = roomEntity.Room.Column; column < num2; ++column)
                {
                    for (int row = roomEntity.Room.Row; row < num1; ++row)
                        shipEntity.gridMap.grid[column, row].room = (GameObject)null;
                }
            }
            roomEntity.Room.RoomStatus = RoomStatus.Inventory;
            roomEntity.Room.Row = -1;
            roomEntity.Room.Column = -1;
            roomEntity.CheckNotifications();
            foreach (CharacterEntity characterEntity in roomEntity.friendlyCharactersInRoom.ToArray())
            {
                characterEntity.StopWander();
                SingletonManager<CharacterManager>.Instance.StoreCharacter(characterEntity);
            }
            if (roomDesign.RoomType == RoomType.Lift)
                shipEntity.BuildLift();
            if (!Object.op_Inequality((Object)roomEntity.exterior, (Object)null))
                return;
            roomEntity.exterior.SetActive(false);
        }

        public void DownloadRoomDesigns(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadRoomDesigns(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomDesigns2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadRoomDesigns(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadRoomDesigns\u003Ec__Iterator0() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseRoomDesign(string xml)
        {
            this.roomDesignList.Add(PSObject.Deserialize<RoomDesign>(xml));
        }

        private void ParseRoomDesigns(string xml)
        {
            foreach (RoomDesign roomDesign in RoomDesignsContainer.LoadFromText(xml).roomDesigns)
                this.roomDesignList.Add(roomDesign);
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("RoomDesignVersion", (float)SingletonManager<SystemManager>.Instance.settings.RoomDesignVersion)), false);
        }

        public void LoadRoomDesigns(SimpleManager.DownloadDelegate del)
        {
            this.roomDesignList.Clear();
            this.LoadXML("RoomDesigns.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomDesigns2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, xmlText, "RoomDesigns", new SimpleManager.ParseDelegate(this.ParseRoomDesigns), (Action)null)));
        }

        public void DownloadMissileDesigns(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadMissileDesigns(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListMissileDesigns", del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadMissileDesigns(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadMissileDesigns\u003Ec__Iterator1() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseMissileDesign(string xml)
        {
            this.missileDesignList.Add(PSObject.Deserialize<MissileDesign>(xml));
        }

        private void ParseMissileDesigns(string xml)
        {
            foreach (MissileDesign missileDesign in MissileDesignContainer.LoadFromText(xml).missileDesigns)
                this.missileDesignList.Add(missileDesign);
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("MissileDesignVersion", (float)SingletonManager<SystemManager>.Instance.settings.MissileDesignVersion)), false);
        }

        public void LoadMissileDesigns(SimpleManager.DownloadDelegate del)
        {
            this.missileDesignList.Clear();
            this.LoadXML("MissileDesigns.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListMissileDesigns", del, xmlText, "MissileDesigns", new SimpleManager.ParseDelegate(this.ParseMissileDesigns), (Action)null)));
        }

        public void DownloadRoomDesignSprites(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadRoomDesignSprites(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomDesignSpriteService/ListRoomDesignSprites", del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadRoomDesignSprites(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadRoomDesignSprites\u003Ec__Iterator2() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseRoomDesignSprite(string xml)
        {
            RoomDesignSprite roomDesignSprite = PSObject.Deserialize<RoomDesignSprite>(xml);
            if (this.roomDesignSpriteDictionary.ContainsKey(roomDesignSprite.RoomDesignId))
                this.roomDesignSpriteDictionary[roomDesignSprite.RoomDesignId].Add(roomDesignSprite);
            else
                this.roomDesignSpriteDictionary.Add(roomDesignSprite.RoomDesignId, new List<RoomDesignSprite>()
        {
          roomDesignSprite
        });
        }

        private void ParseRoomDesignSprites(string xml)
        {
            foreach (RoomDesignSprite roomDesignSprite in RoomDesignSpriteContainer.LoadFromText(xml).roomDesignSprites)
            {
                if (this.roomDesignSpriteDictionary.ContainsKey(roomDesignSprite.RoomDesignId))
                    this.roomDesignSpriteDictionary[roomDesignSprite.RoomDesignId].Add(roomDesignSprite);
                else
                    this.roomDesignSpriteDictionary.Add(roomDesignSprite.RoomDesignId, new List<RoomDesignSprite>()
          {
            roomDesignSprite
          });
            }
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("RoomDesignSpriteVersion", (float)SingletonManager<SystemManager>.Instance.settings.RoomDesignSpriteVersion)), false);
        }

        public void LoadRoomDesignSprites(SimpleManager.DownloadDelegate del)
        {
            this.roomDesignSpriteDictionary.Clear();
            this.LoadXML("RoomDesignSprites.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomDesignSpriteService/ListRoomDesignSprites", del, xmlText, "RoomDesignSprites", new SimpleManager.ParseDelegate(this.ParseRoomDesignSprites), (Action)null)));
        }

        public void DownloadRoomDesignPurchases(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadRoomDesignPurchases(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomDesignPurchase", del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadRoomDesignPurchases(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadRoomDesignPurchases\u003Ec__Iterator3() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private PSRoomDesignPurchase ParseRoomDesignPurchase(string xml)
        {
            return PSObject.Deserialize<PSRoomDesignPurchase>(xml);
        }

        public void LoadRoomDesignPurchases(SimpleManager.DownloadDelegate del)
        {
            this.LoadXML("RoomDesignPurchases.txt", (Action<string>)(xmlText => this.LoadRoomDesignPurchases(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomDesignPurchase", del, xmlText)));
        }

        public void LoadRoomDesignPurchases(string url, SimpleManager.DownloadDelegate del, string xml)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            RoomManager.\u003CLoadRoomDesignPurchases\u003Ec__AnonStorey24 purchasesCAnonStorey24 = new RoomManager.\u003CLoadRoomDesignPurchases\u003Ec__AnonStorey24();
            // ISSUE: reference to a compiler-generated field
            purchasesCAnonStorey24.xml = xml;
            // ISSUE: reference to a compiler-generated field
            purchasesCAnonStorey24.del = del;
            // ISSUE: reference to a compiler-generated field
            purchasesCAnonStorey24.url = url;
            // ISSUE: reference to a compiler-generated field
            purchasesCAnonStorey24.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            purchasesCAnonStorey24.sharedManager = Singleton<SharedManager>.Instance;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated method
            purchasesCAnonStorey24.sharedManager.RunActionAsync(nameof(LoadRoomDesignPurchases), new Action(purchasesCAnonStorey24.\u003C\u003Em__0), false);
        }

        public void DownloadCraftDesigns(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadCraftDesigns(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListCraftDesigns", del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadCraftDesigns(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadCraftDesigns\u003Ec__Iterator4() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseCraftDesign(string xml)
        {
            this.craftDesignStack.Push((object)PSObject.Deserialize<CraftDesign>(xml));
        }

        private void ParseCraftDesigns(string xml)
        {
            foreach (object craftDesign in CraftDesignContainer.LoadFromText(xml).craftDesigns)
                this.craftDesignStack.Push(craftDesign);
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("CraftDesignVersion", (float)SingletonManager<SystemManager>.Instance.settings.CraftDesignVersion)), false);
        }

        public void LoadCraftDesigns(SimpleManager.DownloadDelegate del)
        {
            this.craftDesignStack.Clear();
            this.LoadXML("CraftDesigns.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListCraftDesigns", del, xmlText, "CraftDesigns", new SimpleManager.ParseDelegate(this.ParseCraftDesigns), (Action)null)));
        }

        public void BuyRoom(int roomDesignId, int column, int row, string constructionStartDate, int roomId, ShipMainEntity shipEntity, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.BuyRoom(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/BuyRoom2?roomDesignId=" + (object)roomDesignId + "&column=" + (object)column + "&row=" + (object)row + "&constructionStartDate=" + constructionStartDate + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomDesignId, column, row, constructionStartDate, roomId, shipEntity, endAction));
        }

        [DebuggerHidden]
        private IEnumerator BuyRoom(string url, int roomDesignId, int column, int row, string constructionStartDate, int roomId, ShipMainEntity shipEntity, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CBuyRoom\u003Ec__Iterator5() { roomDesignId = roomDesignId, column = column, row = row, constructionStartDate = constructionStartDate, url = url, endAction = endAction, shipEntity = shipEntity, roomId = roomId, \u0024this = this };
        }

        public void DownloadRooms(SimpleManager.DownloadStackDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadRooms(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomsViaAccessToken?accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadRooms(string url, SimpleManager.DownloadStackDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadRooms\u003Ec__Iterator6() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        public void SortRoomDesignPurchasesById()
        {
            this.roomDesignPurchaseList = this.roomDesignPurchaseList.OrderBy<PSRoomDesignPurchase, int>((Func<PSRoomDesignPurchase, int>)(o => o.RoomDesignId)).ThenByDescending<PSRoomDesignPurchase, int>((Func<PSRoomDesignPurchase, int>)(o => o.Level)).ToList<PSRoomDesignPurchase>();
        }

        public Stack SortRoomsByID(Stack roomStack)
        {
            Stack stack = new Stack();
            List<PSRoom> psRoomList = new List<PSRoom>();
            while (roomStack.Count > 0)
            {
                PSRoom psRoom1 = roomStack.Pop() as PSRoom;
                foreach (PSRoom psRoom2 in roomStack.ToArray())
                {
                    if (psRoom2.RoomId < psRoom1.RoomId)
                    {
                        roomStack.Push((object)psRoom1);
                        psRoom1 = roomStack.Pop() as PSRoom;
                    }
                }
                psRoomList.Add(psRoom1);
            }
            foreach (PSRoom psRoom in psRoomList.ToArray())
                stack.Push((object)psRoom);
            return stack;
        }

        public List<T> SortRoomsByID<T>(List<T> roomList) where T : PSRoom
        {
            List<T> objList = new List<T>();
            return roomList.OrderBy<T, int>((Func<T, int>)(o => o.RoomId)).ToList<T>();
        }

        public ObservableCollection<T> SortRoomsByID<T>(ObservableCollection<T> roomList) where T : PSRoom
        {
            return new ObservableCollection<T>((IEnumerable<T>)roomList.OrderBy<T, int>((Func<T, int>)(i => i.RoomId)));
        }

        public void ReadRoom(string url, SimpleManager.DownloadStackDelegate del, string text, string name)
        {
            this.ReadRoomXML(url, del, text, name);
        }

        public void ReadRoomXML(string url, SimpleManager.DownloadStackDelegate del, string text, string name)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            RoomManager.\u003CReadRoomXML\u003Ec__AnonStorey29 xmlCAnonStorey29 = new RoomManager.\u003CReadRoomXML\u003Ec__AnonStorey29();
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.text = text;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.name = name;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.del = del;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.url = url;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey29.sharedManager = Singleton<SharedManager>.Instance;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated method
            xmlCAnonStorey29.sharedManager.RunActionAsync("ReadRoom", new Action(xmlCAnonStorey29.\u003C\u003Em__0), false);
        }

        public void ReadRooms(string url, SimpleManager.DownloadStackDelegate del, string text, string name, Action failAction = null)
        {
            this.ReadRoomsXML(url, del, text, name, (Action)null);
        }

        public void ReadRoomsXML(string url, SimpleManager.DownloadStackDelegate del, string text, string name, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            RoomManager.\u003CReadRoomsXML\u003Ec__AnonStorey2B xmlCAnonStorey2B = new RoomManager.\u003CReadRoomsXML\u003Ec__AnonStorey2B();
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.text = text;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.name = name;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.del = del;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.url = url;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.failAction = failAction;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            xmlCAnonStorey2B.sharedManager = Singleton<SharedManager>.Instance;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated method
            xmlCAnonStorey2B.sharedManager.RunActionAsync("ReadRooms", new Action(xmlCAnonStorey2B.\u003C\u003Em__0), false);
        }

        public T ParseRoom<T>(string xml) where T : PSRoom
        {
            return PSObject.Deserialize<T>(xml);
        }

        public List<T> ParseRooms<T>(string xml) where T : PSRoom
        {
            return RoomContainer<T>.LoadFromText(xml).rooms;
        }

        public void LoadRooms(SimpleManager.DownloadStackDelegate del)
        {
            this.LoadXML("Rooms.txt", (Action<string>)(xmlText => this.ReadRooms(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListRoomsViaAccessToken?accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, xmlText, "Rooms", (Action)null)));
        }

        public void DownloadRoomActions(SimpleManager.DownloadDelegate del, int shipId, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadRoomActions(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/ListAllRoomActionsOfShip?shipId=" + (object)shipId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadRoomActions(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDownloadRoomActions\u003Ec__Iterator7() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        public MissileDesign GetMissileDesignById(int missileDesignID)
        {
            foreach (MissileDesign missileDesign in this.missileDesignList.ToArray())
            {
                if (missileDesign.MissileDesignId == missileDesignID)
                    return missileDesign;
            }
            return (MissileDesign)null;
        }

        public CraftDesign GetCraftDesignById(int craftDesignID)
        {
            foreach (CraftDesign craftDesign in this.craftDesignStack.ToArray())
            {
                if (craftDesign.CraftDesignId == craftDesignID)
                    return craftDesign;
            }
            return (CraftDesign)null;
        }

        public void UpgradeRoom(int roomId, int upgradeRoomDesignId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.UpgradeRoom(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/UpgradeRoom?roomId=" + (object)roomId + "&upgradeRoomDesignId=" + (object)upgradeRoomDesignId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, upgradeRoomDesignId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator UpgradeRoom(string url, int roomId, int upgradeRoomDesignId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CUpgradeRoom\u003Ec__Iterator8() { roomId = roomId, upgradeRoomDesignId = upgradeRoomDesignId, url = url, endAction = endAction };
        }

        public List<RoomDesign> GetNextLevelRoomDesigns(int roomDesignID)
        {
            List<RoomDesign> roomDesignList = new List<RoomDesign>();
            foreach (RoomDesign roomDesign in this.roomDesignList.ToArray())
            {
                if (roomDesign.UpgradeFromRoomDesignId == roomDesignID)
                    roomDesignList.Add(roomDesign);
            }
            return roomDesignList;
        }

        public void MoveRoom(PSRoom psRoom, int row, int column, Action<PSServerMessage> endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.MoveRoom(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/MoveRoom?roomId=" + (object)psRoom.RoomId + "&row=" + (object)row + "&column=" + (object)column + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, psRoom, row, column, endAction));
        }

        [DebuggerHidden]
        private IEnumerator MoveRoom(string url, PSRoom psRoom, int row, int column, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CMoveRoom\u003Ec__Iterator9() { psRoom = psRoom, row = row, column = column, url = url, endAction = endAction, \u0024this = this };
        }

        public void RemoveRoom(int shipId, int roomId, Action<PSServerMessage> endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.RemoveRoom(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/RemoveRoom?shipId=" + (object)shipId + "&roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipId, roomId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator RemoveRoom(string url, int shipId, int roomId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CRemoveRoom\u003Ec__IteratorA() { shipId = shipId, roomId = roomId, url = url, endAction = endAction };
        }

        public List<RoomDesignSprite> GetRoomDesignSpritesWithRoomDesignId(int roomDesignId)
        {
            List<RoomDesignSprite> roomDesignSpriteList = new List<RoomDesignSprite>();
            foreach (RoomDesignSprite roomDesignSprite in roomDesignSpriteList.ToArray())
            {
                if (roomDesignSprite.RoomDesignId == roomDesignId)
                    roomDesignSpriteList.Add(roomDesignSprite);
            }
            return roomDesignSpriteList;
        }

        public void DeleteRoomAction(int roomActionId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.DeleteRoomAction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/DeleteRoomAction?roomActionId=" + (object)roomActionId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomActionId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator DeleteRoomAction(string url, int roomActionId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CDeleteRoomAction\u003Ec__IteratorB() { roomActionId = roomActionId, url = url, endAction = endAction };
        }

        public void PasteRoomActions(int roomId, int toRoomId, Action<PSServerMessage, List<PSRoomAction>> endAction)
        {
            this.StartCoroutine(this.PasteRoomActions(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/CopyRoomActions?roomId=" + (object)roomId + "&toRoomId=" + (object)toRoomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, toRoomId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator PasteRoomActions(string url, int roomId, int toRoomId, Action<PSServerMessage, List<PSRoomAction>> endAction)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CPasteRoomActions\u003Ec__IteratorC() { roomId = roomId, toRoomId = toRoomId, url = url, endAction = endAction, \u0024this = this };
        }

        public void MoveRoomAction(int roomActionId, int newIndex, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.MoveRoomAction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/MoveRoomAction?roomActionId=" + (object)roomActionId + "&newIndex=" + (object)newIndex + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomActionId, newIndex, endAction));
        }

        [DebuggerHidden]
        private IEnumerator MoveRoomAction(string url, int roomActionId, int newIndex, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CMoveRoomAction\u003Ec__IteratorD() { roomActionId = roomActionId, newIndex = newIndex, url = url, endAction = endAction };
        }

        public void CreateRoomAction(int roomId, int roomActionIndex, int actionTypeId, int conditionTypeId, Action<PSServerMessage, PSRoomAction> endAction = null)
        {
            this.StartCoroutine(this.CreateRoomAction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/CreateRoomAction?roomId=" + (object)roomId + "&roomActionIndex=" + (object)roomActionIndex + "&actionTypeId=" + (object)actionTypeId + "&conditionTypeId=" + (object)conditionTypeId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, roomActionIndex, actionTypeId, conditionTypeId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator CreateRoomAction(string url, int roomId, int roomActionIndex, int actionTypeId, int conditionTypeId, Action<PSServerMessage, PSRoomAction> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CCreateRoomAction\u003Ec__IteratorE() { roomId = roomId, roomActionIndex = roomActionIndex, actionTypeId = actionTypeId, conditionTypeId = conditionTypeId, url = url, endAction = endAction, \u0024this = this };
        }

        private PSRoomAction ParseRoomAction(string xml)
        {
            return PSObject.Deserialize<PSRoomAction>(xml);
        }

        public void EditRoomAction(int roomActionId, int actionTypeId, int conditionTypeId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.EditRoomAction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/EditRoomAction?roomActionId=" + (object)roomActionId + "&actionTypeId=" + (object)actionTypeId + "&conditionTypeId=" + (object)conditionTypeId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomActionId, actionTypeId, conditionTypeId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator EditRoomAction(string url, int roomActionId, int actionTypeId, int conditionTypeId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CEditRoomAction\u003Ec__IteratorF() { roomActionId = roomActionId, actionTypeId = actionTypeId, conditionTypeId = conditionTypeId, url = url, endAction = endAction };
        }

        public void CancelRoomConstruction(int roomId, Action<PSServerMessage> endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.CancelRoomConstruction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/CancelRoomUpgrade?roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator CancelRoomConstruction(string url, int roomId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CCancelRoomConstruction\u003Ec__Iterator10() { roomId = roomId, url = url, endAction = endAction };
        }

        public void RushRoomConstruction(int roomId, Action<PSServerMessage> endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.RushRoomConstruction(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/SpeedUpRoomConstructionUsingCredits?roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator RushRoomConstruction(string url, int roomId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CRushRoomConstruction\u003Ec__Iterator11() { roomId = roomId, url = url, endAction = endAction };
        }

        public int GetNumberOfRoomsAvailableToPurchase(CategoryType category, PSShip psShip)
        {
            int shipLevel = psShip.ShipDesign.ShipLevel;
            int num1 = 0;
            int num2 = SingletonManager<UserManager>.Instance.user.UserType != UserType.UserTypeAlliance ? 1 : 2;
            List<int> intList = new List<int>();
            foreach (PSRoomDesignPurchase roomDesignPurchase in this.roomDesignPurchaseList.ToArray())
            {
                if (shipLevel >= roomDesignPurchase.Level && category == roomDesignPurchase.RoomDesign.CategoryType && (!intList.Contains(roomDesignPurchase.RoomDesign.RoomDesignId) && (roomDesignPurchase.AvailabilityMask & num2) > 0))
                {
                    int num3 = 0;
                    foreach (PSRoom psRoom in ((IEnumerable<PSRoom>)psShip.Rooms).ToArray<PSRoom>())
                    {
                        if (psRoom.RoomDesign.RootRoomDesignId == roomDesignPurchase.RoomDesign.RoomDesignId)
                            ++num3;
                    }
                    num1 += Mathf.Max(0, roomDesignPurchase.TotalQuantity - num3);
                    intList.Add(roomDesignPurchase.RoomDesign.RoomDesignId);
                }
            }
            return num1;
        }

        public void RefillRoom(int roomId)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.RefillRoom(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/Refill?roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId));
        }

        [DebuggerHidden]
        private IEnumerator RefillRoom(string url, int roomId)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CRefillRoom\u003Ec__Iterator12() { roomId = roomId, url = url };
        }

        public void BuyMissile(int roomId, string itemDesignString, string manufactureStartDate, Action<PSServerMessage, List<PSMainRoom>> endAction = null)
        {
            this.StartCoroutine(this.BuyMissile(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/BuyMissile3?roomId=" + (object)roomId + "&itemDesignString=" + itemDesignString + "&manufactureStartDate=" + manufactureStartDate + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, itemDesignString, manufactureStartDate, endAction));
        }

        [DebuggerHidden]
        private IEnumerator BuyMissile(string url, int roomId, string itemDesignString, string manufactureStartDate, Action<PSServerMessage, List<PSMainRoom>> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CBuyMissile\u003Ec__Iterator13() { roomId = roomId, itemDesignString = itemDesignString, manufactureStartDate = manufactureStartDate, url = url, endAction = endAction };
        }

        public void RushMissileBuild(int roomId)
        {
            this.StartCoroutine(this.RushMissileBuild(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/SpeedUpBuildingMissilesUsingCredits?roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId));
        }

        [DebuggerHidden]
        private IEnumerator RushMissileBuild(string url, int roomId)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CRushMissileBuild\u003Ec__Iterator14() { roomId = roomId, url = url };
        }

        public void CollectResources(int roomId, int amount, string collectDate)
        {
            this.StartCoroutine(this.CollectResourcesIEnumerator(roomId, amount, collectDate));
        }

        [DebuggerHidden]
        private IEnumerator CollectResourcesIEnumerator(int roomId, int amount, string collectDate)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CCollectResourcesIEnumerator\u003Ec__Iterator15() { collectDate = collectDate, roomId = roomId, amount = amount };
        }

        public void UpdateItemsForRoom(int roomId, string itemIds)
        {
            AlertController.ShowServerLoadIcon();
            string clientDateTime = Singleton<SharedManager>.Instance.CurrentTime(false).ToString("s");
            string checksum = Singleton<SharedManager>.Instance.SavysodaEncryptString(roomId.ToString() + itemIds + clientDateTime + SingletonManager<UserManager>.Instance.userLogin.accessToken + SingletonManager<Configuration>.Instance.ChecksumKey);
            this.StartCoroutine(this.UpdateItemsForRoom(roomId, itemIds, checksum, clientDateTime));
        }

        [DebuggerHidden]
        private IEnumerator UpdateItemsForRoom(int roomId, string itemIds, string checksum, string clientDateTime)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CUpdateItemsForRoom\u003Ec__Iterator16() { roomId = roomId, itemIds = itemIds, checksum = checksum, clientDateTime = clientDateTime };
        }

        public void MoveItemToRoom(int roomId, int toRoomId, int itemId)
        {
            AlertController.ShowServerLoadIcon();
            string clientDateTime = Singleton<SharedManager>.Instance.CurrentTime(false).ToString("s");
            string checksum = Singleton<SharedManager>.Instance.SavysodaEncryptString(roomId.ToString() + toRoomId.ToString() + itemId.ToString() + clientDateTime + SingletonManager<UserManager>.Instance.userLogin.accessToken + SingletonManager<Configuration>.Instance.ChecksumKey);
            this.StartCoroutine(this.MoveItemToRoom(roomId, toRoomId, itemId, checksum, clientDateTime));
        }

        [DebuggerHidden]
        private IEnumerator MoveItemToRoom(int roomId, int toRoomId, int itemId, string checksum, string clientDateTime)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CMoveItemToRoom\u003Ec__Iterator17() { roomId = roomId, toRoomId = toRoomId, itemId = itemId, checksum = checksum, clientDateTime = clientDateTime };
        }

        public void CollectSalvage(int roomId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.CollectSalvage(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/CollectSalvageItems?roomId=" + (object)roomId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, roomId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator CollectSalvage(string url, int roomId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CCollectSalvage\u003Ec__Iterator18() { roomId = roomId, url = url, endAction = endAction };
        }

        public void CollectAllResources(string itemType, Action<PSServerMessage> endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            string collectDate = Singleton<SharedManager>.Instance.CurrentTime(false).ToString("s");
            this.StartCoroutine(this.CollectAllResources(SingletonManager<Configuration>.Instance.ServerUrl + "/RoomService/CollectAllResources?itemType=" + itemType + "&collectDate=" + collectDate + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, itemType, collectDate, endAction));
        }

        [DebuggerHidden]
        private IEnumerator CollectAllResources(string url, string itemType, string collectDate, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CCollectAllResources\u003Ec__Iterator19() { itemType = itemType, collectDate = collectDate, url = url, endAction = endAction };
        }

        public void RebuildAmmo(AmmoCategory ammoCategory, Action<PSServerMessage, List<PSMainRoom>, List<PSMainItem>> endAction = null)
        {
            string clientDateTime = Singleton<SharedManager>.Instance.CurrentTime(false).ToString("s");
            string checksum = Singleton<SharedManager>.Instance.SavysodaEncryptString(ammoCategory.ToString() + clientDateTime + SingletonManager<UserManager>.Instance.userLogin.accessToken + SingletonManager<Configuration>.Instance.ChecksumKey);
            this.StartCoroutine(this.RebuildAmmo(ammoCategory, clientDateTime, checksum, endAction));
        }

        [DebuggerHidden]
        private IEnumerator RebuildAmmo(AmmoCategory ammoCategory, string clientDateTime, string checksum, Action<PSServerMessage, List<PSMainRoom>, List<PSMainItem>> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CRebuildAmmo\u003Ec__Iterator1A() { ammoCategory = ammoCategory, clientDateTime = clientDateTime, checksum = checksum, endAction = endAction, \u0024this = this };
        }

        public void SwapRoom(int roomId, int swapRoomDesignId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.SwapRoomIEnumerator(roomId, swapRoomDesignId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator SwapRoomIEnumerator(int roomId, int swapRoomDesignId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new RoomManager.\u003CSwapRoomIEnumerator\u003Ec__Iterator1B() { roomId = roomId, swapRoomDesignId = swapRoomDesignId, endAction = endAction };
        }

        public delegate PSRoom ParseRoomDelegate(string xml);

        public delegate void RoomButtonClickDelegate(GameObject button, EventTriggerType type, PointerEventData data);
    }
}