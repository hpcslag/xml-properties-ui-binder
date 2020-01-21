using System;
using System.Collections.Generic;
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

        protected string parent = "";
        public string parentTitle = "";

        public void bindRepeatNode(string path)
        {
            this.parent = path;
        }

    }
}
