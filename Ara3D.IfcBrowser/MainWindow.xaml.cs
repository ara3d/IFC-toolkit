using System.Diagnostics;
using Ara3D.NarwhalDB;
using System.Windows;
using Ara3D.IfcParser;
using Ara3D.Logging;
using Ara3D.Utils;
using Microsoft.Win32;

namespace Ara3D.IfcBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DirectoryPath OutputFolder
            = @"C:\Users\cdigg\dev\impraria\propdb";

        public ILogger Logger;

        public MainWindow()
        {
            InitializeComponent();
            var logWriter = LogWriter.Create(OnLogMessage);
            Logger = new Logger(logWriter, "");
            Logger.Log("Welcome to the Ara 3D property database viewer");
            Logger.Log("This is a prototype GUI for loading / viewing IFC property data");
            Logger.Log("Which has been created as a NarwhalDB and saved as BFAST");
            Grid.RowHeight = 20;
            Grid.ColumnWidth = 100;
            Grid.EnableColumnVirtualization = true;
            Grid.EnableRowVirtualization = true;
        }

        public void OnLogMessage(string logMsg)
        {
            LogTextBox.AppendText(logMsg + Environment.NewLine);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Logger.Log($"User initiating file chose action");
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\Users\cdigg\dev\impraria\propdb";
            if (!ofd.ShowDialog().Value)
            {
                Logger.Log($"No file chosen to open");
                return;
            }

            var sw = Stopwatch.StartNew();
            Logger.Log("Initiating data load");
            var fp = OutputFolder.RelativeFile(ofd.FileName);
            Logger.Log($"Reading database {fp.GetFileName()} from disk");
            var buffers = BFastReader2.Read(fp, Logger);
            Logger.Log($"Read {buffers.Count} buffers");
            var reader = new IfcPropertyDataReader(buffers);
            Logger.Log("Created data reader");
            Logger.Log($"Initializing the DataGrid");
            Grid.ItemsSource = new PropData(reader);
            var timeStr = sw.PrettyPrintTimeElapsed();
            Logger.Log($"Load and initialization duration = {timeStr}");
        }
    }
}