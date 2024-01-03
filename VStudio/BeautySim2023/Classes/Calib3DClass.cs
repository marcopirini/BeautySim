using BeautySim.Common;
using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using HitTestResult = HelixToolkit.Wpf.SharpDX.HitTestResult;

namespace BeautySim2023
{
    public class Calib3DClass : BaseViewModel, INotifyPropertyChanged
    {
     


        private delegate void SetIndicatorDelegate(System.Windows.Shapes.Ellipse bb, bool aactive);

        public event PropertyChangedEventHandler PropertyChanged;


        public void SetIndicator(System.Windows.Shapes.Ellipse bb, bool active)
        {
            if (!AppControl.Instance.CalibrationWindow.Dispatcher.CheckAccess())
            {
                SetIndicatorDelegate d = new SetIndicatorDelegate(SetIndicator);
                AppControl.Instance.CalibrationWindow.Dispatcher.Invoke(d, new object[] { bb, active });
            }
            else
            {
                bb.Fill = active ? AppControl.Instance.ActiveEllipse : System.Windows.Media.Brushes.Gray;
            }
        }



        private void PDIClass_OnConnectionStatusChanged(Device.Motion.CONNECTIONSTATUS status)
        {
            SetIndicator(AppControl.Instance.CalibrationWindow.elSensors, status == Device.Motion.CONNECTIONSTATUS.ACQUIRING);
        }
    }
}