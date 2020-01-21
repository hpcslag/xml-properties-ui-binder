using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace XMLPropertiesUIBinder
{
    class TableBinder<T>:BaseBinder
    {

        public TableBinder(XmlDocument xmlfiles) : base (xmlfiles)
        {
            //setup column
        }

        //from XML to DataGrid UI.
        //public List<T> getDataGrid<T>()
        public List<T> getDataGrid()
        {
            List<T> data = new List<T>();

            XmlNodeList nodes = xmlDoc.SelectNodes(parentPath);
            foreach (XmlNode childrenNode in nodes)
            {
                //create generic instance
                T item = (T)Activator.CreateInstance(typeof(T), null);

                foreach (property p in propertiesColumn)
                {
                    string value = childrenNode.SelectSingleNode(p.pname).InnerText;

                    //set generic type property
                    PropertyInfo T_instance = typeof(T).GetProperty(p.pname);
                    T_instance.SetValue(item, value, null);
                }

                //set to data
                data.Add(item);
            }

            return data;
        }


        //from UI(DataGrid) ItemSource back to XML Foramt
        private XmlDocument formatBackToData(System.Collections.IEnumerable data)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement(getParentPathName());
            
            foreach (T item in data)
            {
                XmlElement nodeParent = doc.CreateElement(getRepeatNodePathName());

                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)){
                    string typeName = propertyInfo.Name;

                    XmlElement node = doc.CreateElement(typeName);
                    node.InnerText = propertyInfo.GetValue(item, null).ToString();

                    nodeParent.AppendChild(node);
                }

                root.AppendChild(nodeParent);
            }

            //referenced:
            /*foreach (mystruct item in movieData.ItemsSource)
            {
                MessageBox.Show(item.Id.ToString() + ", " + item.Name.ToString());
            }*/
            

            doc.AppendChild(root);
            return doc;
        }

        //make changes by UI
        public void changeXMLData(System.Collections.IEnumerable data)
        {
            XmlDocument changedDoc = formatBackToData(data);

            XmlNode oldElem = xmlDoc.SelectSingleNode(getParentPath());
            XmlNode newElem = changedDoc.SelectSingleNode(getParentPathName());

            oldElem.InnerXml = newElem.InnerXml;
        }

        //get current XML to string
        public string getXMLString() {
            StringWriter string_writer = new StringWriter();
            XmlTextWriter xml_text_writer = new XmlTextWriter(string_writer);
            xml_text_writer.Formatting = Formatting.Indented;
            xmlDoc.WriteTo(xml_text_writer);

            return string_writer.ToString();
        }

        //Console debug mode.
        public void showData() {

            XmlNodeList nodes = xmlDoc.SelectNodes(parentPath);
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
