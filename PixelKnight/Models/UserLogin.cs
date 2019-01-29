using System;
using System.Xml.Serialization;
using PixelStarships;

namespace PixelKnight.Models
{
    [Serializable]
    public class UserLogin : PSObject
    {
        [XmlAttribute]
        public string accessToken { get; set; }

        [XmlAttribute]
        public int UserId { get; set; }
    }
}