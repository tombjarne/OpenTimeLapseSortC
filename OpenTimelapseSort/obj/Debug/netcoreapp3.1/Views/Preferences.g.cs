﻿#pragma checksum "..\..\..\..\Views\Preferences.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "888019A7C663DD8148A9EA7827B6FCC5BA655AC7"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// PreferencesView
    /// </summary>
    public partial class PreferencesView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Name;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Headline1;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Copy;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider IntervalSlider;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider IntervalCountSlider;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Interval;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label IntervalCount;
        
        #line default
        #line hidden
        
        
        #line 176 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox AutoDetectionBtn;
        
        #line default
        #line hidden
        
        
        #line 185 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider GenerositySlider;
        
        #line default
        #line hidden
        
        
        #line 198 "..\..\..\..\Views\Preferences.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Generosity;
        
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
            System.Uri resourceLocater = new System.Uri("/OpenTimelapseSort;component/views/preferences.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Preferences.xaml"
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
            this.Name = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            
            #line 47 "..\..\..\..\Views\Preferences.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.ClosePreferencesWindow);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 54 "..\..\..\..\Views\Preferences.xaml"
            ((System.Windows.Shapes.Rectangle)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveWindow);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Headline1 = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.Copy = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.IntervalSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 120 "..\..\..\..\Views\Preferences.xaml"
            this.IntervalSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.UpdateSliderIntervalValue);
            
            #line default
            #line hidden
            return;
            case 7:
            this.IntervalCountSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 131 "..\..\..\..\Views\Preferences.xaml"
            this.IntervalCountSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.UpdateSliderCountValue);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Interval = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.IntervalCount = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            
            #line 162 "..\..\..\..\Views\Preferences.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SavePreferences);
            
            #line default
            #line hidden
            return;
            case 11:
            this.AutoDetectionBtn = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            this.GenerositySlider = ((System.Windows.Controls.Slider)(target));
            
            #line 196 "..\..\..\..\Views\Preferences.xaml"
            this.GenerositySlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.UpdateGenerosityValue);
            
            #line default
            #line hidden
            return;
            case 13:
            this.Generosity = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

