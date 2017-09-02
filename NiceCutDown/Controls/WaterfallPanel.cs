

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NiceCutDown.Controls
{
    public class WaterfallPanel : Panel

    {

        public int NumbersOfColumnsOrRows
        {

            get { return (int)GetValue(NumbersOfColumnsOrRowsProperty); }

            set { SetValue(NumbersOfColumnsOrRowsProperty, value); }

        }



        // Using a DependencyProperty as the backing store for NumbersOfColumnsOrRows. This enables animation, styling, binding, etc...

        public static readonly DependencyProperty NumbersOfColumnsOrRowsProperty =

        DependencyProperty.Register("NumbersOfColumnsOrRows", typeof(int), typeof(WaterfallPanel), new PropertyMetadata(2));







        public Orientation WaterfallOrientation

        {

            get { return (Orientation)GetValue(WaterfallOrientationProperty); }

            set { SetValue(WaterfallOrientationProperty, value); }

        }



        // Using a DependencyProperty as the backing store for WaterfallOrientation. This enables animation, styling, binding, etc...

        public static readonly DependencyProperty WaterfallOrientationProperty =

        DependencyProperty.Register("WaterfallOrientation", typeof(Orientation), typeof(WaterfallPanel), new PropertyMetadata(Orientation.Vertical));



        protected override Size MeasureOverride(Size availableSize)

        {

            if (NumbersOfColumnsOrRows < 1)

            {

                throw (new ArgumentOutOfRangeException("NumberOfColumnsOrRows", "NumberOfColumnsOrRows must >0"));//太窄

            }

            var LenList = new List<double>();

            for (int i = 0; i < NumbersOfColumnsOrRows; i++)

            {

                LenList.Add(0);

            }



            if (WaterfallOrientation == Orientation.Vertical)

            {

                double maxWidth = availableSize.Width / NumbersOfColumnsOrRows;

                Size maxSize = new Size(maxWidth, double.PositiveInfinity);

                foreach (var item in Children)

                {
                    item.Measure(maxSize);

                    var itemHeight = item.DesiredSize.Height;

                    var minLen = LenList[0];

                    int minP = 0;

                    for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                    {

                        if (LenList[i] < minLen)

                        {

                            minLen = LenList[i];

                            minP = i;

                        }

                    }

                    LenList[minP] += itemHeight;

                }

                var maxLen = LenList[0];

                int maxP = 0;

                for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                {

                    if (LenList[i] > maxLen)

                    {

                        maxLen = LenList[i];

                        maxP = i;

                    }

                }

                return new Size(availableSize.Width, LenList[maxP]);

            }

            else

            {

                double maxHeight = availableSize.Height / NumbersOfColumnsOrRows;

                Size maxSize = new Size(double.PositiveInfinity, maxHeight);

                foreach (var item in Children)

                {

                    item.Measure(maxSize);

                    var itemWidth = item.DesiredSize.Width;

                    var minLen = LenList[0];

                    int minP = 0;

                    for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                    {

                        if (LenList[i] < minLen)

                        {

                            minLen = LenList[i];

                            minP = i;

                        }

                    }

                    LenList[minP] += itemWidth;

                }

                var maxLen = LenList[0];

                int maxP = 0;

                for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                {

                    if (LenList[i] > maxLen)

                    {

                        maxLen = LenList[i];

                        maxP = i;

                    }

                }

                return new Size(LenList[maxP], availableSize.Height);

            }

        }
        protected override Size ArrangeOverride(Size finalSize)

        {

            if (NumbersOfColumnsOrRows < 1)

            {

                throw (new ArgumentOutOfRangeException("NumberOfColumnsOrRows", "NumberOfColumnsOrRows must >0"));//太窄

            }

            var LenList = new List<double>();

            var posXorYList = new List<double>();

            if (WaterfallOrientation == Orientation.Vertical)

            {

                double maxWidth = finalSize.Width / NumbersOfColumnsOrRows;

                //列的长度和左上角的x值

                for (int i = 0; i < NumbersOfColumnsOrRows; i++)

                {

                    LenList.Add(0);

                    posXorYList.Add(i * maxWidth);

                }

                foreach (var item in Children)

                {

                    var itemHeight = item.DesiredSize.Height;

                    var minLen = LenList[0];

                    int minP = 0;

                    for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                    {

                        if (LenList[i] < minLen)

                        {

                            minLen = LenList[i];

                            minP = i;

                        }

                    }

                    item.Arrange(new Rect(posXorYList[minP], LenList[minP], item.DesiredSize.Width, item.DesiredSize.Height));

                    LenList[minP] += item.DesiredSize.Height;

                }

                var maxLen = LenList[0];

                int maxP = 0;

                for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                {

                    if (LenList[i] > maxLen)

                    {

                        maxLen = LenList[i];

                        maxP = i;

                    }

                }

                return new Size(finalSize.Width, LenList[maxP]);

            }

            else

            {

                double maxHeight = finalSize.Height / NumbersOfColumnsOrRows;

                //行的长度和左上角的y值

                for (int i = 0; i < NumbersOfColumnsOrRows; i++)

                {

                    LenList.Add(0);

                    posXorYList.Add(i * maxHeight);

                }

                foreach (var item in Children)

                {

                    var itemWidth = item.DesiredSize.Width;

                    var minLen = LenList[0];

                    int minP = 0;

                    for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                    {

                        if (LenList[i] < minLen)

                        {

                            minLen = LenList[i];

                            minP = i;

                        }

                    }

                    item.Arrange(new Rect(LenList[minP], posXorYList[minP], item.DesiredSize.Width, item.DesiredSize.Height));

                    LenList[minP] += item.DesiredSize.Width;

                }

                var maxLen = LenList[0];

                int maxP = 0;

                for (int i = 1; i < NumbersOfColumnsOrRows; i++)

                {

                    if (LenList[i] > maxLen)

                    {

                        maxLen = LenList[i];

                        maxP = i;

                    }

                }

                return new Size(LenList[maxP], finalSize.Height);

            }



        }
    }
}