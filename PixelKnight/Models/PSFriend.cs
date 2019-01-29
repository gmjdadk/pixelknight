using System;
using System.Xml.Serialization;
using PixelKnight.Enums;
using PixelKnight.Managers;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("Friend")]
    [Serializable]
    public class PSFriend : PSObject
    {
        [NonSerialized]
        private string _name;

        [XmlAttribute]
        public int Id { get; set; }

        [XmlAttribute]
        public int FriendUserId { get; set; }

        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

        [XmlAttribute]
        public FriendType FriendType { get; set; }

        [XmlAttribute]
        public int FriendTrophy { get; set; }

        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => _name = SharedManager.StripUnsupportedUnicodeChars(value);
        }

        public DateTime lastMessageReadDate { get; set; }
    }
}