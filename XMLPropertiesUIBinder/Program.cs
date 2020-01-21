using System;
using System.Xml;

namespace XMLPropertiesUIBinder
{
	class Program { 
		private const string TestXml =
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			"<MovieData>" +
				"<Movies>" +
					"<Movie>" +
						"<Id>1</Id>" +
						"<Name>Batman</Name>" +
					"</Movie>" +
					"<Movie>" +
						"<Id>2</Id>" +
						"<Name>Batman Returns</Name>" +
					"</Movie>" +
					"<Movie>" +
						"<Id>3</Id>" +
						"<Name>Batman Dark Knight</Name>" +
					"</Movie>" +
				"</Movies>" +
			"</MovieData>";


		public static void Main()
		{

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(TestXml);

            TableBinder bindingObject = new TableBinder(xmlDoc);
			bindingObject.bindRepeatNode("/MovieData/Movies/Movie");
			bindingObject.setPropertyColumn("Id", "Movie ID", "TextInput");
			bindingObject.setPropertyColumn("Name", "Movie Name", "TextInput");


            bindingObject.showData();
			


			//XmlNode xnl = xmlDoc.SelectNodes;
			//Console.WriteLine(xnl);


			/*string xpath = "MovieData/Movies/Movie";
			var nodes = xmlDoc.SelectNodes(xpath);

			foreach (XmlNode childrenNode in nodes)
			{
				Console.WriteLine(
					"Movie - Id: " + childrenNode.SelectSingleNode(".//Id").InnerText +
					", Name: " + childrenNode.SelectSingleNode(".//Name").InnerText);
			}*/

			Console.ReadLine();
		}
	}
}