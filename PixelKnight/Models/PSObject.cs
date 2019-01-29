using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PixelKnight.Models
{
    [Serializable]
    public class PSObject
    {
        public string Serialize()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            StringWriter stringWriter = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter)stringWriter, settings))
                new XmlSerializer(this.GetType()).Serialize(xmlWriter, (object)this, namespaces);
            return stringWriter.ToString();
        }

        public static T Deserialize<T>(string inputString) where T : PSObject
        {
            new XmlSerializerNamespaces().Add(string.Empty, string.Empty);
            return (T)new XmlSerializer(typeof(T), string.Empty).Deserialize(new XmlTextReader(new StringReader(inputString)));
        }
        
        public static List<T> DeserializeList<T>(string inputString) where T : PSObject
        {
            XmlReader reader = XDocument.Load(new StringReader(inputString)).Root.CreateReader();
            List<T> objList = (List<T>)new XmlSerializer(typeof(List<T>)).Deserialize(reader);
            reader.Close();
            return objList;
        }

        public delegate void GenericDelegate();
    }
}
