using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using PixelKnight.Enums;
using PixelKnight.Models;
using PixelStarships;

namespace PixelKnight.Managers
{
    public class UserManager
    {
        public static readonly int HEARTBEAT_INTERVAL = 60;
        public PSUser user = new PSUser();
        public UserLogin userLogin = new UserLogin();
        public List<PSUser> topUsers = new List<PSUser>();
        public List<PSUser> userSearchResults = new List<PSUser>();
        public List<PSFriend> userFriends = new List<PSFriend>();
        public string deviceType = "DeviceTypeMac";
        public static DateTime lastInputTime;
        public static DateTime lastMinimizedTime;
        public static DateTime lastHeartbeatTime;
        public static DateTime lastRegisteredTime;
        public static bool stationLogin;
        public static PSUser OriginalPsUser;
        public static PSQuiz currentQuiz;
        //public static BlockedFriendsUpdatedDelegate blockedFriendsUpdatedDelegate;
        public string checkSum;
        public string deviceKey;
        public string oldDeviceKey;
        public SDKLogin chinaPaymentLoginInfo;
        public int totalUsers;
        public static bool topUsersUpdated;
        public static bool iapValidated;
        private bool lostFocus;
        private bool heartbeating;

        protected UserManager()
        {
        }

        public List<PSFriend> BlockedFriends
        {
            get
            {
                return this.userFriends.FindAll((Predicate<PSFriend>)(obj => obj.FriendType == FriendType.FriendTypeIgnore));
            }
        }

        public bool IsUserBlocked(int friendUserId)
        {
            return this.BlockedFriends.Find((Predicate<PSFriend>)(obj => obj.FriendUserId == friendUserId)) != null;
        }

        public bool UserHasStatus(UserStatus status)
        {
            return ((UserStatus)SingletonManager<UserManager>.Instance.user.UserStatus & status) > (UserStatus)0;
        }

        public static int Starbux
        {
            get
            {
                if (UserManager.stationLogin)
                    return SingletonManager<AllianceManager>.Instance.PlayerPsAlliance.Credits;
                return SingletonManager<UserManager>.Instance.user.Credits;
            }
            set
            {
                if (UserManager.stationLogin)
                    SingletonManager<AllianceManager>.Instance.PlayerPsAlliance.Credits = value;
                else
                    SingletonManager<UserManager>.Instance.user.Credits = value;
            }
        }

        public static int Supplies
        {
            get
            {
                if (UserManager.stationLogin)
                    return SingletonManager<UserManager>.Instance.user.Credits;
                return 0;
            }
            set
            {
                if (!UserManager.stationLogin)
                    return;
                SingletonManager<UserManager>.Instance.user.Credits = value;
            }
        }

        public string refreshToken
        {
            get
            {
                return PlayerPrefs.GetString(nameof(refreshToken), string.Empty);
            }
            set
            {
                PlayerPrefs.SetString(nameof(refreshToken), value);
            }
        }

        private void Awake()
        {
            this.deviceType = "DeviceTypeAndroid";
            this.deviceKey = PlayerPrefs.GetString("userKey", string.Empty);
            if (string.IsNullOrEmpty(this.deviceKey))
            {
                AndroidJavaObject androidJavaObject = (AndroidJavaObject)((AndroidJavaObject)((AndroidJavaObject)new AndroidJavaClass("com.unity3d.player.UnityPlayer")).GetStatic<AndroidJavaObject>("currentActivity")).Call<AndroidJavaObject>("getContentResolver", (object[])Array.Empty<object>());
                AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.Settings$Secure");
                string str = (string)((AndroidJavaObject)androidJavaClass).GetStatic<string>("ANDROID_ID");
                this.deviceKey = (string)((AndroidJavaObject)androidJavaClass).CallStatic<string>("getString", new object[2]
                {
          (object) androidJavaObject,
          (object) str
                });
            }
            PlayerPrefs.SetString("userKey", this.deviceKey);
            Debug.LogWarning((object)("ANDROID_ID: " + this.deviceKey), (Object)null);
            if (string.IsNullOrEmpty(this.deviceKey) || this.deviceKey.Length < 16 && this.deviceKey.All<char>((Func<char, bool>)(ch => (int)ch == (int)this.deviceKey[0])))
            {
                this.deviceKey = PlayerPrefs.GetString("GUID", new UniqueId().ToString());
                Debug.LogWarning((object)("UniqueId: " + this.deviceKey), (Object)null);
                PlayerPrefs.SetString("GUID", this.deviceKey);
            }
            PlayerPrefs.Save();
            this.checkSum = UserManager.Md5Sum(this.deviceKey + this.deviceType + "savysoda");
        }

