# XML Properties UI Binder
This is an powerful tools that make XML config file can be edit by C# (WPF/Native) Data Grid View.

# Feature - Repeat Item Config

Reference file: [pinlevel.xml](https://github.com/hpcslag/xml-properties-ui-binder/blob/master/XMLBinderUI/pinlevel.xml)

## 1. Define Repeat Struct Config

```cs
public enum TerminationModeEnum { HighZ = 1, Vterm = 2, ActiveLoad = 3 }
class PinLevelStruct { 

    public string Vil { get; set; }
    public string Vih { get; set; }
    public string Vol { get; set; }
    public string Voh { get; set; }
    public TerminationModeEnum TerminationMode { get; set; }
    public double Vcom { get; set; }

}
```

## 2. Load XML File and Apply Struct Config
```cs
XmlDocument xmlDoc2 = new XmlDocument();
xmlDoc2.Load("pinlevel.xml");

//Loading bindingTools, AUTO Binding TYPE CLASS to generic
TableBinder<PinLevelStruct> bindingObject2 = new TableBinder<PinLevelStruct>(xmlDoc2);

//has namespace setup
bindingObject2.useNamespace("http://www.ni.com/Semiconductor/PinLevels");

//set binding properties repeat node
bindingObject2.bindRepeatNode("/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
bindingObject2.bindRepeatNodeProperty("pin", "/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
bindingObject2.bindRepeatNodeProperty("task", "/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
bindingObject2.bindRepeatNodeProperty("mask", "/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
bindingObject2.bindRepeatNodeProperty("sds", "/PinLevelsFile/PinLevelsSheet/DigitalPinLevelSets/DigitalPinLevelSet");
```

## 3. Bind to UI and other feature
```cs
//bind to UI
DataGrid pinlevel = (DataGrid)this.FindName("pinLevelData");
pinlevel.ItemsSource = bindingObject2.getDataGrid();
this.btn2.Click += (object sender, RoutedEventArgs e) =>
{
    DataGrid md = (DataGrid)this.FindName("pinLevelData");
    bindingObject2.changeXMLData(md.ItemsSource);
    MessageBox.Show(bindingObject2.getXMLString());
};
```

# Feature - Single Independ Item Config

# Screenshot
![](https://i.imgur.com/OduYC7j.png)
