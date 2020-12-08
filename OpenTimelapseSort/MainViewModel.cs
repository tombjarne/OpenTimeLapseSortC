using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace OpenTimelapseSort.ViewModels
{
    class MainWindow
    {
        public delegate void InvokeImport();
        public event InvokeImport Invocation;

        public static void Import()
        {
            CommonOpenFileDialog fileChooser = new CommonOpenFileDialog();
            fileChooser.InitialDirectory = @"C:\";
            //fileChooser.Title = "Choose Directory";
            fileChooser.IsFolderPicker = true;

            if (fileChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //TODO: fetch needed attributes
                string filename = fileChooser.FileName;
                //renderDirectories(fileChooser.FileNames, fileChooser.FileName);
            }
        }

    }
}
