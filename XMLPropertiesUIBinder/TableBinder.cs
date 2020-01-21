using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLPropertiesUIBinder
{
    class TableBinder
    {

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

        public void showData() {
            foreach (XmlNode childrenNode in nodes)
            {
                foreach(property p in propertiesColumn)
                {
                    Console.WriteLine(
                    "Movie - Id: " + childrenNode.SelectSingleNode(p.pname).InnerText +
                    ", Name: " + childrenNode.SelectSingleNode(".//Name").InnerText);
                }
                
            }
        }
    }
}
