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

        protected SortedSet<string> bindedPropertyKeys = new SortedSet<string>();
        protected List<Property> bindedProperties = new List<Property>();
        //bind property: <DigitalPinLevelSet pin="Inputs">
        public void bindRepeatNodeProperty(string bindClassName, string path)
        {
            //每次 gene 新的 xml 需要重跑 repeat node, because order 會跑掉
            //other: Property: input="InputLevel" 參數可以指定新增嗎?

            //add key name set
            bindedPropertyKeys.Add(bindClassName);

            XmlNodeList nodes = xmlDoc.SelectNodes(applyNSPathPrefix(path), nsmgr);
            //bind property
            foreach (XmlNode childrenNode in nodes)
            {
                Property p = new Property(path, bindClassName);
                p.value = childrenNode.Attributes[bindClassName].Value;
                bindedProperties.Add(p);
            }
        }

        public string getSinglePropertyValue(string path, string propertyKey) {
            string attributeValue;
            if (this.isUseNamespace)
            {
                path = path.Replace("/", "/a:");
                attributeValue = xmlDoc.SelectSingleNode(path, nsmgr).Attributes[propertyKey].Value;
            }
            else
            {
                attributeValue = xmlDoc.SelectSingleNode(path).Attributes[propertyKey].Value;
            }
            return attributeValue;
        }


        protected bool isUseNamespace = false;
        public XmlNamespaceManager nsmgr = null;
        public void useNamespace(string xmlns){
            nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("a", xmlns);
            isUseNamespace = true;
            //var dn = xmlDoc.SelectNodes("/a:PinLevelsFile/a:PinLevelsSheet/a:DigitalPinLevelSets/a:DigitalPinLevelSet", nsmgr);
        }

        //apply namespace path
        public string applyNSPath(string spath){

            if (isUseNamespace)
            {
                spath = "a:" + spath;
            }

            return spath;
        }

        public string applyNSPathPrefix(string spath)
        {
            spath = spath.Replace(@"//", @"/").Replace(@"\\", @"/").Replace(@"\", @"/");
            if (isUseNamespace)
            {
                spath = spath.Replace("/", "/a:");
            }
            return spath;
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
            return Path.GetDirectoryName(parentPath).Replace(@"\\", @"/").Replace(@"\", @"/");
        }

        //Get DD
        public string getRepeatNodePathName()
        {
            return Path.GetFileName(parentPath);
        }

        //set path -> AA/BB/CC
        public void bindRepeatNode(string path)
        {
            if (isUseNamespace)
            {
                path = path.Replace("/", "/a:");
            }

            this.parentPath = path;
        }

    }

    class Property
    {

        public string propertyKey { get; set; }
        public string pathTarget { get; set; }
        public string value { get; set; }

        public Property(string pathTarget, string propertyKey)
        {
            this.pathTarget = pathTarget;
            this.propertyKey = propertyKey;
            this.value = "";
        }
    }
}
