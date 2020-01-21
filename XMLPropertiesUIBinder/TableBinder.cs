using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLPropertiesUIBinder
{
    class TableBinder:BaseBinder
    {

        public TableBinder(XmlDocument xmlfiles) : base (xmlfiles)
        {
            //setup column
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
