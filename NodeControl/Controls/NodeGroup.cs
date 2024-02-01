using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;

namespace NodeControl.Controls
{
    public class NodeGroup : Control
    {
        static NodeGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeGroup), new FrameworkPropertyMetadata(typeof(NodeGroup)));
        }

        public NodeGroup()
        {
            NodeItems = new ObservableCollection<NodeItem>();
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(NodeGroup), new PropertyMetadata(null));

        public ObservableCollection<NodeItem> NodeItems
        {
            get { return (ObservableCollection<NodeItem>)GetValue(NodeItemsProperty); }
            set { SetValue(NodeItemsProperty, value); }
        }
        public static readonly DependencyProperty NodeItemsProperty =
            DependencyProperty.Register("NodeItems", typeof(ObservableCollection<NodeItem>), typeof(NodeGroup), new PropertyMetadata(null, OnNodeItemsChanged));

        private static void OnNodeItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeGroup = d as NodeGroup;
            nodeGroup.NodeItems.CollectionChanged += nodeGroup.NodeItems_CollectionChanged;
            nodeGroup.Refresh();
        }

        private void NodeItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (nodePanel == null)
            {
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as NodeItem;
                nodePanel.Children.Add(item);
            }
        }

        private void Refresh()
        {
            if (nodePanel == null)
            {
                return;
            }

            nodePanel.Children.Clear();
            foreach (var item in NodeItems)
            {
                nodePanel.Children.Add(item);
            }
        }

        public Point Location
        {
            get { return (Point)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(Point), typeof(NodeGroup), new PropertyMetadata(default(Point), OnLocationChanged));

        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as NodeGroup).UpdateLocate();

        private void UpdateLocate()
        {
            Canvas.SetLeft(this, Location.X);
            Canvas.SetTop(this, Location.Y);
        }

        Grid headerGrid;
        StackPanel nodePanel;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            headerGrid = GetTemplateChild("HeaderGrid") as Grid;
            nodePanel = GetTemplateChild("NodePanel") as StackPanel;
            if (headerGrid != null)
            {
                headerGrid.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
                headerGrid.MouseLeftButtonUp += HeaderGrid_MouseLeftButtonUp;
                headerGrid.MouseMove += HeaderGrid_MouseMove;
            }

            Refresh();
        }

        Point lastPoint = default;
        int zIndex = 0;
        private void HeaderGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            if (lastPoint != default)
            {
                Point nowPoint = e.GetPosition(null);
                Vector offset = nowPoint - lastPoint;
                SetCurrentValue(LocationProperty, new Point(Canvas.GetLeft(this) + offset.X, Canvas.GetTop(this) + offset.Y));

                Panel.SetZIndex(this, 99);
                headerGrid.CaptureMouse();
            }

            lastPoint = e.GetPosition(null);
            foreach (NodeItem item in NodeItems)
            {
                item.UpdatePoint();
            }
        }

        private void HeaderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            headerGrid.ReleaseMouseCapture();
            Panel.SetZIndex(this, zIndex);
        }

        private void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(null);
            zIndex = Panel.GetZIndex(this);
        }
    }
}
