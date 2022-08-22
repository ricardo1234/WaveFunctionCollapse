using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WaveFunctionCollapseLib;

namespace WaveFunctionCollapse.imageItem
{
    /// <summary>
    /// Visitor implementation
    /// </summary>
    public class ItemVisitor : IVisitor<ImageItem>
    {
        private Grid field;

        public ItemVisitor(Grid field)
        {
            this.field = field;
        }

        /// <summary>
        /// Before start visiting event implementation
        /// </summary>
        public void BeforeVisit()
        {
            field.Children.Clear();
        }

        /// <summary>
        /// Visited event implementation
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="possibilities"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fastWay"></param>
        public void Visit(ImageItem? visitor, int? possibilities, long x, long y, bool fastWay)
        {
            ClearPlace(x, y);
            if (fastWay && visitor is null) return;

            if (visitor is null)
            {
                var label = new Label
                {
                    Content = possibilities
                };


                Grid.SetRow(label, Convert.ToInt32(x));
                Grid.SetColumn(label, Convert.ToInt32(y));

                field.Children.Add(label);

                AllowUIToUpdate();
                return;
            }

            var img = new Image
            {
                Width = visitor.Width,
                Height = visitor.Height,
                Source = visitor.Image
            };

            if (visitor.Rotation > 0)
            {
                img.RenderTransform = new RotateTransform(Convert.ToDouble(visitor.Rotation), 0, 0);
                img.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            }

            Grid.SetRow(img, Convert.ToInt32(x));
            Grid.SetColumn(img, Convert.ToInt32(y));

            field.Children.Add(img);

            AllowUIToUpdate();
        }

        /// <summary>
        /// Clear childreen of grid in x/y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ClearPlace(long x, long y)
        {
            var eList = from UIElement child in field.Children
                        where Grid.GetRow(child) == x && Grid.GetColumn(child) == y
                        select child;
            
            if (eList is null) return;
            if ((eList?.Count() ?? 0) == 0) return;
            
            field.Children.Remove(eList?.FirstOrDefault());
        }

        /// <summary>
        /// Update the UI in realtime
        /// </summary>
        void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            //EDIT:
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
    }
}
