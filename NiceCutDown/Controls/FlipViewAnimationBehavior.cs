using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;


namespace NiceCutDown.Controls
{
    public class FlipViewAnimationBehavior : DependencyObject, IBehavior
    {
        FlipView flipView;
        ScrollViewer scrollViewer;
        Compositor compositor;
        CompositionPropertySet scrollPropSet;
        ExpressionAnimation CenterPointAnimation;
        ExpressionAnimation ScaleXAnimation;
        ExpressionAnimation ScaleYAnimation;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            if (associatedObject is FlipView flip) flipView = flip;
            else throw new ArgumentNullException("FlipView为空");
            scrollViewer = Helper.FindVisualChild<ScrollViewer>(flipView, "ScrollingHost");
            if (scrollViewer == null)
            {
                flipView.Loaded += FlipView_Loaded;
            }
            else InitCompositionResources(scrollViewer);
            flipView.SelectionChanged += FlipView_SelectionChanged;
        }

        public void Detach()
        {
            if (flipView != null)
            {
                flipView.Loaded -= FlipView_Loaded;
                flipView.SelectionChanged -= FlipView_SelectionChanged;
            }
        }

        void InitCompositionResources(ScrollViewer scroll)
        {
            if (compositor == null) compositor = ElementCompositionPreview.GetElementVisual(flipView).Compositor;
            if (scroll == null) return;

            scrollPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            if (CenterPointAnimation == null)
            {
                CenterPointAnimation = compositor.CreateExpressionAnimation("Vector3(visual.Size.X/2,visual.Size.Y/2,0)");
            }
            if (ScaleXAnimation == null)
            {
                ScaleXAnimation = compositor.CreateExpressionAnimation("scroll.Translation.X % visual.Size.X < 0 ? ( -scroll.Translation.X % visual.Size.X) / visual.Size.X > 0.5 ? ( -scroll.Translation.X % visual.Size.X) / visual.Size.X : 1- ( -scroll.Translation.X % visual.Size.X) / visual.Size.X : 1");
                ScaleXAnimation.SetReferenceParameter("scroll", scrollPropSet);
            }
            if(ScaleYAnimation == null)
            {
                ScaleYAnimation = compositor.CreateExpressionAnimation("visual.Scale.X");
            }
        }

        private void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            flipView.Loaded -= FlipView_Loaded;
            var scroll = Helper.FindVisualChild<ScrollViewer>(flipView, "ScrollingHost");
            if (scroll == null) throw new ArgumentNullException("ScrollViewer为空");
            else scrollViewer = scroll;

            InitCompositionResources(scrollViewer);
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (compositor != null)
            {
                for (int i = 0; i < flipView.Items.Count; i++)
                {
                    var item = flipView.ContainerFromIndex(i);
                    if (item is UIElement ele)
                    {
                        var visual = ElementCompositionPreview.GetElementVisual(ele);
                        CenterPointAnimation.SetReferenceParameter("visual", visual);
                        visual.StartAnimation("CenterPoint", CenterPointAnimation);
                        visual.StopAnimation("Scale.X");
                        if (i < flipView.SelectedIndex + 1 && i > flipView.SelectedIndex - 1)
                        {
                            ScaleXAnimation.SetReferenceParameter("visual", visual);
                            ScaleYAnimation.SetReferenceParameter("visual", visual);
                            visual.StartAnimation("Scale.X", ScaleXAnimation);
                            visual.StartAnimation("Scale.Y", ScaleXAnimation);
                        }
                    }
                }
            }
        }

    }
}
