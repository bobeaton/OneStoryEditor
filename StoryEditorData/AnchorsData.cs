using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public class AnchorData
    {
        public string JumpTarget = null;
        public string ToolTipText = null;
        public ExegeticalHelpNotesData ExegeticalHelpNotes = null;

        public AnchorData(string strJumpTarget, string strComment)
        {
            JumpTarget = strJumpTarget;
            ToolTipText = strComment;
            ExegeticalHelpNotes = new ExegeticalHelpNotesData();
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(JumpTarget));
                XElement elemAnchor = new XElement("anchor", new XAttribute("jumpTarget", JumpTarget),
                    new XElement("toolTip", ToolTipText));
                if (ExegeticalHelpNotes.HasData)
                    elemAnchor.Add(ExegeticalHelpNotes.GetXml);
                return elemAnchor;
            }
        }
    }

    public class AnchorsData : List<AnchorData>
    {
        public AnchorsData()
        {
        }

        public AnchorData AddAnchorData(string strJumpTarget, string strComment)
        {
            AnchorData anAD = new AnchorData(strJumpTarget, strComment);
            this.Add(anAD);
            return anAD;
        }

        public bool HasData
        {
            get { return (this.Count > 0); }
        }

        public XElement GetXml
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasData, "trying to serialize an AnchorsData without items");
                XElement elemAnchors = new XElement("anchors");
                foreach (AnchorData anAnchorData in this)
                    elemAnchors.Add(anAnchorData.GetXml);
                return elemAnchors;
            }
        }
    }
}
