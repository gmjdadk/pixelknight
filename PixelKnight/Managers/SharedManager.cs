using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PixelKnight.Enums;
using PixelKnight.Models;
using PixelKnight.Utils;
using PixelStarships;

namespace PixelKnight.Managers
{
    public class SharedManager
    {
        public Dictionary<string, string> timeLocalizedTexts = new Dictionary<string, string>();
        public Dictionary<string, Queue<Action>> actionQueues = new Dictionary<string, Queue<Action>>();
        public const double SPEED_UP_FREE_TIME = 300.0;

        protected SharedManager()
        {
        }

        private void Awake()
        {
            this.timeLocalizedTexts.Add("y", "yearsSymbol");
            this.timeLocalizedTexts.Add("w", "weeksSymbol");
            this.timeLocalizedTexts.Add("d", "daysSymbol");
            this.timeLocalizedTexts.Add("h", "hoursSymbol");
            this.timeLocalizedTexts.Add("m", "minutesSymbol");
            this.timeLocalizedTexts.Add("s", "secondsSymbol");
        }

        public int TimeCheckSum(DateTime time)
        {
            return time.Minute * time.Second;
        }

        public int ChecksumPasswordWithString(string dataString)
        {
            return (int)dataString[0] + (int)dataString[1] + (int)dataString[3];
        }

        public static DateTime CurrentTime(bool checkBattleTime = false)
        {
            return DateTime.UtcNow;
        }

        public string ConvertNumToString(int num)
        {
            string str1 = num.ToString();
            if (num >= 1000000)
            {
                int num1 = num / 1000000;
                int num2 = (num - num1 * 1000000) / 100000;
                string localizedText = "M";
                str1 = num2 == 0 || num1 >= 100 ? num1 + localizedText : $"{num1}.{num2:#}{localizedText}";
            }
            else if (num >= 1000)
            {
                int num1 = num / 1000;
                int num2 = (num - num1 * 1000) / 100;
                string localizedText = "K";
                string str2;
                if (num2 != 0 && num1 < 100)
                {
                    str2 = $"{num1}.{num2:#}{localizedText}";
                }
                else
                {
                    str2 = num1 + localizedText;
                }
                str1 = str2;
            }
            return str1;
        }

        public string SavysodaEncryptString(string dataString)
        {
            return this.Md5Sum(dataString + "savysoda");
        }

