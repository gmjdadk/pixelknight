using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using PixelKnight.Models;
using PixelStarships;

namespace PixelKnight.Managers
{
    public class ShipManager
    {
        public Dictionary<int, ShipDesign> shipDesignDict = new Dictionary<int, ShipDesign>();
        private PSMainShip _playerPSShip;
        public GameObject gridBoxPrefab;
        public GameObject thrustersPrefab;

        protected ShipManager()
        {
        }

        public PSMainShip PlayerShip
        {
            get
            {
                return this._playerPSShip;
            }
        }

        public PSShip TestPlayerShip { get; set; }

        public void ResetPlayerShip(PSMainShip ship = null)
        {
            this._playerPSShip = ship;
        }

        private void Awake()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            // ISSUE: explicit reference operation
            if (((Scene)@activeScene).get_name() != "LoadingScene")
                this.LoadShipDesigns((SimpleManager.DownloadDelegate)null);
            this.gridBoxPrefab = Resources.Load("GridBox") as GameObject;
            this.thrustersPrefab = Resources.Load("ThrustersNew") as GameObject;
        }

        public ShipDesign GetShipDesignById(int shipDesignId)
        {
            if (!this.shipDesignDict.ContainsKey(shipDesignId))
                return (ShipDesign)null;
            return this.shipDesignDict[shipDesignId];
        }

        public ShipMainEntity CreateMainShipObject(PSShip psShip, GameObject shipLayer)
        {
            GameObject gameObject = Object.Instantiate(Resources.Load("ShipMainPrefab"), shipLayer.get_transform()) as GameObject;
            ((Object)gameObject).set_name(psShip.ShipName + "-" + (object)psShip.ShipId);
            gameObject.get_transform().set_localScale(Vector3.get_one());
            gameObject.get_transform().set_position(new Vector3(99999f, 0.0f, 0.0f));
            ShipMainEntity component = (ShipMainEntity)gameObject.GetComponent<ShipMainEntity>();
            component.shipEntity = (ShipEntity)component;
            component.PSShip = psShip;
            component.gridMap = new GridMap(new Grid[psShip.columns, psShip.rows]);
            component.PSShip.ConvertedHp = psShip.Hp * 100.0;
            component.PSShip.InitialHP = component.PSShip.ConvertedHp;
            return component;
        }

        public ShipBattleEntity CreateBattleShipObject(PSShip psShip, GameObject shipLayer)
        {
            GameObject gameObject = Object.Instantiate(Resources.Load("ShipBattlePrefab"), shipLayer.get_transform()) as GameObject;
            ((Object)gameObject).set_name(psShip.ShipName + "-" + (object)psShip.ShipId);
            gameObject.get_transform().set_localScale(Vector3.get_one());
            gameObject.get_transform().set_position(new Vector3(99999f, 0.0f, 0.0f));
            ShipBattleEntity component = (ShipBattleEntity)gameObject.GetComponent<ShipBattleEntity>();
            component.shipEntity = (ShipEntity)component;
            component.PSShip = psShip;
            component.gridMap = new GridMap(new Grid[psShip.columns, psShip.rows]);
            component.PSShip.ConvertedHp = psShip.Hp * 100.0;
            component.PSShip.InitialHP = component.PSShip.ConvertedHp;
            return component;
        }

