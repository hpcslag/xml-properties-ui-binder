using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using XMLBinderUI.Libaray;

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
        public List<object> getDataGrid()
        {
            List<T> data = new List<T>();

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


            List<object> peBinding = propertyEditBind(data);
            return peBinding;
        }

        //ADD UI Edit <xxx attriute="xxxxx">
        private List<object> propertyEditBind(List<T> typedData)
        {
            //Examples
            //TypedClassBuilder tcb = new TypedClassBuilder("EditContent");
            //object createdClasses = tcb.CreateObject(new string[3] { "ID", "Name", "Address" }, new Type[3] { typeof(int), typeof(string), typeof(string) });
            //Type TYPED = createdClasses.GetType();
            //object instance = TypedClassBuilder.createInstance(TYPED, new object[] { 1, "hello", "bad" });
            /*foreach (PropertyInfo PI in TYPED.GetProperties())
            {
                string n = PI.Name;
                Console.WriteLine(PI.Name);
            }*/

            TypedClassBuilder tcb = new TypedClassBuilder("EditContent");
            
            //create classes base on <T>
            Dictionary<string, Type> classProps = new Dictionary<string, Type>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string propName = propertyInfo.Name;
                string qfname = propertyInfo.PropertyType.AssemblyQualifiedName;
                Type type = Type.GetType(qfname);
                classProps.Add(propName, type);
            }

            //create classes extend with bind object
            foreach (string bindedKeyName in bindedPropertyKeys)
            {
                classProps.Add(bindedKeyName, typeof(string));
            }

            object createdClasses = tcb.CreateObject(classProps.Keys.ToArray(), classProps.Values.ToArray());
            Type TYPED = createdClasses.GetType();

            //create (new XXXX()) to data
            List<object> data = new List<object>();

            int count = 0;
            foreach (T item in typedData)
            {
                
                //create value
                List<object> values = new List<object>(); //careful the values order may not same with TYPED...

                //add base data from <T>
                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    string value = propertyInfo.GetValue(item, null).ToString();

                    string qfname = propertyInfo.PropertyType.AssemblyQualifiedName;
                    Type type = Type.GetType(qfname);

                    try
                    {
                        MethodInfo method = typeof(TableBinder<T>).GetMethod("conv");
                        MethodInfo genericMethod = method.MakeGenericMethod(type);
                        object realData = genericMethod.Invoke(null, new object[] { value });
                        values.Add(realData);
                    }
                    catch
                    {
                        values.Add("");
                    }
                    
                }
                
                //add extend data
                int lenofkey = bindedPropertyKeys.ToArray().Length;
                int totalLength = typedData.ToArray().Length;
                for (int j = 0; j < lenofkey; j ++)
                {
                    //an = am + (n-m)*d
                    int index = (j * (lenofkey - totalLength)) + count;
                    string v = bindedProperties.ElementAt(index).value;
                    values.Add(v);
                }


                object instance = TypedClassBuilder.createInstance(TYPED, values.ToArray());//careful values order.
                data.Add(instance);

                count++;
            }

            return data;
        }


        //from UI(DataGrid) ItemSource back to XML Foramt
        private XmlDocument formatBackToData(System.Collections.IEnumerable data)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement(getParentPathName());
            
            foreach (object item in data)
            {
                XmlElement nodeParent = doc.CreateElement(getRepeatNodePathName());

                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)){
                    string typeName = propertyInfo.Name;

                    XmlElement node = doc.CreateElement(typeName);
                    string value;
                    try
                    {
                        value = propertyInfo.GetValue(item, null).ToString();
                    }
                    catch
                    {
                        value = "";
                    }
                    node.InnerText = value;


                    //check bindedProperty exists, then skip.
                    if(bindedPropertyKeys.Contains(typeName)){
                        nodeParent.SetAttribute(typeName, value);
                        continue;
                    }

                    nodeParent.AppendChild(node);
                }

                root.AppendChild(nodeParent);
            }

            doc.AppendChild(root);
            return doc;
        }

        //make changes by UI
        public void changeXMLData(System.Collections.IEnumerable data)
        {
            XmlDocument changedDoc = formatBackToData(data);

            XmlNode oldElem = xmlDoc.SelectSingleNode(getParentPath(), nsmgr);
            //XmlNode newElem = changedDoc.SelectSingleNode(getParentPathName(), nsmgr);

            oldElem.InnerXml = changedDoc.InnerXml;
        }

        //get current XML to string
        public string getXMLString() {
            StringWriter string_writer = new StringWriter();
            XmlTextWriter xml_text_writer = new XmlTextWriter(string_writer);
            xml_text_writer.Formatting = Formatting.Indented;
            xmlDoc.WriteTo(xml_text_writer);

            return string_writer.ToString();
        }
    }
}
