using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using XMLPropertiesUIBinder;

namespace XMLBinderUI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

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

        class mystruct
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        TableBinder<mystruct> bindingObject;
        public MainWindow()
        {
            InitializeComponent();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(TestXml);

            bindingObject = new TableBinder<mystruct>(xmlDoc);
            bindingObject.bindRepeatNode("/MovieData/Movies/Movie");
            bindingObject.setPropertyColumn("Id", "Movie ID", "TextInput");
            bindingObject.setPropertyColumn("Name", "Movie Name", "TextInput");


            ///combine data
            //MessageBox.Show(bindingObject.getRepeatNodePathName());
            //MessageBox.Show(bindingObject.getParentPathName());


            //////////
            DataGrid movieData = (DataGrid)this.FindName("movieData");
            movieData.ItemsSource = bindingObject.getDataGrid();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //generate xml
            DataGrid movieData = (DataGrid)this.FindName("movieData");
            bindingObject.changeXMLData(movieData.ItemsSource);

            MessageBox.Show(bindingObject.getXMLString());
        }
    }

    
}
