using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OseProjectData
{
    public static class CSharpExtensions
    {
        public static T ParseXml<T>(string strContents)
        {
            if (String.IsNullOrEmpty(strContents))
                return default(T);

            Console.WriteLine(String.Format("Loading Xml: '{0}'...", strContents.Substring(0, Math.Min(strContents.Length, 50))));
            var serializer = new XmlSerializer(typeof(T));
            var readerSettings = new XmlReaderSettings { IgnoreWhitespace = false };
            using (var reader = XmlReader.Create(new StreamReader(strContents.ToStream(), Encoding.UTF8), readerSettings))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Turn a string into a Stream
        /// </summary>
        /// <param name="str">string to turn into a stream</param>
        /// <returns>the (Memory)Stream created</returns>
        public static Stream ToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
