using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    public class Serialize
    {

        // Use this for initialization
        public static string Serialization(object o)
        {
            string text = "";
            TextWriter writer;
            XmlSerializer serialWrite = new XmlSerializer(o.GetType());
            writer = new StringWriter();
            serialWrite.Serialize(writer, o);
            text = writer.ToString();
            writer.Close();
            return text;
        }

        public static object Deserialization(object o, string xml_text)
        {
            TextReader reader;
            XmlSerializer serialRead = new XmlSerializer(o.GetType());
            reader = new StringReader(xml_text);
            o = serialRead.Deserialize(reader);
            reader.Close();
            return o;
        }

        public static object Deserialization(string path, object o)
        {
            //players level = new players ();
            XmlSerializer preferences = new XmlSerializer(o.GetType());
            Stream reader;
            try
            {
                reader = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                Serialization(path, o);
                reader = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            o = preferences.Deserialize(reader);
            reader.Close();
            return o;
        }

        public static void Serialization(string path, object o)
        {
            Stream writer;
            XmlSerializer serialWrite = new XmlSerializer(o.GetType());
            writer = new FileStream(path, FileMode.Create, FileAccess.Write);
            serialWrite.Serialize(writer, o);
            writer.Close();
        }
    }
}
