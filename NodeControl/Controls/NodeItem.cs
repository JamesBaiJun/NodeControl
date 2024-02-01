using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NodeControl.Controls
{
    public class NodeItem : Control
    {
        static NodeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeItem), new FrameworkPropertyMetadata(typeof(NodeItem)));
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(NodeItem), new PropertyMetadata(null));

        public event RoutedEventHandler ConnectorPressed
        {
            add { AddHandler(ConnectorPressedEvent, value); }
            remove { RemoveHandler(ConnectorPressedEvent, value); }
        }
        public static readonly RoutedEvent ConnectorPressedEvent =
            EventManager.RegisterRoutedEvent("ConnectorPressedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NodeItem));

        Shape leftConnector;
        Shape rightConnector;
        NodeCanvas rootCanvas;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            leftConnector = GetTemplateChild("LeftConnector") as Shape;
            rightConnector = GetTemplateChild("RightConnector") as Shape;
            leftConnector.MouseLeftButtonDown += LeftConnector_MouseLeftButtonDown;
            rightConnector.MouseLeftButtonDown += RightConnector_MouseLeftButtonDown;
            rootCanvas = GetParent<NodeCanvas>(this);
        }

        public void UpdatePoint()
        {
            if (NodeType == NodeType.Input)
            {
                UpdateLeftPoint();
            }
            else
            {
                UpdateRightPoint();
            }
        }

        private void UpdateLeftPoint()
        {
            var point = leftConnector.TranslatePoint(new Point(), rootCanvas);
            ConnectPoint = new Point(point.X + 6, point.Y + 6);
        }

        private void UpdateRightPoint()
        {
            var point = rightConnector.TranslatePoint(new Point(), rootCanvas);
            ConnectPoint = new Point(point.X + 6, point.Y + 6);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        private void RightConnector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //RoutedEventArgs routedEvent = new RoutedEventArgs(ConnectorPressedEvent);
            //RaiseEvent(routedEvent);
            UpdateRightPoint();
            rootCanvas.StartOrEndDraw(this);
        }

        private void LeftConnector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateLeftPoint();
            rootCanvas.StartOrEndDraw(this);
        }

        public NodeType NodeType
        {
            get => (NodeType)GetValue(NodeTypeProperty);
            set => SetValue(NodeTypeProperty, value);
        }

        public static readonly DependencyProperty NodeTypeProperty =
            DependencyProperty.Register("NodeType", typeof(NodeType), typeof(NodeItem), new PropertyMetadata(NodeType.Input));

        public Point ConnectPoint
        {
            get { return (Point)GetValue(ConnectPointProperty); }
            set { SetValue(ConnectPointProperty, value); }
        }

        public static readonly DependencyProperty ConnectPointProperty =
            DependencyProperty.Register("ConnectPoint", typeof(Point), typeof(NodeItem), new PropertyMetadata(default));


        public static T GetParent<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }

    public enum NodeType
    {
        Input,
        Output,
    }
}
