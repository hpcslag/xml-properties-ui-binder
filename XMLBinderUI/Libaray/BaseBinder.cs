using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLPropertiesUIBinder
{
    class BaseBinder
    {

        protected XmlDocument xmlDoc;

        public BaseBinder(XmlDocument xmlfiles)
        {
            this.xmlDoc = xmlfiles;
        }

        protected struct property
        {
            public string pname;
            public string description;
            public string type;

            public property(string pname, string description, string type)
            {
                this.pname = pname;
                this.description = description;
                this.type = type;
            }
        }

        protected List<property> propertiesColumn = new List<property>();
        public void setPropertyColumn(string pname, string description, string type)
        {
            propertiesColumn.Add(new property(pname, description, type));
        }

        protected string parentPath = "";
        public string parentTitle = "";

        //AA/BB/CC/DD

        //Get CC
        public string getParentPathName()
        {
            return Path.GetDirectoryName(parentPath).Split(Path.DirectorySeparatorChar).Last();
        }

        //Get AA/BB/CC
        public string getParentPath()
        {
            return Path.GetDirectoryName(parentPath).Replace(@"\\", @"/").Replace(@"\", @"/");;
        }

        //Get DD
        public string getRepeatNodePathName()
        {
            return Path.GetFileName(parentPath);
        }

        //set path -> AA/BB/CC
        public void bindRepeatNode(string path)
        {
            this.parentPath = path;
        }

    }
}
