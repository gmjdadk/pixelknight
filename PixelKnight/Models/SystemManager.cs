using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    public class SystemManager
    {
        public Setting settings = new Setting();
        public bool downloaded;
        public CrashLogClass logClass;

        protected SystemManager()
        {
        }

        private void Start()
        {
            MessageManager.NewMessageDelegate newMessageDelegate = MessageManager.newMessageDelegate;
            // ISSUE: reference to a compiler-generated field
            if (SystemManager.\u003C\u003Ef__mg\u0024cache0 == null)
      {
                // ISSUE: reference to a compiler-generated field
                SystemManager.\u003C\u003Ef__mg\u0024cache0 = new MessageManager.NewMessageDelegate(SystemManager.ServerMessageReceived);
            }
            // ISSUE: reference to a compiler-generated field
            MessageManager.NewMessageDelegate fMgCache0 = SystemManager.\u003C\u003Ef__mg\u0024cache0;
            MessageManager.newMessageDelegate = newMessageDelegate + fMgCache0;
            this.logClass = new CrashLogClass();
        }

        private void Update()
        {
        }

        private static void ServerMessageReceived(PSMessage psMessage, bool isPusher)
        {
            if (!isPusher)
                return;
            if (psMessage.ActivityType == ActivityType.MembershipChanged && isPusher)
            {
                if (SingletonManager<UserManager>.Instance.user.Id == psMessage.ToUserId)
                {
                    Debug.LogWarning((object)"Membership Changed", (Object)null);
                    SingletonManager<UserManager>.Instance.user.AllianceMembership = AllianceManager.MembershipStringToEnum(psMessage.ActivityArgument);
                }
                if (SingletonManager<UserManager>.Instance.user.AllianceMembership == AllianceMembership.None)
                {
                    Debug.LogWarning((object)"Membership Reset", (Object)null);
                    SingletonManager<AllianceManager>.Instance.ResetCurrentAlliance();
                }
            }
            if (psMessage.ActivityType == ActivityType.DeviceLogin && !psMessage.ActivityArgument.Equals(UserManager.Md5Sum(SingletonManager<UserManager>.Instance.deviceKey)))
                AlertController.ShowDisconnectPanel(SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Disconnected"), psMessage.MessageString, SingletonManager<LocalizationManager>.Instance.GetLocalizedText("Reload"));
            if (psMessage.ActivityType != ActivityType.ModelUpdate || string.IsNullOrEmpty(psMessage.ActivityArgument))
                return;
            PSMainRoom room = SingletonManager<RoomManager>.Instance.ParseRoom<PSMainRoom>(Regex.Unescape(psMessage.ActivityArgument));
            SingletonManager<ShipManager>.Instance.PlayerShip.GetRoomByRoomId(room.RoomId).ConstructionStartDate = room.ConstructionStartDate;
        }

        public void DownloadSettings(SimpleManager.DownloadDelegate del)
        {
            this.StartCoroutine(this.DownloadSettings(SingletonManager<Configuration>.Instance.ServerUrl + "/settingservice/getlatestversion2?languageKey=" + SingletonManager<LocalizationManager>.Instance.CurrentServerLanguage, del));
        }

        [DebuggerHidden]
        private IEnumerator DownloadSettings(string url, SimpleManager.DownloadDelegate del)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator)new SystemManager.\u003CDownloadSettings\u003Ec__Iterator0() { url = url, del = del, \u0024this = this };
        }

        public void ParseSettings(string xml)
        {
            this.settings = PSObject.Deserialize<Setting>(xml);
        }
    }
}