using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    public class NodeConnectLine : Control
    {
        static NodeConnectLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeConnectLine), new FrameworkPropertyMetadata(typeof(NodeConnectLine)));
        }

        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(NodeConnectLine), new PropertyMetadata(default(Point), OnPointChanged));

        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(NodeConnectLine), new PropertyMetadata(default(Point), OnPointChanged));

        private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as NodeConnectLine)?.Refresh();

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Geometry), typeof(NodeConnectLine), new PropertyMetadata(default));

        public bool IsCompelete
        {
            get { return (bool)GetValue(IsCompeleteProperty); }
            set { SetValue(IsCompeleteProperty, value); }
        }

        public static readonly DependencyProperty IsCompeleteProperty =
            DependencyProperty.Register("IsCompelete", typeof(bool), typeof(NodeConnectLine), new PropertyMetadata(false));

        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register("IsAnimated", typeof(bool), typeof(NodeConnectLine), new PropertyMetadata(false));


        private void Refresh()
        {
            PathGeometry geo = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = StartPoint;
            PathSegmentCollection pac = new PathSegmentCollection();

            BezierSegment quadratic = new BezierSegment(CalculateControlPoint(StartPoint, EndPoint, true), CalculateControlPoint(StartPoint, EndPoint, false), EndPoint, true);
            pac.Add(quadratic);
            pathFigure.Segments = pac;
            geo.Figures.Add(pathFigure);

            Data = geo;
        }

        private Point CalculateControlPoint(Point startPoint, Point endPoint, bool isFirstControlPoint)
        {
            double distanceX = Math.Abs(endPoint.X - startPoint.X);
            double controlPointX = (startPoint.X + endPoint.X) / 2;
            double controlPointY = isFirstControlPoint ? startPoint.Y - distanceX / 12 : endPoint.Y + distanceX / 12;
            return new Point(controlPointX, controlPointY);
        }
    }
}
