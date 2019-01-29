using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using PixelKnight.Managers;
using PixelKnight.Utils;
using PixelStarships;

namespace PixelKnight.Models
{
    [XmlRoot("Quiz")]
    [Serializable]
    public class PSQuiz : PSObject
    {
        [NonSerialized]
        private List<PSResourceGroup> _options = new List<PSResourceGroup>();
        [NonSerialized]
        private DateTime _quizDate;

        [XmlAttribute]
        public int QuizId { get; set; }

        [XmlIgnore]
        public QuestionType QuestionType { get; set; }

        [XmlAttribute("QuestionType")]
        public string QuestionTypeString
        {
            get
            {
                return this.QuestionType.ToString();
            }
            set
            {
                this.QuestionType = !Enum.IsDefined(typeof(QuestionType), (object)value) ? QuestionType.Unknown : (QuestionType)Enum.Parse(typeof(QuestionType), value);
            }
        }

        [XmlIgnore]
        public QuizType QuizType { get; set; }

        [XmlAttribute("QuizType")]
        public string QuizTypeString
        {
            get
            {
                return this.QuizType.ToString();
            }
            set
            {
                this.QuizType = !Enum.IsDefined(typeof(QuizType), (object)value) ? QuizType.Unknown : (QuizType)Enum.Parse(typeof(QuizType), value);
            }
        }

        [XmlAttribute]
        public int UserId { get; set; }

        [XmlAttribute]
        public string Question { get; set; }

        [XmlIgnore]
        public DateTime QuizDate
        {
            get
            {
                return this._quizDate;
            }
            set
            {
                this._quizDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
            }
        }

        [XmlAttribute("QuizDate")]
        public string QuizDateString
        {
            get
            {
                if (this.QuizDate == SingletonManager<Configuration>.Instance.OriginDate)
                    return string.Empty;
                return XmlConvert.ToString(this.QuizDate, "yyyy-MM-ddTHH:mm:ss");
            }
            set
            {
                this.QuizDate = !string.IsNullOrEmpty(value) ? XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified) : SingletonManager<Configuration>.Instance.OriginDate;
            }
        }

        [XmlIgnore]
        public List<PSResourceGroup> Options
        {
            get
            {
                return this._options;
            }
            set
            {
                this._options = value;
            }
        }

        [XmlAttribute("Options")]
        public string OptionsString
        {
            get
            {
                string str = string.Empty;
                for (int index = 0; index < this._options.Count; ++index)
                    str = str + (object)this._options[index].resourceId + "x" + (object)this._options[index].quantity + (index == this._options.Count - 1 ? (object)string.Empty : (object)",");
                return str;
            }
            set
            {
                if (!(value != string.Empty))
                    return;
                this._options = ((IEnumerable<PSResourceGroup>)SharedManager.ParseRewardStrings(value)).ToList<PSResourceGroup>();
            }
        }
    }
}