        public static string Md5Sum(string strToEncrypt)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(strToEncrypt));
            string empty = string.Empty;
            for (int index = 0; index < hash.Length; ++index)
                empty += Convert.ToString(hash[index], 16).PadLeft(2, '0');
            return empty.PadLeft(32, '0');
        }

        public int CheckSumPasswordWithString(string thisString)
        {
            return (int)thisString[0] + (int)thisString[1] + (int)thisString[3];
        }

        public int TimeCheckSumForDate(DateTime date)
        {
            return date.Second * date.Minute;
        }

        public bool IsCurrentUserFriendsWithUser(int friendUserId)
        {
            foreach (PSFriend userFriend in SingletonManager<UserManager>.Instance.userFriends)
            {
                if (userFriend.FriendUserId == friendUserId && userFriend.FriendType != FriendType.FriendTypeIgnore)
                    return true;
            }
            return false;
        }

        public string GetActivatedPromotions()
        {
            return this.user.ActivatedPromotions;
        }

        public void DownloadUserLogin(SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.DownloadUserLogin(string.Format("{0}/UserService/DeviceLogin8?deviceKey={1}&advertisingKey=&isJailBroken={2}&checksum={3}&deviceType={4}&signal={5}&languageKey={6}{7}", (object)SingletonManager<Configuration>.Instance.SecureServerUrl, (object)this.deviceKey, (object)(MediaTypeNames.Application.get_sandboxType() == 3), (object)this.checkSum, (object)this.deviceType, (object)UserManager.stationLogin, (object)SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, !string.IsNullOrEmpty(this.refreshToken) ? (object)("&refreshToken=" + this.refreshToken) : (object)string.Empty), del));
        }

        [DebuggerHidden]
        private IEnumerator DownloadUserLogin(string url, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CDownloadUserLogin\u003Ec__Iterator0() { url = url, del = del, \u0024this = this };
        }

        public PSUser ReadUser(string url, string text, string name)
        {
            PSUser psUser = (PSUser)null;
            bool flag = false;
            try
            {
                XmlTextReader xmlTextReader = new XmlTextReader((TextReader)new StringReader(text));
                while (!xmlTextReader.EOF)
                {
                    if (xmlTextReader.Name == name)
                        psUser = this.ParseUser(xmlTextReader.ReadOuterXml());
                    else
                        xmlTextReader.Read();
                }
            }
            catch
            {
                flag = true;
            }
            return psUser;
        }

        private void Update()
        {
            if (Input.get_touchCount() <= 0 && !Input.get_anyKey())
                return;
            UserManager.lastInputTime = DateTime.Now;
        }

        public void StartHeartbeatCheck()
        {
            if (SingletonManager<Configuration>.Instance.TestMode && !(SingletonManager<Configuration>.Instance.ServerName == "Prod"))
                return;
            Debug.LogWarning((object)nameof(StartHeartbeatCheck), (Object)null);
            this.StopHeartbeatCheck();
            try
            {
                this.StartCoroutine(this.HeartbeatCheckTimer());
            }
            catch
            {
                AlertController.ShowDisconnectPanel("TRY LOG IN AGAIN", string.Empty, "RECONNECT");
            }
        }

        public void StopHeartbeatCheck()
        {
            this.heartbeating = false;
            UserManager.lastInputTime = DateTime.Now;
            UserManager.lastRegisteredTime = DateTime.Now;
        }

        public bool ShouldForceReload()
        {
            return (DateTime.Now - UserManager.lastRegisteredTime).TotalMinutes >= 3.0;
        }

        [DebuggerHidden]
        public IEnumerator HeartbeatCheckTimer()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CHeartbeatCheckTimer\u003Ec__Iterator1() { \u0024this = this };
        }

        public void CheckHeartbeat(DateTime clientDateTime, Action<bool, string, PSQuiz> endAction)
        {
            UserManager.lastRegisteredTime = DateTime.Now;
            this.StartCoroutine(this.CheckHeartbeatCoroutine(clientDateTime, endAction));
        }

        [DebuggerHidden]
        public IEnumerator CheckHeartbeatCoroutine(DateTime clientDateTime, Action<bool, string, PSQuiz> endAction)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CCheckHeartbeatCoroutine\u003Ec__Iterator2() { clientDateTime = clientDateTime, endAction = endAction, \u0024this = this };
        }

        public void ValidateUser(string name, Action<PSServerMessage, string> endAction = null)
        {
            this.StartCoroutine(this.ValidateUserCoroutine(name, this.userLogin.accessToken, endAction));
        }

        [DebuggerHidden]
        public IEnumerator ValidateUserCoroutine(string name, string accessToken, Action<PSServerMessage, string> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CValidateUserCoroutine\u003Ec__Iterator3() { name = name, accessToken = accessToken, endAction = endAction, \u0024this = this };
        }

        public void SetGenderType(SimpleManager.DownloadDelegate del, int gender, string accessToken)
        {
            this.StartCoroutine(this.SetGenderTypeCoroutine(del, gender, accessToken));
        }

        [DebuggerHidden]
        public IEnumerator SetGenderTypeCoroutine(SimpleManager.DownloadDelegate del, int gender, string accessToken)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSetGenderTypeCoroutine\u003Ec__Iterator4() { gender = gender, accessToken = accessToken, del = del, \u0024this = this };
        }

        public void SetRaceType(SimpleManager.DownloadDelegate del, int race, string accessToken)
        {
            this.StartCoroutine(this.SetRaceTypeCoroutine(del, race, accessToken));
        }

        [DebuggerHidden]
        public IEnumerator SetRaceTypeCoroutine(SimpleManager.DownloadDelegate del, int race, string accessToken)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSetRaceTypeCoroutine\u003Ec__Iterator5() { race = race, accessToken = accessToken, del = del, \u0024this = this };
        }

        public void UpdateTutorialStep(TutorialStep step)
        {
            if (step == TutorialStep.TapOption)
                step = TutorialStep.EndPhase;
            this.StartCoroutine(this.UpdateTutorialStep((int)step));
        }

        [DebuggerHidden]
        public IEnumerator UpdateTutorialStep(int step)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUpdateTutorialStep\u003Ec__Iterator6() { step = step, \u0024this = this };
        }

        public void UpdateTutorialTipStatus(int tip)
        {
            this.StartCoroutine(this.UpdateTutorialTipStatusCoroutine(tip));
        }

        [DebuggerHidden]
        private IEnumerator UpdateTutorialTipStatusCoroutine(int tip)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUpdateTutorialTipStatusCoroutine\u003Ec__Iterator7() { tip = tip, \u0024this = this };
        }

        public void AddStarBux(int quantity)
        {
            this.StartCoroutine(this.AddStarBuxCoroutine(quantity));
        }

        [DebuggerHidden]
        public IEnumerator AddStarBuxCoroutine(int quantity)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CAddStarBuxCoroutine\u003Ec__Iterator8() { quantity = quantity, \u0024this = this };
        }

        public void SetUserStatusState(UserStatus status, bool active, Action<PSServerMessage> successAction = null)
        {
            this.StartCoroutine(this.SetUserStatusStateCoroutine(status, active, successAction));
        }

        [DebuggerHidden]
        private IEnumerator SetUserStatusStateCoroutine(UserStatus status, bool active, Action<PSServerMessage> successAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSetUserStatusStateCoroutine\u003Ec__Iterator9() { active = active, status = status, successAction = successAction, \u0024this = this };
        }

        public void BuySuppliesWithStarbux(int itemQuantity, Action<PSServerMessage> successAction, Action endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.BuySuppliesWithStarbuxCoroutine(itemQuantity, successAction, endAction));
        }

        [DebuggerHidden]
        public IEnumerator BuySuppliesWithStarbuxCoroutine(int itemQuantity, Action<PSServerMessage> successAction, Action endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CBuySuppliesWithStarbuxCoroutine\u003Ec__IteratorA() { itemQuantity = itemQuantity, successAction = successAction, endAction = endAction, \u0024this = this };
        }

        public void BuyItemWithStarbux(int itemDesignId, int itemQuantity, Action<PSServerMessage> successAction, Action endAction = null)
        {
            AlertController.ShowServerLoadIcon();
            this.StartCoroutine(this.BuyItemWithStarbuxCoroutine(itemDesignId, itemQuantity, successAction, endAction));
        }

        [DebuggerHidden]
        public IEnumerator BuyItemWithStarbuxCoroutine(int itemDesignId, int itemQuantity, Action<PSServerMessage> successAction, Action endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CBuyItemWithStarbuxCoroutine\u003Ec__IteratorB() { itemDesignId = itemDesignId, itemQuantity = itemQuantity, successAction = successAction, endAction = endAction, \u0024this = this };
        }

        public void BuyItemsWithStarbux(int itemDesignId1, int itemQuantity1, int itemDesignId2, int itemQuantity2, Action<PSServerMessage> successAction, Action endAction = null)
        {
            this.StartCoroutine(this.BuyItemsWithStarbuxCoroutine(itemDesignId1, itemQuantity1, itemDesignId2, itemQuantity2, successAction, endAction));
        }

        [DebuggerHidden]
        public IEnumerator BuyItemsWithStarbuxCoroutine(int itemDesignId1, int itemQuantity1, int itemDesignId2, int itemQuantity2, Action<PSServerMessage> successAction, Action endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CBuyItemsWithStarbuxCoroutine\u003Ec__IteratorC() { itemDesignId1 = itemDesignId1, itemQuantity1 = itemQuantity1, itemDesignId2 = itemDesignId2, itemQuantity2 = itemQuantity2, successAction = successAction, endAction = endAction, \u0024this = this };
        }

        public void GenerateNewDeviceKeyFromServer(Action<bool> endAction = null)
        {
            this.StartCoroutine(this.GenerateNewDeviceKeyFromServer(string.Format("{0}/UserService/GenerateKey?deviceKey={1}", (object)SingletonManager<Configuration>.Instance.SecureServerUrl, (object)this.deviceKey), endAction));
        }

        [DebuggerHidden]
        private IEnumerator GenerateNewDeviceKeyFromServer(string url, Action<bool> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CGenerateNewDeviceKeyFromServer\u003Ec__IteratorD() { url = url, endAction = endAction, \u0024this = this };
        }

        public void SetAuthenticationTypeOnServer(AuthenticationType authType, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.SetAuthenticationTypeOnServer(string.Format("{0}/UserService/SetAuthenticationType?authenticationType={1}&accessToken={2}&refreshToken={3}", (object)SingletonManager<Configuration>.Instance.SecureServerUrl, (object)authType.ToString(), (object)this.userLogin.accessToken, (object)this.refreshToken), authType, endAction));
        }

        [DebuggerHidden]
        private IEnumerator SetAuthenticationTypeOnServer(string url, AuthenticationType authType, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSetAuthenticationTypeOnServer\u003Ec__IteratorE() { authType = authType, url = url, endAction = endAction, \u0024this = this };
        }

        public void NewAccount(Action endAction = null)
        {
            this.StartCoroutine(this.NewAccount(SingletonManager<Configuration>.Instance.SecureServerUrl + "/UserService/NewAccount?deviceKey=" + this.deviceKey + "&accessToken=" + this.userLogin.accessToken, endAction));
        }

        [DebuggerHidden]
        private IEnumerator NewAccount(string url, Action endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CNewAccount\u003Ec__IteratorF() { url = url, endAction = endAction, \u0024this = this };
        }

        public void LoginExistingAccount(string email, string password, SimpleManager.DownloadDelegate del)
        {
            string clientDateTime = Singleton<SharedManager>.Instance.CurrentTime(false).ToString("s");
            string checksum = Singleton<SharedManager>.Instance.SavysodaEncryptString(this.deviceKey + email + clientDateTime + this.userLogin.accessToken + SingletonManager<Configuration>.Instance.ChecksumKey);
            this.StartCoroutine(this.LoginExistingAccount(string.Format("{0}/UserService/UserEmailPasswordAuthorize2?clientDateTime={1}&checksum={2}&deviceKey={3}&email={4}&password={5}&accessToken={6}", (object)SingletonManager<Configuration>.Instance.SecureServerUrl, (object)clientDateTime, (object)checksum, (object)this.deviceKey, (object)UnityWebRequest.EscapeURL(email), (object)UnityWebRequest.EscapeURL(password), (object)this.userLogin.accessToken), clientDateTime, checksum, email, password, del));
        }

        [DebuggerHidden]
        private IEnumerator LoginExistingAccount(string url, string clientDateTime, string checksum, string email, string password, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CLoginExistingAccount\u003Ec__Iterator10() { clientDateTime = clientDateTime, checksum = checksum, email = email, password = password, url = url, del = del, \u0024this = this };
        }

        public void UserKeyPasswordAuthorize(string deviceKey, string password, SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.UserKeyPasswordAuthorize(string.Format("{0}/UserService/UserKeyPasswordAuthorize?deviceKey={1}&password={2}", (object)SingletonManager<Configuration>.Instance.SecureServerUrl, (object)deviceKey, (object)UnityWebRequest.EscapeURL(password)), deviceKey, password, del));
        }

        [DebuggerHidden]
        private IEnumerator UserKeyPasswordAuthorize(string url, string deviceKey, string password, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUserKeyPasswordAuthorize\u003Ec__Iterator11() { deviceKey = deviceKey, password = password, url = url, del = del, \u0024this = this };
        }

        public void RegisterAccount(string email, string password, SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.RegisterAccount(SingletonManager<Configuration>.Instance.SecureServerUrl + "/UserService/RegisterUser?email=" + UnityWebRequest.EscapeURL(email) + "&password=" + UnityWebRequest.EscapeURL(password) + "&accessToken=" + this.userLogin.accessToken, email, password, del));
        }

        [DebuggerHidden]
        private IEnumerator RegisterAccount(string url, string email, string password, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CRegisterAccount\u003Ec__Iterator12() { email = email, password = password, url = url, del = del, \u0024this = this };
        }

        public void GetPlayerRankings(SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.GetPlayerRankings(SingletonManager<Configuration>.Instance.ServerUrl + "/LadderService/FindUserRanking?accessToken=" + this.userLogin.accessToken, del));
        }

        [DebuggerHidden]
        private IEnumerator GetPlayerRankings(string url, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CGetPlayerRankings\u003Ec__Iterator13() { url = url, del = del, \u0024this = this };
        }

        public void DownloadPlayerLeaderboards(int highest = 0, int lowest = 100)
        {
            this.StartCoroutine(this.DownloadPlayerLeaderboards(SingletonManager<Configuration>.Instance.ServerUrl + "/LadderService/ListUsersByRanking?from=" + (object)highest + "&to" + (object)lowest + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator DownloadPlayerLeaderboards(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CDownloadPlayerLeaderboards\u003Ec__Iterator14() { url = url, \u0024this = this };
        }

        public void ChangeLanguage(string languageKey)
        {
            this.StartCoroutine(this.ChangeLanguage(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/UpdateUserLanguageKey?languageKey=" + languageKey + "&accessToken=" + this.userLogin.accessToken, languageKey));
        }

        [DebuggerHidden]
        private IEnumerator ChangeLanguage(string url, string languageKey)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CChangeLanguage\u003Ec__Iterator15() { languageKey = languageKey, url = url, \u0024this = this };
        }

        public void PurchaseStarbux(int starbuxQuantity, string productKey, bool isProduction, string receipt, string transactionKey, string deviceType, Action<PSServerMessage, int> endAction = null)
        {
            this.StartCoroutine(this.PurchaseStarbuxCoroutine(starbuxQuantity, productKey, isProduction, receipt, Uri.EscapeUriString(transactionKey), deviceType, endAction));
        }

        [DebuggerHidden]
        public IEnumerator PurchaseStarbuxCoroutine(int starbux, string productKey, bool isProduction, string receipt, string transactionKey, string deviceType, Action<PSServerMessage, int> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CPurchaseStarbuxCoroutine\u003Ec__Iterator16() { productKey = productKey, receipt = receipt, starbux = starbux, isProduction = isProduction, transactionKey = transactionKey, deviceType = deviceType, endAction = endAction, \u0024this = this };
        }

        public void RegisterChinaPaymentLogin(SDKLogin loginInfo, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.RegisterChinaPaymentLoginCoroutine(loginInfo, endAction));
        }

        [DebuggerHidden]
        public IEnumerator RegisterChinaPaymentLoginCoroutine(SDKLogin loginInfo, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CRegisterChinaPaymentLoginCoroutine\u003Ec__Iterator17() { loginInfo = loginInfo, endAction = endAction, \u0024this = this };
        }

        public void ChinaPaymentAuthorize(Action<PSServerMessage, int> endAction = null)
        {
            this.StartCoroutine(this.ChinaPaymentAuthorizeCoroutine(endAction));
        }

        [DebuggerHidden]
        public IEnumerator ChinaPaymentAuthorizeCoroutine(Action<PSServerMessage, int> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CChinaPaymentAuthorizeCoroutine\u003Ec__Iterator18() { endAction = endAction, \u0024this = this };
        }

        public void CollectDailyReward(DailyRewardStatus dailyRewardStatus, int argument, Action<PSServerMessage> endAction = null)
        {
            this.StartCoroutine(this.CollectDailyReward(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/CollectDailyReward2?dailyRewardStatus=" + (object)dailyRewardStatus + "&argument=" + (object)argument + "&accessToken=" + this.userLogin.accessToken, dailyRewardStatus, argument, endAction));
        }

        [DebuggerHidden]
        private IEnumerator CollectDailyReward(string url, DailyRewardStatus dailyRewardStatus, int argument, Action<PSServerMessage> endAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CCollectDailyReward\u003Ec__Iterator19() { dailyRewardStatus = dailyRewardStatus, argument = argument, url = url, endAction = endAction, \u0024this = this };
        }

        public void PurchaseCatalog(int argument, Action<PSServerMessage> successAction = null)
        {
            DateTime time = Singleton<SharedManager>.Instance.CurrentTime(false);
            int checksum = Singleton<SharedManager>.Instance.ChecksumPasswordWithString(this.userLogin.accessToken) + Singleton<SharedManager>.Instance.TimeCheckSum(time);
            string clientDateTime = time.ToString("s");
            this.StartCoroutine(this.PurchaseCatalog(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/PurchaseCatalog2?argument=" + (object)argument + "&clientDateTime=" + clientDateTime + "&checksum=" + (object)checksum + "&accessToken=" + this.userLogin.accessToken, argument, clientDateTime, checksum, successAction));
        }

        [DebuggerHidden]
        private IEnumerator PurchaseCatalog(string url, int argument, string clientDateTime, int checksum, Action<PSServerMessage> successAction = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CPurchaseCatalog\u003Ec__Iterator1A() { argument = argument, clientDateTime = clientDateTime, checksum = checksum, url = url, successAction = successAction, \u0024this = this };
        }

        public void FacebookAuthorize(DateTime FacebookExpiry, string FacebookToken)
        {
            this.StartCoroutine(this.FacebookAuthorizeCoroutine(SingletonManager<Configuration>.Instance.SecureServerUrl + "/UserService/UserFacebookAuthorize?facebookToken=" + FacebookToken + "&expires=" + FacebookExpiry.ToString("s") + "&deviceKey=" + this.deviceKey + "&accessToken=" + this.userLogin.accessToken, FacebookExpiry, FacebookToken));
        }

        [DebuggerHidden]
        private IEnumerator FacebookAuthorizeCoroutine(string url, DateTime FacebookExpiry, string FacebookToken)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CFacebookAuthorizeCoroutine\u003Ec__Iterator1B() { FacebookToken = FacebookToken, FacebookExpiry = FacebookExpiry, url = url, \u0024this = this };
        }

        public void SearchUsers(string searchQuery, SimpleManager.DownloadDelegate del = null)
        {
            this.StartCoroutine(this.SearchUsersCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/SearchUsers?searchString=" + Uri.EscapeUriString(searchQuery), del));
        }

        [DebuggerHidden]
        private IEnumerator SearchUsersCoroutine(string url, SimpleManager.DownloadDelegate del = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSearchUsersCoroutine\u003Ec__Iterator1C() { url = url, del = del, \u0024this = this };
        }

        public void GetFriends(SimpleManager.DownloadDelegate del = null)
        {
            this.StartCoroutine(this.GetFriendsCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/ListFriends?UserId=" + (object)this.user.Id + "&accessToken=" + this.userLogin.accessToken, del));
        }

        [DebuggerHidden]
        private IEnumerator GetFriendsCoroutine(string url, SimpleManager.DownloadDelegate del = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CGetFriendsCoroutine\u003Ec__Iterator1D() { url = url, del = del, \u0024this = this };
        }

        public void AddFriend(int friendUserId)
        {
            this.StartCoroutine(this.AddFriendCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/AddFriend2?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator AddFriendCoroutine(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CAddFriendCoroutine\u003Ec__Iterator1E() { url = url, \u0024this = this };
        }

        public void AcceptFriend(int friendUserId)
        {
            this.StartCoroutine(this.AcceptFriendCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/AcceptFriendRequest?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator AcceptFriendCoroutine(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CAcceptFriendCoroutine\u003Ec__Iterator1F() { url = url };
        }

        public void DeclineFriend(int friendUserId)
        {
            this.StartCoroutine(this.DeclineFriendCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/DeclineFriendRequest?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator DeclineFriendCoroutine(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CDeclineFriendCoroutine\u003Ec__Iterator20() { url = url };
        }

        public void RemoveFriend(int friendUserId)
        {
            this.StartCoroutine(this.RemoveFriendCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/RemoveFriend?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken, friendUserId));
        }

        [DebuggerHidden]
        private IEnumerator RemoveFriendCoroutine(string url, int friendUserId)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CRemoveFriendCoroutine\u003Ec__Iterator21() { url = url, friendUserId = friendUserId };
        }

        public void BlockUser(int friendUserId, string blockedUserName)
        {
            this.ReportUser(friendUserId);
            PSFriend psFriend = this.userFriends.Find((Predicate<PSFriend>)(obj => obj.FriendUserId == friendUserId));
            if (psFriend != null)
                psFriend.FriendType = FriendType.FriendTypeIgnore;
            else
                this.userFriends.Add(new PSFriend()
                {
                    FriendUserId = friendUserId,
                    FriendType = FriendType.FriendTypeIgnore,
                    FriendTrophy = 0,
                    Id = -1,
                    Name = blockedUserName
                });
            if (UserManager.blockedFriendsUpdatedDelegate == null)
                return;
            UserManager.blockedFriendsUpdatedDelegate();
        }

        public void UnBlockUser(int friendUserId)
        {
            this.UnIgnoreUser(friendUserId);
            PSFriend psFriend = this.userFriends.Find((Predicate<PSFriend>)(obj => obj.FriendUserId == friendUserId));
            if (psFriend != null)
                this.userFriends.Remove(psFriend);
            if (UserManager.blockedFriendsUpdatedDelegate == null)
                return;
            UserManager.blockedFriendsUpdatedDelegate();
        }

        public void ReportUser(int friendUserId)
        {
            this.StartCoroutine(this.ReportUserCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/ReportUser?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator ReportUserCoroutine(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CReportUserCoroutine\u003Ec__Iterator22() { url = url, \u0024this = this };
        }

        public void UnIgnoreUser(int friendUserId)
        {
            this.StartCoroutine(this.UnIgnoreUserCoroutine(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/UnignoreUser?friendUserId=" + (object)friendUserId + "&accessToken=" + this.userLogin.accessToken));
        }

        [DebuggerHidden]
        private IEnumerator UnIgnoreUserCoroutine(string url)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUnIgnoreUserCoroutine\u003Ec__Iterator23() { url = url };
        }

        public void UpdateUserLanguageKey(string languageKey)
        {
            this.StartCoroutine(this.UpdateUserLanguageKey(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/UpdateUserLanguageKey?languageKey=" + languageKey + "&accessToken=" + this.userLogin.accessToken, languageKey));
        }

        [DebuggerHidden]
        private IEnumerator UpdateUserLanguageKey(string url, string languageKey)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUpdateUserLanguageKey\u003Ec__Iterator24() { languageKey = languageKey, url = url };
        }

        public void UseAchievementIcon(int achievementId)
        {
            this.StartCoroutine(this.UseAchievementIcon(SingletonManager<Configuration>.Instance.ServerUrl + "/UserService/UseIconForAchievement?achievementId=" + (object)achievementId + "&accessToken=" + this.userLogin.accessToken, achievementId));
        }

        [DebuggerHidden]
        private IEnumerator UseAchievementIcon(string url, int achievementId)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CUseAchievementIcon\u003Ec__Iterator25() { achievementId = achievementId, url = url };
        }

        public void SetAdIntention(RewardVideoIntention intention, int argument, Action endAction)
        {
            this.StartCoroutine(this.SetAdIntentionCoroutine(intention, argument, endAction));
        }

        [DebuggerHidden]
        private IEnumerator SetAdIntentionCoroutine(RewardVideoIntention intention, int argument, Action endAction)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSetAdIntentionCoroutine\u003Ec__Iterator26() { intention = intention, argument = argument, endAction = endAction };
        }

        public void GetCurrentUser(SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.GetCurrentUserCoroutine(del));
        }

        [DebuggerHidden]
        private IEnumerator GetCurrentUserCoroutine(SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CGetCurrentUserCoroutine\u003Ec__Iterator27() { del = del, \u0024this = this };
        }

        public void SubmitQuiz(int quizId, string answer, Action<PSServerMessage, ErrorCode> endAction)
        {
            this.StartCoroutine(this.SubmitQuizCoroutine(quizId, answer, endAction));
        }

        [DebuggerHidden]
        private IEnumerator SubmitQuizCoroutine(int quizId, string answer, Action<PSServerMessage, ErrorCode> endAction)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new UserManager.\u003CSubmitQuizCoroutine\u003Ec__Iterator28() { quizId = quizId, answer = answer, endAction = endAction, \u0024this = this };
        }

        public PSUser ParseUser(string xml)
        {
            return PSObject.Deserialize<PSUser>(xml);
        }

        public void ParseUsers(string xml)
        {
            foreach (PSUser user in UserContainer.LoadFromText(xml).users)
                this.topUsers.Add(user);
        }

        public void ParseUserSearch(string xml)
        {
            foreach (PSUser user in UserContainer.LoadFromText(xml).users)
                this.userSearchResults.Add(user);
        }

        private void ParseUserLogin(string xml)
        {
            this.userLogin = PSObject.Deserialize<UserLogin>(xml);
        }

        public PSUserRanking ParseUserRanking(string xml)
        {
            return PSObject.Deserialize<PSUserRanking>(xml);
        }

        private void ParseXMLFriend(string data)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated method
            Singleton<SharedManager>.Instance.SendTaskToMainThread(new Action(new UserManager.\u003CParseXMLFriend\u003Ec__AnonStorey59()
            {
        \u0024this = this,
                psFriend = PSObject.Deserialize<PSFriend>(data)
            }.\u003C\u003Em__0), false);
        }

        private void ParseXMLFriends(string data)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated method
            Singleton<SharedManager>.Instance.SendTaskToMainThread(new Action(new UserManager.\u003CParseXMLFriends\u003Ec__AnonStorey5A()
            {
                friendContainer = FriendContainer.LoadFromText(data)
            }.\u003C\u003Em__0), false);
        }

        public PSQuiz ParseQuiz(string xml)
        {
            return PSObject.Deserialize<PSQuiz>(xml);
        }

        public delegate void BlockedFriendsUpdatedDelegate();
    }
}
