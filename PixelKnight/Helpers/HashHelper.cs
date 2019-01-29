using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using Microsoft.Win32.SafeHandles;

namespace PixelKnight.Helpers
{
    public class HashHelper
    {
        public MD5 md5Hash;
        private string salt = "5343";

        public HashHelper()
        {
            this.md5Hash = MD5.Create();
        }

        public string Login(string devicekey)
        {
            return GetMd5Hash(devicekey+"DeviceTypeAndroid");
        }

        public string Action(string clientTime)
        {
            return GetMd5Hash(clientTime + salt);
        }

        public string BattleCheckSum(int battleId, int attackingShipId, int defendingShipId, string accessToken, int hp)
        {
            var secret = "12325" + "3106755" + "113278" + battleId;
            var str = $"{secret}{attackingShipId}{defendingShipId}{accessToken}{hp}{salt}";
            return GetMd5Hash(str);
        }

        private string GetMd5Hash(string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input + "savysoda"));
            StringBuilder sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}