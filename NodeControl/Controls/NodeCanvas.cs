using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeControl.Controls
{
    public class NodeCanvas : Canvas
    {
        public NodeCanvas()
        {
            NodeGroups = new ObservableCollection<NodeGroup>();
            ConnectedDatas = new ObservableCollection<ConnectedData>();
            InternalConnects = new ObservableCollection<ConnectedData>();
            Loaded += NodeCanvas_Loaded;
        }

        public ObservableCollection<NodeGroup> NodeGroups
        {
            get { return (ObservableCollection<NodeGroup>)GetValue(NodeGroupsProperty); }
            set { SetValue(NodeGroupsProperty, value); }
        }

        public static readonly DependencyProperty NodeGroupsProperty =
            DependencyProperty.Register("NodeGroups", typeof(ObservableCollection<NodeGroup>), typeof(NodeCanvas), new PropertyMetadata(null, OnNodeGroupsChanged));

        private static void OnNodeGroupsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = d as NodeCanvas;
            if (s.NodeGroups != null)
            {
                foreach (var item in s.NodeGroups)
                {
                    s.Children.Add(item);
                }

                s.NodeGroups.CollectionChanged += s.NodeGroups_CollectionChanged;
            }
        }

        private void NodeGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var group = e.NewItems[0] as NodeGroup;
                Children.Add(group);
            }
        }

        public ObservableCollection<ConnectedData> ConnectedDatas
        {
            get { return (ObservableCollection<ConnectedData>)GetValue(ConnectedDatasProperty); }
            set { SetValue(ConnectedDatasProperty, value); }
        }
        public static readonly DependencyProperty ConnectedDatasProperty =
            DependencyProperty.Register("ConnectedDatas", typeof(ObservableCollection<ConnectedData>), typeof(NodeCanvas), new FrameworkPropertyMetadata(null, OnConnectedDatasChanged));

        private static void OnConnectedDatasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var s = d as NodeCanvas;
            s.Refresh();
        }

        /// <summary>
        /// 添加连接线集合数据时，绘制并绑定连接线数据
        /// </summary>
        private void ConnectedDatas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ConnectedData item = e.NewItems[0] as ConnectedData;
                if (CanDraw(item))
                {
                    GenConnectLine(item);
                }

                ConnectedData errData = ConnectedDatas.FirstOrDefault(x => x.Line == null);
                while (errData != null)
                {
                    ConnectedDatas.Remove(errData);
                    errData = ConnectedDatas.FirstOrDefault(x => x.Line == null);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ConnectedData item = e.OldItems[0] as ConnectedData;
                ConnectedDatas.Remove(item);
                InternalConnects.Remove(item);
                Children.Remove(item.Line);
            }
        }

        private ObservableCollection<ConnectedData> InternalConnects { get; set; }

        private void NodeCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!IsLoaded)
            {
                return;
            }

            if (ConnectedDatas != null)
            {
                foreach (var item in ConnectedDatas)
                {
                    if (CanDraw(item))
                    {
                        GenConnectLine(item);
                    }
                }

                ConnectedDatas.CollectionChanged += ConnectedDatas_CollectionChanged;
            }
        }

        public NodeItem StartNode { get; set; }
        public CanvasState State { get; set; }

        NodeConnectLine tempPreviewLine;
        public void StartOrEndDraw(NodeItem nodeItem)
        {
            // 点击第一个点开始划线
            if (State == CanvasState.Normal)
            {
                StartNode = nodeItem;
                State = CanvasState.DrawLine;
            }
            // 点击第二个点结束判断
            else
            {
                ConnectedData connected = new ConnectedData()
                {
                    StartNode = StartNode,
                    EndNode = nodeItem,
                };
                if (!CanDraw(connected))
                {
                    return;
                }

                State = CanvasState.Normal;
                // 添加数据到连接集合
                ConnectedDatas.Add(connected);
                Children.Remove(tempPreviewLine);
                tempPreviewLine = null;
            }
        }

        private void GenConnectLine(ConnectedData item)
        {
            NodeConnectLine addLine = new NodeConnectLine()
            {
                StartPoint = item.StartNode.ConnectPoint,
                EndPoint = item.EndNode.ConnectPoint,
                IsCompelete = true,
            };
            item.Line = addLine;

            item.StartNode.ApplyTemplate();
            item.EndNode.ApplyTemplate();
            item.StartNode.UpdatePoint();
            item.EndNode.UpdatePoint();
            // 绑定开始点和结束点
            Binding bindingStart = new Binding("ConnectPoint")
            {
                Source = item.StartNode
            };
            addLine.SetBinding(NodeConnectLine.StartPointProperty, bindingStart);

            Binding bindingEnd = new Binding("ConnectPoint")
            {
                Source = item.EndNode
            };
            addLine.SetBinding(NodeConnectLine.EndPointProperty, bindingEnd);

            // 绑定动画属性
            Binding bindingAnimation = new Binding("UseAnimation")
            {
                Source = item,
            };
            addLine.SetBinding(NodeConnectLine.IsAnimatedProperty, bindingAnimation);

            addLine.MouseRightButtonDown += TempPreviewLine_MouseRightButtonDown;
            SetZIndex(addLine, -1);
            Children.Add(addLine);
            InternalConnects.Add(item);
        }

        private bool CanDraw(ConnectedData connectedData)
        {
            bool canDraw = true;
            if (connectedData.StartNode == connectedData.EndNode)
            {
                canDraw = false;
                ShowMessage("不可以与自己连线！");
            }
            else if (connectedData.StartNode.NodeType == connectedData.EndNode.NodeType)
            {
                canDraw = false;
                ShowMessage("只允许输入节点与输出节点连线！");
            }
            else if (InternalConnects.FirstOrDefault(x => x.StartNode == connectedData.StartNode && x.EndNode == connectedData.EndNode) != null)
            {
                canDraw = false;
                ShowMessage("已存在相同连线！");
            }
            else if (InternalConnects.FirstOrDefault(x => x.StartNode == connectedData.EndNode && x.EndNode == connectedData.StartNode) != null)
            {
                canDraw = false;
                ShowMessage("已存在相同连线！");
            }
            else if (NodeItem.GetParent<NodeGroup>(connectedData.StartNode) == NodeItem.GetParent<NodeGroup>(connectedData.EndNode))
            {
                canDraw = false;
                ShowMessage("同一个组节点之间不允许连线！");
            }

            return canDraw;
        }

        int messCount = 0;
        async void ShowMessage(string message)
        {
            TextBlock text = new TextBlock()
            {
                Text = message,
                Foreground = Brushes.IndianRed,
            };
            SetLeft(text, 4);
            SetTop(text, 4 + messCount * 16);
            Children.Add(text);
            messCount++;
            await Task.Delay(2000);
            Children.Remove(text);
            messCount--;
        }

        /// <summary>
        /// 在连接线上右键点击，删除连接线
        /// </summary>
        private void TempPreviewLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var curLine = sender as NodeConnectLine;
            var data = ConnectedDatas.FirstOrDefault(x => x.Line == curLine);
            ConnectedDatas.Remove(data);
            InternalConnects.Remove(data);
            Children.Remove(curLine);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (State == CanvasState.Normal)
            {
                return;
            }

            if (tempPreviewLine == null)
            {
                tempPreviewLine = new NodeConnectLine
                {
                    StartPoint = StartNode.ConnectPoint
                };

                Children.Add(tempPreviewLine);
                SetZIndex(tempPreviewLine, -1);
            }

            tempPreviewLine.EndPoint = new Point(e.GetPosition(this).X - 2, e.GetPosition(this).Y - 2);
        }

        /// <summary>
        /// 取消连线操作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            State = CanvasState.Normal;
            Children.Remove(tempPreviewLine);
            tempPreviewLine = null;
        }
    }

    public enum CanvasState
    {
        Normal,
        DrawLine,
    }

    public class ConnectedData : DependencyObject
    {
        public NodeConnectLine Line { get; set; }
        public NodeItem StartNode { get; set; }
        public NodeItem EndNode { get; set; }

        public bool UseAnimation
        {
            get { return (bool)GetValue(UseAnimationProperty); }
            set { SetValue(UseAnimationProperty, value); }
        }
        public static readonly DependencyProperty UseAnimationProperty =
            DependencyProperty.Register("UseAnimation", typeof(bool), typeof(ConnectedData), new PropertyMetadata(false));
    }
}
