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

        //checkout is number type
        private static bool IsNumericType(Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        //try to parse enum type
        public static T conv<T>(String value)
        {
            //changed to enum
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value, true);


            //zero exception
            if (IsNumericType(typeof(T)) && value == "")
            {
                value = "0";
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        //from XML to DataGrid UI.
        //public List<T> getDataGrid<T>()
        public List<T> getDataGrid()
        {
            List<T> data = new List<T>();

            string s = parentPath;
            var m = nsmgr;
            XmlNodeList nodes = xmlDoc.SelectNodes(parentPath, nsmgr);
            foreach (XmlNode childrenNode in nodes)
            {
                //create generic instance
                T item = (T)Activator.CreateInstance(typeof(T), null);

                //auto iterate TYPE CLASS
                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    string typeName = propertyInfo.PropertyType.Name;
                    string propName = propertyInfo.Name;
                    string nodeName = applyNSPath(propName);

                    //也要加 namespace \a:
                    string value;
                    try { 
                        value = childrenNode.SelectSingleNode(nodeName, nsmgr).InnerText;
                    }catch{
                        value = ""; //no this property in current, auto add in
                    }

                    //setup generic type property
                    PropertyInfo T_instance = typeof(T).GetProperty(propName);

                    string qfname = propertyInfo.PropertyType.AssemblyQualifiedName;
                    Type type = Type.GetType(qfname);

                    try
                    {

                        //object classInstance = Activator.CreateInstance(propertyInfo.PropertyType.Assembly.FullName, type.FullName);
                        
                        //目標: 自動化建構 value type reflaction 到 datagrid 上, 目前的 enum 需要可以自動轉換 type
                        //other: Enum Menu 可以列出來嗎?
                        //other: Property: input="InputLevel" 參數可以指定新增嗎?

                        //object realData = typeof(TableBinder<T>).GetMethod("conv").Invoke(classInstance, new object[] { value });//Convert.ChangeType(value, type);
                        
                        //under following worked!
                        //object realData = conv<XMLBinderUI.MainWindow.TerminationModeEnum>(value);//Convert.ChangeType(value, type);
                        //object realData = conv<XMLBinderUI.MainWindow.TerminationModeEnum>(value);//Convert.ChangeType(value, type);

                        MethodInfo method = typeof(TableBinder<T>).GetMethod("conv");
                        MethodInfo genericMethod = method.MakeGenericMethod(type);
                        object realData = genericMethod.Invoke(null, new object[] { value });

                        
                        //set generic type property
                        T_instance.SetValue(item, realData, null);
                    }
                    catch {


                        //set generic type property
                        T_instance.SetValue(item, "", null);
                    }

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

            XmlNode oldElem = xmlDoc.SelectSingleNode(getParentPath(), nsmgr);
            XmlNode newElem = changedDoc.SelectSingleNode(getParentPathName(), nsmgr);

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
        /*public void showData() {

            XmlNodeList nodes = xmlDoc.SelectNodes(parentPath, nsmgr);
            foreach (XmlNode childrenNode in nodes)
            {
                foreach(property p in propertiesColumn)
                {
                    Console.WriteLine("Movie - " + p.description + ": " + childrenNode.SelectSingleNode(p.pname, nsmgr).InnerText);
                }
                Console.WriteLine("--------------");
            }
        }*/
    }
}
