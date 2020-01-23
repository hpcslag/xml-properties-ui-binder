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


        //TYPE CLASS
        class mystruct
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public enum TerminationModeEnum { HighZ = 1, Vterm = 2, ActiveLoad = 3 }

        class PinLevelStruct { 

            public string Vil { get; set; }
            public string Vih { get; set; }
            public string Vol { get; set; }
            public string Voh { get; set; }
            public TerminationModeEnum TerminationMode { get; set; }
            public double Vcom { get; set; }

            //public static Property Input { get; set; } //<xcamjksjakjd input="InputLevel"/> and need to bind this.
        }

        TableBinder<PinLevelStruct> bindingObject;
        public MainWindow()
        {
            InitializeComponent();

            //Read XML Document
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(TestXml);
            xmlDoc.Load("pinlevel.xml");


            //Loading bindingTools, AUTO Binding TYPE CLASS to generic
            bindingObject = new TableBinder<PinLevelStruct>(xmlDoc);


            //has namespace setup
            bindingObject.useNamespace("http://www.ni.com/Semiconductor/PinLevels");


            //set binding properties repeat node
            //bindingObject.bindRepeatNode("/MovieData/Movies/Movie");
            bindingObject.bindRepeatNode("/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
            bindingObject.bindRepeatNodeProperty("pin", "/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");


            //SET DATA TO UI
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
