using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace StraxeHelpers
{
    public class XmlHelper
    {
        public static string SerializeToXml<T>(T dataToSerialize)
        {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }
        public static List<T> DeserializeList<T>(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            XmlReader reader = doc.CreateReader();
            List<T> result = (List<T>)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }
        public static T DeserializeModel<T>(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);
            var serializer = new XmlSerializer(typeof(T));
            var reader = doc.CreateReader();
            var result = (T)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }
    }
}