        public void AddShipButtonDelegate(EventTrigger eventTrigger, SimpleManager.GenericDelegate buttonDel)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ShipManager.\u003CAddShipButtonDelegate\u003Ec__AnonStoreyD delegateCAnonStoreyD = new ShipManager.\u003CAddShipButtonDelegate\u003Ec__AnonStoreyD();
            // ISSUE: reference to a compiler-generated field
            delegateCAnonStoreyD.buttonDel = buttonDel;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = (__Null)2;
            // ISSUE: method pointer
            ((UnityEvent<BaseEventData>)entry.callback).AddListener(new UnityAction<BaseEventData>((object)delegateCAnonStoreyD, __methodptr(\u003C\u003Em__0)));
            eventTrigger.get_triggers().Add(entry);
        }

        public void InitializeShipSprites(ShipEntity shipEntity, PSShip psShip)
        {
            Vector2 vector2;
            // ISSUE: explicit reference operation
            ((Vector2)@vector2).\u002Ector((float)psShip.columns * 25f, (float)psShip.rows * 25f);
            Sprite unitySprite1 = psShip.ShipDesign.InteriorSprite.UnitySprite;
            ((SpriteRenderer)shipEntity.shipInterior.GetComponent<SpriteRenderer>()).set_sprite(unitySprite1);
            Sprite unitySprite2 = psShip.ShipDesign.ExteriorSprite.UnitySprite;
            ((SpriteRenderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).set_sprite(unitySprite2);
            Vector2 size = ((SpriteRenderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).get_size();
            ((BoxCollider)shipEntity.shipExterior.GetComponent<BoxCollider>()).set_size(Vector2.op_Implicit(new Vector2(Mathf.Abs((float)size.x), Mathf.Abs((float)size.y))));
            ((SpriteRenderer)shipEntity.skinLayer.GetComponent<SpriteRenderer>()).set_sprite(unitySprite2);
            ((SpriteMask)shipEntity.stickerLayer.GetComponent<SpriteMask>()).set_sprite(unitySprite2);
            ShipBattleEntity shipBattleEntity = shipEntity as ShipBattleEntity;
            Material material = Object.Instantiate(Resources.Load("SpriteMask")) as Material;
            if (Object.op_Inequality((Object)shipBattleEntity, (Object)null))
            {
                ((SpriteRenderer)shipBattleEntity.cloakOverlayLayer.GetComponent<SpriteRenderer>()).set_sprite(unitySprite2);
                ((Renderer)shipBattleEntity.cloakOverlayLayer.GetComponent<SpriteRenderer>()).set_material(material);
            }
          ((SpriteRenderer)shipEntity.innerLayer.GetComponent<SpriteRenderer>()).set_sprite(unitySprite1);
            ((Renderer)shipEntity.innerLayer.GetComponent<SpriteRenderer>()).set_material(material);
            Sprite unitySprite3 = psShip.ShipDesign.ShieldSprite.UnitySprite;
            SpriteRenderer component = (SpriteRenderer)shipEntity.shipShield.GetComponent<SpriteRenderer>();
            component.set_sprite(unitySprite3);
            component.set_drawMode((SpriteDrawMode)1);
            component.set_size(vector2);
            component.set_drawMode((SpriteDrawMode)0);
            SpriteDesign spriteDesign = psShip.SkinItemDesignId != 0 ? psShip.SkinItemDesign.LogoSprite : SingletonManager<SpriteManager>.Instance.GetSpriteByKey("blankPixel");
            ((SpriteRenderer)shipEntity.exteriorSkin.GetComponent<SpriteRenderer>()).set_sprite(spriteDesign.UnitySprite);
            ((RectTransform)((Component)shipEntity).GetComponent<RectTransform>()).set_sizeDelta(vector2);
        }

        public void InitializeShipMaterials(ShipEntity shipEntity, PSShip psShip)
        {
            Material material1 = Object.Instantiate(Resources.Load("HSBMaterial")) as Material;
            material1.SetFloat("_Hue", psShip.HueValue);
            material1.SetFloat("_Saturation", psShip.SaturationValue);
            material1.SetFloat("_Brightness", psShip.BrightnessValue);
            shipEntity.sharedMaterial = material1;
            ((Renderer)shipEntity.shipInterior.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
            ((Renderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).set_material(shipEntity.sharedMaterial);
            Material material2 = Object.Instantiate(Resources.Load("SpriteMask")) as Material;
            material2.SetFloat("_Hue", psShip.HueValue);
            material2.SetFloat("_Saturation", psShip.SaturationValue);
            material2.SetFloat("_Brightness", psShip.BrightnessValue);
            shipEntity.skinMaskMaterial = material2;
            ((Renderer)shipEntity.skinLayer.GetComponent<SpriteRenderer>()).set_material(shipEntity.skinMaskMaterial);
            Material material3 = Object.Instantiate(Resources.Load("BlendModeRamp")) as Material;
            material3.SetFloat("_Hue", psShip.HueValue);
            material3.SetFloat("_Saturation", psShip.SaturationValue);
            material3.SetFloat("_Brightness", psShip.BrightnessValue);
            shipEntity.skinMaterial = material3;
            ((Renderer)shipEntity.exteriorSkin.GetComponent<SpriteRenderer>()).set_material(shipEntity.skinMaterial);
        }

        public void SharedShipCreation(ShipEntity shipEntity, PSShip psShip, SimpleManager.GenericDelegate buttonDel)
        {
            this.AddShipButtonDelegate((EventTrigger)shipEntity.clickDetector.GetComponent<EventTrigger>(), buttonDel);
            this.InitializeShipSprites(shipEntity, psShip);
            shipEntity.CreateStickerEntities();
            shipEntity.UpdateFleetFlagIcon();
            this.InitializeShipMaterials(shipEntity, psShip);
            float num1 = 0.0f;
            float num2 = 0.0f;
            if (Object.op_Inequality((Object)((SpriteRenderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).get_sprite(), (Object)null))
            {
                Bounds bounds1 = ((SpriteRenderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).get_sprite().get_bounds();
                // ISSUE: explicit reference operation
                num1 = (float)((Bounds)@bounds1).get_size().x;
                Bounds bounds2 = ((SpriteRenderer)shipEntity.shipExterior.GetComponent<SpriteRenderer>()).get_sprite().get_bounds();
                // ISSUE: explicit reference operation
                num2 = (float)((Bounds)@bounds2).get_size().y;
            }
            if ((double)num1 > (double)num2)
                TransformExtensions.ScaleByXY((Transform)shipEntity.exteriorSkin.GetComponent<RectTransform>(), 3.2f);
            else
                TransformExtensions.ScaleByXY((Transform)shipEntity.exteriorSkin.GetComponent<RectTransform>(), 3.5f);
            ((Component)shipEntity).get_gameObject().SetActive(true);
        }

        public void CreateMainShip(PSShip psShip, GameObject shipLayer, SimpleManager.GenericDelegate buttonDel, ShipManager.ShipCreationDelegate shipDel)
        {
            if (psShip == null || !Object.op_Inequality((Object)shipLayer, (Object)null))
                return;
            ShipMainEntity mainShipObject = this.CreateMainShipObject(psShip, shipLayer);
            this.SharedShipCreation((ShipEntity)mainShipObject, psShip, buttonDel);
            if (shipDel == null)
                return;
            shipDel((ShipEntity)mainShipObject);
        }

        public void CreateBattleShip(PSShip psShip, GameObject shipLayer, SimpleManager.GenericDelegate buttonDel, ShipManager.ShipCreationDelegate shipDel)
        {
            if (psShip == null || !Object.op_Inequality((Object)shipLayer, (Object)null))
                return;
            ShipBattleEntity battleShipObject = this.CreateBattleShipObject(psShip, shipLayer);
            this.SharedShipCreation((ShipEntity)battleShipObject, psShip, buttonDel);
            SceneViewManager instance = SingletonManager<SceneViewManager>.Instance;
            if (instance.CurrentPsBackground != null)
            {
                battleShipObject.cloakEntity.OuterCloak.set_sprite(SingletonManager<SpriteManager>.Instance.GetSpriteById(instance.CurrentPsBackground.BackgroundSpriteId).UnitySprite);
                battleShipObject.cloakEntity.InnerCloak.set_sprite(SingletonManager<SpriteManager>.Instance.GetSpriteById(instance.CurrentPsBackground.BackgroundSpriteId).UnitySprite);
            }
            if (shipDel == null)
                return;
            shipDel((ShipEntity)battleShipObject);
        }

        public void UpgradeShip(int shipDesignId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.UpgradeShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/UpgradeShip?shipDesignId=" + (object)shipDesignId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipDesignId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator UpgradeShip(string url, int shipDesignId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CUpgradeShip\u003Ec__Iterator0() { shipDesignId = shipDesignId, url = url, endAction = endAction };
        }

        public void TransformShip(int shipDesignId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.TransformShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/TransformShip?shipDesignId=" + (object)shipDesignId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipDesignId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator TransformShip(string url, int shipDesignId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CTransformShip\u003Ec__Iterator1() { shipDesignId = shipDesignId, url = url, endAction = endAction };
        }

        public void DownloadShip(ShipManager.DownloadShipDelegate del, int userId, Action failAction = null, bool isTestBattleShip = false)
        {
            this.StartCoroutine(this.DownloadShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/GetShipByUserId?userId=" + (object)userId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, failAction, isTestBattleShip));
        }

        [DebuggerHidden]
        private IEnumerator DownloadShip(string url, ShipManager.DownloadShipDelegate del, Action failAction = null, bool isTestBattleShip = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CDownloadShip\u003Ec__Iterator2() { url = url, del = del, failAction = failAction, isTestBattleShip = isTestBattleShip, \u0024this = this };
        }

        public void ReadShip(string url, ShipManager.DownloadShipDelegate del, string text, string name, Action failAction = null, bool isTestBattleShip = false)
        {
            this.ReadShipXML(url, del, text, name, failAction, isTestBattleShip);
        }

        public void ReadShipXML(string url, ShipManager.DownloadShipDelegate del, string text, string name, Action failAction = null, bool isTestBattleShip = false)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ShipManager.\u003CReadShipXML\u003Ec__AnonStoreyE shipXmlCAnonStoreyE = new ShipManager.\u003CReadShipXML\u003Ec__AnonStoreyE();
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.text = text;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.name = name;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.del = del;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.url = url;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.failAction = failAction;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            shipXmlCAnonStoreyE.sharedManager = Singleton<SharedManager>.Instance;
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated method
            shipXmlCAnonStoreyE.sharedManager.RunActionAsync("ReadShip", new Action(shipXmlCAnonStoreyE.\u003C\u003Em__0), false);
        }

        public void CompleteShipItemsDownload(List<PSItem> itemList, string url, string message, bool success, Action failAction = null)
        {
            if (success)
            {
                PSShip playerShip = (PSShip)SingletonManager<ShipManager>.Instance.PlayerShip;
                if (itemList == null)
                    return;
                foreach (PSItem psItem in itemList)
                    playerShip.AddItemToShip(psItem);
            }
            else
                Debug.LogWarning((object)"Failed Ship Items Download", (Object)null);
        }

        public T ReadShip<T>(string url, string text, string name, bool temp = false) where T : PSShip
        {
            T obj = (T)null;
            bool flag = false;
            try
            {
                XmlTextReader xmlTextReader = new XmlTextReader((TextReader)new StringReader(text));
                while (!xmlTextReader.EOF)
                {
                    if (xmlTextReader.Name == name)
                        obj = this.ParseShip<T>(xmlTextReader.ReadOuterXml(), temp);
                    else
                        xmlTextReader.Read();
                }
            }
            catch
            {
                flag = true;
            }
            return obj;
        }

        public void LoadShip(ShipManager.DownloadShipDelegate del)
        {
            this.LoadXML("Ship.txt", (Action<string>)(xmlText => this.ReadShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/GetShipByUserId?userId=" + (object)SingletonManager<UserManager>.Instance.userLogin.UserId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, xmlText, "Ship", (Action)null, false)));
        }

        public PSShip InitShip(PSShip tempPSShipObject, bool temp)
        {
            PSShip psShip = tempPSShipObject;
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                if (shipDesign.ShipDesignId == psShip.ShipDesignId)
                {
                    int index1 = 0;
                    string[] array1 = ((IEnumerable<char>)shipDesign.Mask.ToCharArray()).Select<char, string>((Func<char, string>)(c => c.ToString())).ToArray<string>();
                    // ISSUE: reference to a compiler-generated field
                    if (ShipManager.\u003C\u003Ef__mg\u0024cache0 == null)
          {
                        // ISSUE: reference to a compiler-generated field
                        ShipManager.\u003C\u003Ef__mg\u0024cache0 = new Func<string, int>(int.Parse);
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<string, int> fMgCache0 = ShipManager.\u003C\u003Ef__mg\u0024cache0;
                    int[] array2 = ((IEnumerable<string>)array1).Select<string, int>(fMgCache0).ToArray<int>();
                    psShip.rows = shipDesign.Rows;
                    psShip.columns = shipDesign.Columns;
                    psShip.mask = new int[psShip.columns, psShip.rows];
                    for (int index2 = 0; index2 < psShip.rows; ++index2)
                    {
                        for (int index3 = 0; index3 < psShip.columns; ++index3)
                        {
                            psShip.mask[index3, index2] = array2[index1];
                            ++index1;
                        }
                    }
                    break;
                }
            }
            return psShip;
        }

        public T ParseShip<T>(string xml, bool temp = false) where T : PSShip
        {
            T obj = PSObject.Deserialize<T>(xml);
            if ((object)obj != null)
                obj = this.InitShip((PSShip)obj, temp) as T;
            return obj;
        }

        public void ChooseShip(int shipDesignId, SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.ChooseShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/ChooseShip?shipDesignId=" + (object)shipDesignId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del, shipDesignId));
        }

        [DebuggerHidden]
        private IEnumerator ChooseShip(string url, SimpleManager.DownloadDelegate del, int shipDesignId)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CChooseShip\u003Ec__Iterator3() { shipDesignId = shipDesignId, url = url, del = del, \u0024this = this };
        }

        public void DownloadShipDesigns(SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            this.StartCoroutine(this.DownloadShipDesigns(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/ListAllShipDesigns2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, failAction));
        }

        [DebuggerHidden]
        private IEnumerator DownloadShipDesigns(string url, SimpleManager.DownloadDelegate del, Action failAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CDownloadShipDesigns\u003Ec__Iterator4() { url = url, del = del, failAction = failAction, \u0024this = this };
        }

        private void ParseShipDesign(string xml)
        {
            ShipDesign shipDesign = PSObject.Deserialize<ShipDesign>(xml);
            this.shipDesignDict.Add(shipDesign.ShipDesignId, shipDesign);
        }

        private void ParseShipDesigns(string xml)
        {
            foreach (ShipDesign shipDesign in ShipDesignContainer.LoadFromText(xml).shipDesigns)
                this.shipDesignDict.Add(shipDesign.ShipDesignId, shipDesign);
            Singleton<SharedManager>.Instance.SendTaskToMainThread((Action)(() => PlayerPrefs.SetFloat("ShipDesignVersion", (float)SingletonManager<SystemManager>.Instance.settings.ShipDesignVersion)), false);
        }

        public void LoadShipDesigns(SimpleManager.DownloadDelegate del)
        {
            this.shipDesignDict.Clear();
            this.LoadXML("ShipDesigns.txt", (Action<string>)(xmlText => this.ReadXML(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/ListAllShipDesigns2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del, xmlText, "ShipDesigns", new SimpleManager.ParseDelegate(this.ParseShipDesigns), (Action)null)));
        }

        public void ScanShip<T>(int inspectedUserId, Action<PSUser, T, string, PSServerMessage> endAction) where T : PSShip
        {
            this.StartCoroutine(this.ScanShip<T>(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/ScanShip2?inspectedUserId=" + (object)inspectedUserId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, endAction, inspectedUserId));
        }

        [DebuggerHidden]
        private IEnumerator ScanShip<T>(string url, Action<PSUser, T, string, PSServerMessage> endAction, int inspectedUserId) where T : PSShip
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CScanShip\u003Ec__Iterator5<T>() { inspectedUserId = inspectedUserId, url = url, endAction = endAction, \u0024this = this };
        }

        public void RushShipUpgrade(int shipId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.RushShipUpgrade(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/SpeedUpShipUpgradeUsingCredits2?shipId=" + (object)shipId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator RushShipUpgrade(string url, int shipId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CRushShipUpgrade\u003Ec__Iterator6() { shipId = shipId, url = url, endAction = endAction };
        }

        public void RushShipRepair(Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.RushShipRepair(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/SpeedUpShipHp?accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, endAction));
        }

        [DebuggerHidden]
        private IEnumerator RushShipRepair(string url, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CRushShipRepair\u003Ec__Iterator7() { url = url, endAction = endAction };
        }

        public void InspectShip(int userId, ShipManager.InspectShipDelegate del)
        {
            this.StartCoroutine(this.InspectShip(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/InspectShip2?userId=" + (object)userId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, del));
        }

        [DebuggerHidden]
        private IEnumerator InspectShip(string url, ShipManager.InspectShipDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CInspectShip\u003Ec__Iterator8() { url = url, del = del, \u0024this = this };
        }

        private PSUser ParseInspectedUser(string xml)
        {
            return PSObject.Deserialize<PSUser>(xml);
        }

        public ShipDesign GetUpgradeShipDesignFromShipDesignId(int shipDesignId)
        {
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                if (shipDesign.RequiredShipDesignId == shipDesignId)
                    return shipDesign;
            }
            return (ShipDesign)null;
        }

        public void UpdateShipPaint(int shipId, double brightnessValue, double saturationValue, double hueValue, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.UpdateShipPaint(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/CustomiseShip?shipId=" + (object)shipId + "&brightnessValue=" + (object)brightnessValue + "&saturationValue=" + (object)saturationValue + "&hueValue=" + (object)hueValue + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipId, brightnessValue, saturationValue, hueValue, endAction));
        }

        [DebuggerHidden]
        private IEnumerator UpdateShipPaint(string url, int shipId, double brightnessValue, double saturationValue, double hueValue, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CUpdateShipPaint\u003Ec__Iterator9() { shipId = shipId, brightnessValue = brightnessValue, saturationValue = saturationValue, hueValue = hueValue, url = url, endAction = endAction };
        }

        public void UpdateShipSkin(int shipId, int skinItemDesignId, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.UpdateShipSkin(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/UpdateShipSkin?shipId=" + (object)shipId + "&skinItemDesignId=" + (object)skinItemDesignId + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipId, skinItemDesignId, endAction));
        }

        [DebuggerHidden]
        private IEnumerator UpdateShipSkin(string url, int shipId, int skinItemDesignId, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CUpdateShipSkin\u003Ec__IteratorA() { shipId = shipId, skinItemDesignId = skinItemDesignId, url = url, endAction = endAction };
        }

        public void UpdateShipStickers(int shipId, string stickerString, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.UpdateShipStickers(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/UpdateShipStickers?shipId=" + (object)shipId + "&stickerString=" + stickerString + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, shipId, stickerString, endAction));
        }

        [DebuggerHidden]
        private IEnumerator UpdateShipStickers(string url, int shipId, string stickerString, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CUpdateShipStickers\u003Ec__IteratorB() { shipId = shipId, stickerString = stickerString, url = url, endAction = endAction };
        }

        public void GetCurrentResources(Action endAction = null)
        {
            this.StartCoroutine(this.GetCurrentResources(SingletonManager<Configuration>.Instance.ServerUrl + "/ShipService/GetCurrentResources?userId=" + (object)SingletonManager<UserManager>.Instance.user.Id + "&accessToken=" + SingletonManager<UserManager>.Instance.userLogin.accessToken, endAction));
        }

        [DebuggerHidden]
        private IEnumerator GetCurrentResources(string url, Action endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new ShipManager.\u003CGetCurrentResources\u003Ec__IteratorC() { url = url, endAction = endAction };
        }

        public int GetHighestShipDesignMineralCapacity()
        {
            int num = 0;
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                if (shipDesign.MineralCapacity > num)
                    num = shipDesign.MineralCapacity;
            }
            return num;
        }

        public int GetHighestShipDesignGasCapacity()
        {
            int num = 0;
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                if (shipDesign.GasCapacity > num)
                    num = shipDesign.GasCapacity;
            }
            return num;
        }

        public int GetHighestShipDesignGrids()
        {
            int num1 = 0;
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                int num2 = shipDesign.TotalType1Grids + shipDesign.TotalType2Grids;
                if (num2 > num1)
                    num1 = num2;
            }
            return num1;
        }

        public double GetHighestShipDesignHp()
        {
            double num = 0.0;
            foreach (ShipDesign shipDesign in this.shipDesignDict.Values.ToArray<ShipDesign>())
            {
                if (shipDesign.Hp > num)
                    num = shipDesign.Hp;
            }
            return num;
        }

        public delegate void DownloadShipDelegate(PSShip psShip, string url, string message, bool success, Action failAction = null);

        public delegate PSShip ParseShipDelegate(string xml);

        public delegate void ShipCreationDelegate(ShipEntity shipEntity);

        public delegate void InspectShipDelegate(PSUser psUser, PSMainShip psShip, string shipXML, string url, string message, bool success);
    }
}