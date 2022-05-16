using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace DiagramDesigner
{
    // Represents a selectable item in the Toolbox/>.
    public class ToolboxItem : ContentControl
    {
        // caches the start point of the drag operation
        private Point? dragStartPoint = null;
        public static DragObject LatestDragObject { get; set; } = null;

        static ToolboxItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            dragStartPoint = new Point?(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            OnMouseMoveAction(e);
        }

        private void OnMouseMoveAction(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                dragStartPoint = null;

            if (dragStartPoint.HasValue)
            {
                // XamlWriter.Save() has limitations in exactly what is serialized,
                // see SDK documentation; short term solution only;
                string xamlString = XamlWriter.Save(Content);
                DragObject dataObject = new DragObject
                {
                    Xaml = xamlString
                };

                WrapPanel panel = VisualTreeHelper.GetParent(this) as WrapPanel;
                if (panel != null)
                {
                    // desired size for DesignerCanvas is the stretched Toolbox item size
                    double scale = 1.3;
                    dataObject.DesiredSize = new Size(panel.ItemWidth * scale, panel.ItemHeight * scale);
                }

                LatestDragObject = dataObject;

                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);

                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            OnMouseMoveAction(e);
        }
    }

    // Wraps info of the dragged object into a class
    public class DragObject
    {
        // Xaml string that represents the serialized content
        public string Xaml { get; set; }

        // Defines width and height of the DesignerItem
        // when this DragObject is dropped on the DesignerCanvas
        public Size? DesiredSize { get; set; }
    }
}
