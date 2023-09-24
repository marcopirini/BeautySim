﻿#pragma checksum "..\..\ManikinControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "55B321F589E3B66083CCCDFFFE8F28EC904E13CADBBCE2FBC14E5D7C821BA0C8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BeautySimStartingApp;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace BeautySimStartingApp {
    
    
    /// <summary>
    /// ManikinControl
    /// </summary>
    public partial class ManikinControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 69 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.SharpDX.Viewport3DX hvView3D;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D noSkinHead;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D noSkinHead2;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel spSystems;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel spControls;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbProbe;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbHead;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbArteries;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbVeins;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\ManikinControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbNerves;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BeautySimStartingApp;component/manikincontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ManikinControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.hvView3D = ((HelixToolkit.Wpf.SharpDX.Viewport3DX)(target));
            
            #line 69 "..\..\ManikinControl.xaml"
            this.hvView3D.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.hvView3D_PreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 69 "..\..\ManikinControl.xaml"
            this.hvView3D.Loaded += new System.Windows.RoutedEventHandler(this.HvView3D_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.noSkinHead = ((HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D)(target));
            
            #line 76 "..\..\ManikinControl.xaml"
            this.noSkinHead.Mouse3DDown += new System.EventHandler<HelixToolkit.Wpf.SharpDX.MouseDown3DEventArgs>(this.MeshGeometryModel3D_Mouse3DDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.noSkinHead2 = ((HelixToolkit.Wpf.SharpDX.MeshGeometryModel3D)(target));
            
            #line 81 "..\..\ManikinControl.xaml"
            this.noSkinHead2.Mouse3DDown += new System.EventHandler<HelixToolkit.Wpf.SharpDX.MouseDown3DEventArgs>(this.MeshGeometryModel3D_Mouse3DDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.spSystems = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.spControls = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.cbProbe = ((System.Windows.Controls.CheckBox)(target));
            
            #line 126 "..\..\ManikinControl.xaml"
            this.cbProbe.Checked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            
            #line 126 "..\..\ManikinControl.xaml"
            this.cbProbe.Unchecked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.cbHead = ((System.Windows.Controls.CheckBox)(target));
            
            #line 127 "..\..\ManikinControl.xaml"
            this.cbHead.Checked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            
            #line 127 "..\..\ManikinControl.xaml"
            this.cbHead.Unchecked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            return;
            case 8:
            this.cbArteries = ((System.Windows.Controls.CheckBox)(target));
            
            #line 128 "..\..\ManikinControl.xaml"
            this.cbArteries.Checked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            
            #line 128 "..\..\ManikinControl.xaml"
            this.cbArteries.Unchecked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.cbVeins = ((System.Windows.Controls.CheckBox)(target));
            
            #line 129 "..\..\ManikinControl.xaml"
            this.cbVeins.Checked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            
            #line 129 "..\..\ManikinControl.xaml"
            this.cbVeins.Unchecked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.cbNerves = ((System.Windows.Controls.CheckBox)(target));
            
            #line 130 "..\..\ManikinControl.xaml"
            this.cbNerves.Checked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            
            #line 130 "..\..\ManikinControl.xaml"
            this.cbNerves.Unchecked += new System.Windows.RoutedEventHandler(this.cbsModified_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

