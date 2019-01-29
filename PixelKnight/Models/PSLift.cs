using System;
using System.Xml.Serialization;
using PixelKnight.Enums;

namespace PixelKnight.Models
{
    [XmlRoot("Lift")]
    [Serializable]
    public class PSLift : PSObject
    {
        [XmlAttribute]
        public int RoomId { get; set; }

        [XmlAttribute]
        public int XInGrid { get; set; }

        [XmlAttribute]
        public int YInGrid { get; set; }

        [XmlAttribute]
        public float XInPixels { get; set; }

        [XmlAttribute]
        public float YInPixels { get; set; }

        [XmlIgnore]
        public LiftStatus LiftStatus { get; set; }

        [XmlAttribute("LiftStatus")]
        public string LiftStatusString
        {
            get => LiftStatus.ToString();
            set => LiftStatus = !Enum.IsDefined(typeof(LiftStatus), value) ? LiftStatus.Stand : (LiftStatus)Enum.Parse(typeof(LiftStatus), value);
        }

        [XmlAttribute]
        public double Speed { get; set; }

        [XmlAttribute]
        public int OriginRoomId { get; set; }
    }
}