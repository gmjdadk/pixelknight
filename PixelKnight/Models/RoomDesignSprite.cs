using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using PixelKnight.Enums;

namespace PixelKnight.Models
{
    public class RoomDesignSprite : PSObject
    {
        public const string ALL_ROOMDESIGNSPRITES_CACHE = "allRoomDesignSprites";

        [XmlAttribute]
        public int RoomDesignSpriteId { get; set; }

        [XmlAttribute]
        public int RoomDesignId { get; set; }

        [XmlAttribute]
        public int SpriteId { get; set; }

        [XmlAttribute]
        public int RaceId { get; set; }

        [XmlIgnore]
        public RoomSpriteType RoomSpriteType { get; set; }

        [XmlAttribute("RoomSpriteType")]
        public string RoomSpriteTypeString
        {
            get => RoomSpriteType.ToString();
            set => RoomSpriteType = !Enum.IsDefined(typeof(RoomSpriteType), value.Replace(" ", string.Empty)) ? RoomSpriteType.Interior : (RoomSpriteType)Enum.Parse(typeof(RoomSpriteType), value.Replace(" ", string.Empty));
        }

        [XmlAttribute]
        public int AnimationId { get; set; }
    }
}