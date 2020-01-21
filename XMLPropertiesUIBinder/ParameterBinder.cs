using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLPropertiesUIBinder
{
    class ParameterBinder : BaseBinder
    {
        public ParameterBinder(XmlDocument xmlfiles) : base(xmlfiles)
        {
        }

        private struct property {
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

        string parent = "";


        public void bindRepeatNode(string path)
        {
            this.parent = path;
        }

        private List<property> propertiesColumn = new List<property>();
        public void setPropertyColumn(string pname, string description, string type)
        {
            propertiesColumn.Add(new property (pname, description, type));
        }

        public void bindToUI(){
            //on changed and back to xml
        }

        public void showData() {
            XmlNodeList nodes = xmlDoc.SelectNodes(parent);

            foreach (XmlNode childrenNode in nodes)
            {
                foreach(property p in propertiesColumn)
                {
                    Console.WriteLine("Movie - "+ p.description + ": " + childrenNode.SelectSingleNode(p.pname).InnerText);
                }
                Console.WriteLine("--------------");
            }
        }
    }
}
