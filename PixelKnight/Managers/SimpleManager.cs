using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PixelKnight.Enums;

namespace PixelStarships
{
    public class SimpleManager
    {
        private void Awake()
        {
            if (!Directory.Exists("/Data"))
                Directory.CreateDirectory("/Data");
            if (Directory.Exists("/AssetFiles"))
                return;
            Directory.CreateDirectory("/AssetFiles");
        }

        public void SaveXml(string filename, string text, bool editorOnly = true)
        {
            
        }

        public void LoadXML(string filename, Action<string> loadCompleteDel)
        {

        }

        public void ReadXML(string url, SimpleManager.DownloadDelegate del, string text, string name, SimpleManager.ParseDelegate parse, Action failAction = null)
        {

        }

        public void GetDesignTableVersionOnDisk(string XMLDesignTableFileName, string XMLDesignTableName, SimpleManager.XMLVersionDelegate del)
        {
            this.LoadXML(XMLDesignTableFileName, (Action<string>)(xmlText =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(xmlText))
                    {
                        int result = 0;
                        XmlTextReader xmlTextReader = new XmlTextReader((TextReader)new StringReader(xmlText));
                        while (!xmlTextReader.EOF)
                        {
                            if (xmlTextReader.Name == XMLDesignTableName)
                            {
                                if (xmlTextReader.HasAttributes)
                                {
                                    while (xmlTextReader.MoveToNextAttribute())
                                    {
                                        if (xmlTextReader.Name == "version")
                                        {
                                            int.TryParse(xmlTextReader.Value, out result);
                                            break;
                                        }
                                    }
                                    break;
                                }
                                break;
                            }
                            xmlTextReader.Read();
                        }
                        del(result);
                    }
                    else
                        del(0);
                }
                catch (Exception ex)
                {
                    del(0);
                }
            }));
        }


        public delegate void DownloadDelegate(string url, string message, bool success, Action failAction = null, ErrorCode errorCode = ErrorCode.NoError);

        public delegate void XMLVersionDelegate(int version);

        public delegate void DownloadStackDelegate(Stack stack, string url, string message, bool success, Action failAction = null);

        public delegate void DownloadListDelegate<T>(List<T> stack, string url, string message, bool success, Action failAction = null);

        public delegate void ParseDelegate(string xml);

        public delegate void GenericDelegate();
    }
}
