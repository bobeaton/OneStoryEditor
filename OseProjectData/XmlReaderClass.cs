using System;
using System.IO;
using System.Web;
using OseProjectData.External.Models;

namespace OseProjectData
{
    // from http://www.c-sharpcorner.com/UploadFile/xiankaylle/read-data-from-xml-in-Asp-Net-mvc-5/
    public class XmlReaderClass
    {
        public static StoryProject GetStoryProject(string strProjectId)
        {
            var strProjectFileSpec = String.Format("~/App_Data/{0}.xml", strProjectId);
            strProjectFileSpec = HttpContext.Current.Server.MapPath(strProjectFileSpec);
            var strContents = File.ReadAllText(strProjectFileSpec);
            var data = CSharpExtensions.ParseXml<StoryProject>(strContents);
            return data;
        }
    }
}