        public string Md5Sum(string strToEncrypt)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(strToEncrypt));
            string empty = string.Empty;
            for (int index = 0; index < hash.Length; ++index)
                empty += Convert.ToString(hash[index], 16).PadLeft(2, '0');
            return empty.PadLeft(32, '0');
        }

        public string ConvertTimeToString(int time, TimeFormat format = TimeFormat.Suffix, bool showSeconds = true)
        {
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = Math.Max(0, time);
            if (num6 >= 60)
            {
                num5 = num6 / 60;
                num6 -= num5 * 60;
                if (num5 >= 60)
                {
                    num4 = num5 / 60;
                    num5 -= num4 * 60;
                    if (num4 >= 24)
                    {
                        num3 = num4 / 24;
                        num4 -= num3 * 24;
                        if (num3 >= 7)
                        {
                            num2 = num3 / 7;
                            num3 -= num2 * 7;
                            if (num2 >= 52)
                            {
                                num1 = num2 / 52;
                                num2 -= num2 * 52;
                            }
                        }
                    }
                }
            }
            if (!showSeconds)
            {
                num6 = 0;
                num5 = Math.Max(1, num5);
            }
            string str = string.Empty;
            if (num2 > 0 || num1 > 0)
            {
                switch (format)
                {
                    case TimeFormat.Suffix:
                        str = (num1 <= 0 ? string.Empty : num1 + "Y ") + (num2 <= 0 ? string.Empty : num2 + "W ") + (num3 <= 0 ? string.Empty : num3 + "D");
                        break;
                    case TimeFormat.NoSuffix:
                        str = string.Format((num1 <= 0 ? string.Empty : "{0:00}:") + (num2 <= 0 ? string.Empty : "{1:00}:") + (num2 != 0 || num3 <= 0 ? string.Empty : "{2:00}:") + "{3:00}:{4:00}:{5:00}", num1, num2, num3, num4, num5, num6);
                        break;
                }
            }
            else
            {
                switch (format)
                {
                    case TimeFormat.Suffix:
                        str = (num3 <= 0 ? string.Empty : num3 + "D ") + (num4 <= 0 ? string.Empty : num4 + "H ") + (num5 <= 0 || time >= 86400 ? string.Empty : num5 + "M ") + (num6 <= 0 || time >= 3600 ? string.Empty : num6 + "S");
                        break;
                    case TimeFormat.NoSuffix:
                        str = string.Format((num3 <= 0 ? string.Empty : "{0:00}:") + "{1:00}:{2:00}:{3:00}", num3, num4, num5, num6);
                        break;
                }
            }
            return str;
        }

        public double RoundDoubleToDigits(double f, int d)
        {
            double num = Math.Pow(10.0, (double)d);
            f *= num;
            f = f <= Math.Floor(f) + 0.49999 ? Math.Floor(f) : Math.Ceiling(f);
            return f / num;
        }

        public int ConvertDoubleTo3DigitInt(double thisDouble)
        {
            double num = thisDouble * 100.0;
            return num <= Math.Floor(num) + 0.49999 ? (int)Math.Floor(num) : (int)Math.Ceiling(num);
        }

        public int ConvertFloatTo3DigitInt(float thisFloat)
        {
            float num = thisFloat * 100f;
            return (int) (num <= Math.Floor(num) + 0.499989986419678 ? Math.Floor(num) : Math.Ceiling(num));
        }

        public int RoundDoubleToInt(double f)
        {
            f = f <= Math.Floor(f) + 0.49999 ? Math.Floor(f) : Math.Ceiling(f);
            return (int)f;
        }

        public int GetSpeedUpCostWithConstructionTime(double seconds, bool freeTime = true)
        {
            if (freeTime && seconds <= 300.0)
                return 0;
            int num = (int)Math.Floor(Math.Pow(seconds / 60.0, 0.73)) + 1;
            return num;
        }

        public static string StripUnsupportedUnicodeChars(string originalString)
        {
            return originalString;
        }

        public int ConvertDoubleDecimalsToInt(Decimal num)
        {
            string s = num.ToString().Replace(".", string.Empty).TrimStart('0');
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }

        public static PSResourceGroup[] ParseRewardStrings(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return new PSResourceGroup[0];
            string[] strArray = theString.Split('|');
            List<PSResourceGroup> psResourceGroupList = new List<PSResourceGroup>();
            for (int index = 0; index < ((IEnumerable<string>)strArray).Count<string>(); ++index)
            {
                PSResourceGroup psResourceGroup = new PSResourceGroup();
                psResourceGroup.ParseRewardString(strArray[index]);
                if (psResourceGroup.quantity > 0)
                    psResourceGroupList.Add(psResourceGroup);
            }
            return psResourceGroupList.ToArray();
        }

        public float GetCameraConvertedPosX(Camera cam, float screenPercentage, GameObject focus)
        {
            float num = Mathf.Abs((float)(((Component)cam).get_transform().get_position().z - focus.get_transform().get_position().z));
            return (float)((double)Vector3.Distance(cam.ScreenToWorldPoint(new Vector3((float)cam.get_pixelWidth() / 2f, (float)cam.get_pixelHeight() / 2f, num)), cam.ScreenToWorldPoint(new Vector3((float)cam.get_pixelWidth(), (float)cam.get_pixelHeight() / 2f, num))) * (double)screenPercentage / 100.0);
        }

        public float GetCameraConvertedPosY(Camera cam, float screenPercentage, GameObject focus)
        {
            float num = Mathf.Abs((float)(((Component)cam).get_transform().get_position().z - focus.get_transform().get_position().z));
            return (float)((double)Vector3.Distance(cam.ScreenToWorldPoint(new Vector3((float)cam.get_pixelWidth() / 2f, (float)cam.get_pixelHeight() / 2f, num)), cam.ScreenToWorldPoint(new Vector3((float)cam.get_pixelWidth() / 2f, (float)cam.get_pixelHeight(), num))) * (double)screenPercentage / 100.0);
        }

        public Vector3 ConvertWorldToCanvasPosition(Vector3 worldPos, Canvas targetCanvas, Camera targetCamera = null)
        {
            Camera camera = !Object.op_Equality((Object)targetCamera, (Object)null) ? targetCamera : targetCanvas.get_worldCamera();
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPos);
            screenPoint.z = (__Null)(double)targetCanvas.get_planeDistance();
            return camera.ScreenToWorldPoint(screenPoint);
        }

        public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z, Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(screenPosition);
            Plane plane;
            // ISSUE: explicit reference operation
            ((Plane)@plane).\u002Ector(Vector3.get_forward(), new Vector3(0.0f, 0.0f, z));
            float num;
            // ISSUE: explicit reference operation
            ((Plane)@plane).Raycast(ray, ref num);
            // ISSUE: explicit reference operation
            return ((Ray)@ray).GetPoint(num);
        }

        public SharedManager.ScaleAndPos MoveCanvasMaintainScaleandPos(GameObject obj, GameObject currentCam, Canvas currentCanvas, GameObject newCam, Canvas newCanvas)
        {
            Vector3 localScale1 = obj.get_transform().get_localScale();
            Vector3 localScale2 = ((Component)currentCanvas).get_transform().get_localScale();
            Vector3 position1 = obj.get_transform().get_position();
            Vector3 position2 = currentCam.get_transform().get_position();
            Vector3 scale;
            // ISSUE: explicit reference operation
            ((Vector3)@scale).\u002Ector((float)(localScale1.x * localScale2.x / ((Component)newCanvas).get_transform().get_localScale().x), (float)(localScale1.y * localScale2.y / ((Component)newCanvas).get_transform().get_localScale().y), 1f);
            Vector3 pos = Vector3.op_Addition(Vector3.op_Subtraction(position1, position2), newCam.get_transform().get_position());
            return new SharedManager.ScaleAndPos(scale, pos);
        }

        public void SetLayerForAllChildren(GameObject obj, int newLayer)
        {
            if (Object.op_Equality((Object)null, (Object)obj))
                return;
            obj.set_layer(newLayer);
            IEnumerator enumerator = obj.get_transform().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform)enumerator.Current;
                    if (!Object.op_Equality((Object)null, (Object)current))
                        this.SetLayerForAllChildren(((Component)current).get_gameObject(), newLayer);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = enumerator as IDisposable) != null)
                    disposable.Dispose();
            }
        }

        public static void ConvertStarbuxToSupplies(int amount, int starbuxCost, Action<PSServerMessage> endAction)
        {
            if (starbuxCost <= UserManager.Starbux)
            {
                AlertController.ShowWaitPanel(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Please wait..."));
                SingletonManager<UserManager>.Instance.BuySuppliesWithStarbux(amount, endAction, (Action)(() => AlertController.HideWaitPanel()));
            }
            else
                SharedManager.ShowInsufficientStarbuxDialogue(starbuxCost);
        }

        public static void ConvertStarbuxToResource(PSItem psItem, int amount, int starbuxCost, Action<PSServerMessage> endAction)
        {
            if (starbuxCost <= UserManager.Starbux)
            {
                AlertController.ShowWaitPanel(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Please wait..."));
                SingletonManager<UserManager>.Instance.BuyItemWithStarbux(psItem.ItemDesignId, amount, endAction, (Action)(() => AlertController.HideWaitPanel()));
            }
            else
                SharedManager.ShowInsufficientStarbuxDialogue(starbuxCost);
        }

        public static void ConvertStarbuxToResources(PSItem item1, int amount1, PSItem item2, int amount2, int starbuxCost, Action<PSServerMessage> endAction)
        {
            if (starbuxCost <= UserManager.Starbux)
            {
                AlertController.ShowWaitPanel(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Please wait..."));
                SingletonManager<UserManager>.Instance.BuyItemsWithStarbux(item1.ItemDesignId, amount1, item2.ItemDesignId, amount2, endAction, (Action)(() => AlertController.HideWaitPanel()));
            }
            else
                SharedManager.ShowInsufficientStarbuxDialogue(starbuxCost);
        }

        public static void ShowInsufficientStarbuxDialogue(int starbuxCost)
        {
            if (SingletonManager<Configuration>.Instance.ShouldHideBank)
                AlertController.ShowAlert(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Not enough Starbux"), SingletonManager<LocalizationManager>.Instance.GetLocalizedText("You don't have enough Starbux to make this purchase."), true, (Action)null, (SpriteDesign)null, (Action<bool>)null, string.Empty, (string)null, 0.0f);
            else
                AlertController.ShowOptionPanel(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Not enough Starbux"), string.Format(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("You require {0} more Starbux for this purchase. Would you like to visit the Bank to buy more Starbux?"), (object)(starbuxCost - UserManager.Starbux)), (Action)(() => GameController.CurrentUIManager.OpenMenuPanel("BankMenuPanel", (Action<GameObject>)null)), (Action)null, 0.0f, false, string.Empty);
        }

        public void GoToSceneAsync(string scene, string bottomMessage = "", string middleMessage = "", bool clearBattleData = false, AsyncOperation operation = null)
        {
            // ISSUE: variable of a compiler-generated type
            SharedManager.\u003CGoToSceneAsync\u003Ec__asyncC sceneAsyncCAsyncC;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.clearBattleData = clearBattleData;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.scene = scene;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.bottomMessage = bottomMessage;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.middleMessage = middleMessage;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.operation = operation;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            sceneAsyncCAsyncC.\u0024builder = AsyncVoidMethodBuilder.Create();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            ((AsyncVoidMethodBuilder)@sceneAsyncCAsyncC.\u0024builder).Start < SharedManager.\u003CGoToSceneAsync\u003Ec__asyncC > ((M0 &) @sceneAsyncCAsyncC);
        }

        private Task GoToSceneAsyncTask(string scene, string bottomMessage = "", string middleMessage = "", AsyncOperation operation = null)
        {
            // ISSUE: variable of a compiler-generated type
            SharedManager.\u003CGoToSceneAsyncTask\u003Ec__asyncD asyncTaskCAsyncD;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.scene = scene;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.bottomMessage = bottomMessage;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.middleMessage = middleMessage;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.operation = operation;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.\u0024this = this;
            // ISSUE: reference to a compiler-generated field
            asyncTaskCAsyncD.\u0024builder = AsyncTaskMethodBuilder.Create();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            AsyncTaskMethodBuilder & local = @asyncTaskCAsyncD.\u0024builder;
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            ((AsyncTaskMethodBuilder)local).Start < SharedManager.\u003CGoToSceneAsyncTask\u003Ec__asyncD > ((M0 &) @asyncTaskCAsyncD);
            return ((AsyncTaskMethodBuilder)local).get_Task();
        }

        private Task LoadSceneComplete(Scene originalScene)
        {
            // ISSUE: variable of a compiler-generated type
            SharedManager.\u003CLoadSceneComplete\u003Ec__asyncE sceneCompleteCAsyncE;
            // ISSUE: reference to a compiler-generated field
            sceneCompleteCAsyncE.originalScene = originalScene;
            // ISSUE: reference to a compiler-generated field
            sceneCompleteCAsyncE.\u0024builder = AsyncTaskMethodBuilder.Create();
            // ISSUE: reference to a compiler-generated field
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            AsyncTaskMethodBuilder & local = @sceneCompleteCAsyncE.\u0024builder;
            // ISSUE: explicit reference operation
            // ISSUE: cast to a reference type
            ((AsyncTaskMethodBuilder)local).Start < SharedManager.\u003CLoadSceneComplete\u003Ec__asyncE > ((M0 &) @sceneCompleteCAsyncE);
            return ((AsyncTaskMethodBuilder)local).get_Task();
        }

        public string EmailEscapeURL(string url)
        {
            return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }

        public string GetEnhancementTypeShortString(EnhancementType type)
        {
            string key = string.Empty;
            switch (type)
            {
                case EnhancementType.Hp:
                    key = "HP";
                    break;
                case EnhancementType.Attack:
                    key = "ATK";
                    break;
                case EnhancementType.Pilot:
                    key = "PLT";
                    break;
                case EnhancementType.Repair:
                    key = "RPR";
                    break;
                case EnhancementType.Weapon:
                    key = "WPN";
                    break;
                case EnhancementType.Science:
                    key = "SCI";
                    break;
                case EnhancementType.Engine:
                    key = "ENG";
                    break;
                case EnhancementType.Stamina:
                    key = "STA";
                    break;
                case EnhancementType.Ability:
                    key = "ABL";
                    break;
                case EnhancementType.FireResistance:
                    key = "RST";
                    break;
            }
            return SingletonManager<LocalizationManager>.Instance.GetLocalizedText(key);
        }

        public string GetEnhancementTypeProperString(EnhancementType type)
        {
            if (type == EnhancementType.FireResistance)
                return "Fire Resistance";
            return SingletonManager<LocalizationManager>.Instance.GetLocalizedText(type.ToString());
        }

        public void ShowItemInfoPopup(ItemDesign itemDesign)
        {
            string str = string.Empty;
            if (itemDesign.EnhancementType != EnhancementType.None)
                str = string.Format("\n<color=lime>{0} +{1:##.##}</color>", (object)Singleton<SharedManager>.Instance.GetEnhancementTypeShortString(itemDesign.EnhancementType), (object)itemDesign.EnhancementValue);
            AlertController.ShowAlert(itemDesign.ItemDesignName, itemDesign.ItemDesignDescription + str, false, (Action)null, itemDesign.ImageSprite, (Action<bool>)null, string.Empty, (string)null, 0.0f);
        }

        public struct ScaleAndPos
        {
            public Vector3 Scale;
            public Vector3 Position;

            public ScaleAndPos(Vector3 scale, Vector3 pos)
            {
                this.Scale = scale;
                this.Position = pos;
            }
        }
    }
}
