using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeautySim2023
{
    public class CustomBorder : Border
    {
        public static readonly DependencyProperty YourIntegerPropertyProperty =
            DependencyProperty.Register("YourIntegerProperty", typeof(int), typeof(CustomBorder), new PropertyMetadata(0));

        public CustomBorder() : base()
        {
            YourIntegerProperty = 0;
        }

        public int YourIntegerProperty
        {
            get { return (int)GetValue(YourIntegerPropertyProperty); }
            set { SetValue(YourIntegerPropertyProperty, value); }
        }

        internal void UpdateBackground()
        {
            switch (YourIntegerProperty)
            {
                case 0:
                    Background = Brushes.Transparent;

                    break;
                case 1:
                    Background = Brushes.LightBlue;

                    break;
                case 2:
                    Background = Brushes.Green;

                    break;
                case 3:
                    Background = Brushes.Red;

                    break;
                case 4:
                    Background = Brushes.Yellow;


                    break;
                default:
                    break;
            }
        }
    }
}
