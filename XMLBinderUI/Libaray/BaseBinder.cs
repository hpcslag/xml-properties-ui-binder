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

        //bind property: <DigitalPinLevelSet pin="Inputs">
        public void bindProperty(string path, string pname)
        {

        }


        protected bool useNamespce = false;
        public XmlNamespaceManager nsmgr = null;
        public void useNamespace(string xmlns){
            nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("a", xmlns);
            useNamespce = true;
            //var dn = xmlDoc.SelectNodes("/a:PinLevelsFile/a:PinLevelsSheet/a:DigitalPinLevelSets/a:DigitalPinLevelSet", nsmgr);
        }

        //apply namespace path
        public string applyNSPath(string spath){

            if (useNamespce)
            {
                spath = "a:" + spath;
            }

            return spath;
        }

        public string applyNSPathPrefix(string spath)
        {
            spath = "/" + spath.Replace(@"//", @"/").Replace(@"\\", @"/").Replace(@"\", @"/");
            if (useNamespce)
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
            if (useNamespce)
            {
                path = path.Replace("/", "/a:");
            }

            this.parentPath = path;
        }

    }
}
