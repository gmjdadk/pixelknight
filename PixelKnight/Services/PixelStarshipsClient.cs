using System;
using System.Net;
using PixelKnight.Helpers;

namespace PixelKnight.Services
{
    public class PixelStarshipsClient
    {
        private readonly string baseUrl;
        private HashHelper hashHelper;

        public PixelStarshipsClient()
        {
            this.baseUrl = "https://api.pixelstarships.com";
            this.hashHelper = new HashHelper();
        }

        public string Login(string deviceKey)
        {
            var param = $"devicekey={deviceKey}&checksum={hashHelper.Login(deviceKey)}&advertisingKey=&isJailBroken=False&deviceType=DeviceTypeAndroid&signal=False&languageKey=en";
            var url = $"{baseUrl}/UserService/DeviceLogin8?{param}";
            var resp = Post(url, param);
            return resp;
        }

        private string Post(string url, string param)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                return client.UploadString(url, param);
            }
        }
    }
}