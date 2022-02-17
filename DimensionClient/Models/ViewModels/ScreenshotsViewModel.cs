using DimensionClient.Common;
using System.Windows;

namespace DimensionClient.Models.ViewModels
{
    public class ScreenshotsViewModel : ModelBase
    {
        private Point pointStart;
        private Point point1;
        private Point point2;
        private Point point3;
        private Point point4;

        public Point PointStart
        {
            get => pointStart;
            set
            {
                pointStart = value;
                OnPropertyChanged(nameof(PointStart));
            }
        }
        public Point Point1
        {
            get => point1;
            set
            {
                point1 = value;
                OnPropertyChanged(nameof(Point1));
            }
        }
        public Point Point2
        {
            get => point2;
            set
            {
                point2 = value;
                OnPropertyChanged(nameof(Point2));
            }
        }
        public Point Point3
        {
            get => point3;
            set
            {
                point3 = value;
                OnPropertyChanged(nameof(Point3));
            }
        }
        public Point Point4
        {
            get => point4;
            set
            {
                point4 = value;
                OnPropertyChanged(nameof(Point4));
            }
        }

        public override void InitializeVariable()
        {
            pointStart = new Point();
            point1 = new Point();
            point2 = new Point();
            point3 = new Point();
            point4 = new Point();
        }

        public Point MaxPoint()
        {
            Point point = new();
            List<Point> points = new()
            {
                pointStart,
                point1,
                point2,
                point3,
                point4
            };
            foreach (Point item in points)
            {
                if (point.X < item.X)
                {
                    point.X = item.X;
                }
                if (point.Y < item.Y)
                {
                    point.Y = item.Y;
                }
            }
            return point;
        }

        public Point MinPoint()
        {
            Point point = new(pointStart.X, pointStart.Y);
            List<Point> points = new()
            {
                pointStart,
                point1,
                point2,
                point3,
                point4
            };
            foreach (Point item in points)
            {
                if (point.X > item.X)
                {
                    point.X = item.X;
                }
                if (point.Y > item.Y)
                {
                    point.Y = item.Y;
                }
            }
            return point;
        }
    }
}