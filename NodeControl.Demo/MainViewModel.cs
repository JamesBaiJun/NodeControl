using NodeControl.Demo.Common;
using NodeControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeControl.Demo
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitData();

            TestAddCommand = new NormalCommand() { ExecuteMethod = TestAdd };
            TestRemoveCommand = new NormalCommand() { ExecuteMethod = TestRemove };
            AnimateSetCommand = new NormalCommand<bool>() { ExecuteMethod = AnimateSet };
        }

        private void InitData()
        {
            NodeGroups = new ObservableCollection<NodeGroup>();
            ConnectedDatas = new ObservableCollection<ConnectedData>();

            group1 = new NodeGroup()
            {
                Location = new Point(50, 80),
                Header = "测试组",
            };
            for (int i = 0; i < 15; i++)
            {
                NodeItem item = new NodeItem()
                {
                    Content = "node" + i,
                    NodeType = i % 2 == 0 ? NodeType.Input : NodeType.Output,
                };
                group1.NodeItems.Add(item);
            }
            NodeGroups.Add(group1);

            group2 = new NodeGroup()
            {
                Header = "测试组",
                Location = new Point(350, 80),
            };
            for (int i = 0; i < 15; i++)
            {
                NodeItem item = new NodeItem()
                {
                    Content = "node" + i,
                    NodeType = i % 2 == 0 ? NodeType.Input : NodeType.Output,
                };
                group2.NodeItems.Add(item);
            }
            NodeGroups.Add(group2);

            for (int i = 1; i < 15; i+=2)
            {
                for (int j = 0; j < 7; j++)
                {
                    ConnectedDatas.Add(new ConnectedData()
                    {
                        StartNode = group1.NodeItems[i],
                        EndNode = group2.NodeItems[2*j]
                    });
                }
            }
        }

        public ObservableCollection<NodeGroup> NodeGroups { get; set; }
        public ObservableCollection<ConnectedData> ConnectedDatas { get; set; }

        NodeGroup group1;
        NodeGroup group2;

        public NormalCommand TestAddCommand { get; set; }
        public NormalCommand TestRemoveCommand { get; set; }
        public NormalCommand<bool> AnimateSetCommand { get; set; }

        private bool isAnimated;

        public bool IsAnimated
        {
            get { return isAnimated; }
            set { isAnimated = value; RaisePropertyChanged(); }
        }


        public void TestAdd()
        {
            ConnectedDatas.Add(new ConnectedData()
            {
                StartNode = group1.NodeItems[1],
                EndNode = group2.NodeItems[2],
                UseAnimation = IsAnimated,
            });

            ConnectedDatas.Add(new ConnectedData()
            {
                StartNode = group1.NodeItems[3],
                EndNode = group2.NodeItems[4],
                UseAnimation = IsAnimated,
            });
        }


        private void TestRemove()
        {
            while (ConnectedDatas.Count > 0)
            {
                ConnectedDatas.RemoveAt(0);
            }
        }

        private void AnimateSet(bool onOff)
        {
            foreach (var item in ConnectedDatas)
            {
                item.UseAnimation = onOff;
            }
        }
    }
}
