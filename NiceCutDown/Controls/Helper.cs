using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace NiceCutDown.Controls
{
    public static class Helper
    {
        public static T FindVisualChild<T>(DependencyObject obj, int Index = 0) where T : DependencyObject
        {
            if (Index == -1) return null;
            int count = VisualTreeHelper.GetChildrenCount(obj);
            int findedcount = 0;
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    if (findedcount == Index)
                        return (T)child;
                    else
                    {
                        findedcount++;
                    }
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child, findedcount);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        public static T FindVisualChild<T>(DependencyObject obj, string name) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(obj);
            int findedcount = 0;
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    if ((child as FrameworkElement).Name == name)
                        return (T)child;
                    else
                    {
                        findedcount++;
                    }
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child, findedcount);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
