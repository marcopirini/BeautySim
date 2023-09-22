using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BeautySim2023
{
    public static class ExtensionMethods
    {
        public static void Copy<T>(this IList<T> list)
        {
            Random rng = new Random(Guid.NewGuid().GetHashCode());
            int n = list.Count;
            while (n > 1) //OK
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random(Guid.NewGuid().GetHashCode());
            int n = list.Count;
            while (n > 1) //OK
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
