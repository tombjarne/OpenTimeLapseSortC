﻿#pragma checksum "..\..\..\..\Views\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "AF1C5367DB27DC7A2AE545FEC688FB3010A4343D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using FontAwesome.WPF;
using FontAwesome.WPF.Converters;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace OpenTimelapseSort.Views {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 318 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Pane;
        
        #line default
        #line hidden
        
        
        #line 322 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MenuBar;
        
        #line default
        #line hidden
        
        
        #line 330 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ApplicationName;
        
        #line default
        #line hidden
        
        
        #line 342 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PreferencesBtn;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImportBtn;
        
        #line default
        #line hidden
        
        
        #line 381 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FontAwesome.WPF.FontAwesome Loader;
        
        #line default
        #line hidden
        
        
        #line 395 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse CloseBtn;
        
        #line default
        #line hidden
        
        
        #line 404 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Ellipse MinimizeBtn;
        
        #line default
        #line hidden
        
        
        #line 423 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Warning;
        
        #line default
        #line hidden
        
        
        #line 431 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FontAwesome.WPF.FontAwesome WarningIcon;
        
        #line default
        #line hidden
        
        
        #line 440 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ErrorMessage;
        
        #line default
        #line hidden
        
        
        #line 450 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel LeftPanel;
        
        #line default
        #line hidden
        
        
        #line 475 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Calendar SortingCalendar;
        
        #line default
        #line hidden
        
        
        #line 521 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer DirectoryViewer;
        
        #line default
        #line hidden
        
        
        #line 527 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView DirectoryViewer1;
        
        #line default
        #line hidden
        
        
        #line 537 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel RightPanel;
        
        #line default
        #line hidden
        
        
        #line 548 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer ImageViewer;
        
        #line default
        #line hidden
        
        
        #line 555 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ImageViewer1;
        
        #line default
        #line hidden
        
        
        #line 570 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DirectoryName;
        
        #line default
        #line hidden
        
        
        #line 599 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label DirectoryPath;
        
        #line default
        #line hidden
        
        
        #line 614 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup ImportPopup;
        
        #line default
        #line hidden
        
        
        #line 630 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ImportHeadline;
        
        #line default
        #line hidden
        
        
        #line 643 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImportTarget;
        
        #line default
        #line hidden
        
        
        #line 654 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChooseDirectoryBtn;
        
        #line default
        #line hidden
        
        
        #line 672 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImportAbortBtn;
        
        #line default
        #line hidden
        
        
        #line 690 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ImportConfirmBtn;
        
        #line default
        #line hidden
        
        
        #line 712 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup WarningPopup;
        
        #line default
        #line hidden
        
        
        #line 728 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ErrorHead;
        
        #line default
        #line hidden
        
        
        #line 741 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorDesc;
        
        #line default
        #line hidden
        
        
        #line 751 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ErrorBtn;
        
        #line default
        #line hidden
        
        
        #line 771 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.Popup ImportProgressPopup;
        
        #line default
        #line hidden
        
        
        #line 800 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ImportProgressCount;
        
        #line default
        #line hidden
        
        
        #line 812 "..\..\..\..\Views\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SortingCountdown;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OpenTimelapseSort;component/views/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Pane = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.MenuBar = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.ApplicationName = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.PreferencesBtn = ((System.Windows.Controls.Button)(target));
            
            #line 351 "..\..\..\..\Views\MainWindow.xaml"
            this.PreferencesBtn.Click += new System.Windows.RoutedEventHandler(this.InvokePreferences);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ImportBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.Loader = ((FontAwesome.WPF.FontAwesome)(target));
            return;
            case 7:
            this.CloseBtn = ((System.Windows.Shapes.Ellipse)(target));
            
            #line 402 "..\..\..\..\Views\MainWindow.xaml"
            this.CloseBtn.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.CloseApplication);
            
            #line default
            #line hidden
            return;
            case 8:
            this.MinimizeBtn = ((System.Windows.Shapes.Ellipse)(target));
            
            #line 411 "..\..\..\..\Views\MainWindow.xaml"
            this.MinimizeBtn.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MinimizeApplication);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 420 "..\..\..\..\Views\MainWindow.xaml"
            ((System.Windows.Shapes.Rectangle)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveWindow);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Warning = ((System.Windows.Controls.Grid)(target));
            return;
            case 11:
            this.WarningIcon = ((FontAwesome.WPF.FontAwesome)(target));
            return;
            case 12:
            this.ErrorMessage = ((System.Windows.Controls.TextBox)(target));
            return;
            case 13:
            this.LeftPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 14:
            this.SortingCalendar = ((System.Windows.Controls.Calendar)(target));
            return;
            case 15:
            this.DirectoryViewer = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 524 "..\..\..\..\Views\MainWindow.xaml"
            this.DirectoryViewer.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.DirectoryViewer_OnPreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 16:
            this.DirectoryViewer1 = ((System.Windows.Controls.ListView)(target));
            return;
            case 17:
            this.RightPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 18:
            this.ImageViewer = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 552 "..\..\..\..\Views\MainWindow.xaml"
            this.ImageViewer.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ImageViewer_OnPreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 19:
            this.ImageViewer1 = ((System.Windows.Controls.ListView)(target));
            return;
            case 20:
            this.DirectoryName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 21:
            this.DirectoryPath = ((System.Windows.Controls.Label)(target));
            return;
            case 22:
            this.ImportPopup = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 23:
            this.ImportHeadline = ((System.Windows.Controls.Label)(target));
            return;
            case 24:
            this.ImportTarget = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 25:
            this.ChooseDirectoryBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 26:
            this.ImportAbortBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 27:
            this.ImportConfirmBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 28:
            this.WarningPopup = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 29:
            this.ErrorHead = ((System.Windows.Controls.Label)(target));
            return;
            case 30:
            this.ErrorDesc = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 31:
            this.ErrorBtn = ((System.Windows.Controls.Button)(target));
            return;
            case 32:
            this.ImportProgressPopup = ((System.Windows.Controls.Primitives.Popup)(target));
            return;
            case 33:
            this.ImportProgressCount = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 34:
            this.SortingCountdown = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

