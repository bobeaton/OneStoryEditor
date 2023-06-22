using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using SIL.IO;
using System.Xml.Serialization;

namespace OseCommon
{
    public class OseXmlSerializer
    {
        public delegate void ExceptionHandler(Exception ex);

        public static void SaveDoc(string filePath, XDocument doc, ExceptionHandler exceptionHandler, int maxAttempt = 2)
        {
            var attempt = 0;
            var finfo = new FileInfo(filePath);
            Exception error = null;
            while (attempt < maxAttempt)
            {
                SerializeXmlToFileWithWriteThrough(filePath, doc.Root, out error);
                if (error == null)
                    break;
                exceptionHandler(error);

                if (attempt++ == 0 && finfo.IsReadOnly)
                {
                    try
                    {
                        finfo.IsReadOnly = false;
                    }
                    catch (Exception e)
                    {
                        exceptionHandler(e);
                    }
                }
            }

            // This is used for core OneStory data files. If we can't save it for some reason,
            //  the problem probably isn't going to magically go away.
            if (attempt > 2)
                throw new ApplicationException($"Unable to save file: {filePath}: {error}");
        }

        public static void SerializeXmlToFileWithWriteThrough<T>(string path, T data, out Exception e)
        {
            e = null;

            try
            {
                // ENHANCE: For perhaps even more robustness, we could try using
                // TempFileForSafeWriting or create a backup before serializing and then the
                // caller could attempt to deserialize from the backup if the serialized file
                // was corrupt...
                SerializeXmlToFileWithWriteThrough(path, data);
            }
            catch (Exception outEx)
            {
                e = outEx;
            }
        }

        /// <summary>
        /// Stolen from SIL.Core so that we could have an XML Serializer that writes UTF-16
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void SerializeXmlToFileWithWriteThrough<T>(string path, T data, XmlSerializerNamespaces ns = null)
        {
            // Note: RobustFile.Create() uses FileOptions.WriteThrough which causes the data to still be
            //       written to the operating system cache but it is immediately flushed. If this doesn't
            //       address the problem then a more thorough solution that completely bypasses the cache
            //       is to use the c++ CreateFile() api and pass both FILE_FLAG_NO_BUFFERING and
            //       FILE_FLAG_WRITE_THROUGH.
            //       https://docs.microsoft.com/en-us/windows/win32/fileio/file-caching
            using var writer = RobustFile.Create(path);
            var xmlSerializer = new XmlSerializer(typeof(T));
            if (ns == null)
                xmlSerializer.Serialize(writer, data);
            else
                xmlSerializer.Serialize(writer, data, ns);
        }
    }
}
