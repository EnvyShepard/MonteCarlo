using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace ImageProcessing
{

    public class Point {
        double x;
        double y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public Point(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public double X => x;
        public double Y => y;

        private void PointTransformation( double[,] matrix) {
            List<double> sourseList = new List<double> { X, Y, 1 };
            List<double> rezList = new List<double>();

            for (int i = 0; i < 3; i++) {
                double s = 0;
                for (int j = 0; j < 3; j++) {
                    s += sourseList[j] * matrix[j, i];
                }
                rezList.Add(s);
            }

            x = rezList[0];
            y = rezList[1];
        }

        public void Move(double deltaX, double deltaY) {
            /*
            var matrix = new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { deltaX, deltaY, 1 } };
            PointTransformation(matrix);
            */

            x += deltaX;
            y += deltaY;
        }

        public void Rotate(Point center, int angle) {
            var an = Math.PI * angle / 180;
            var matrix = new double[,] { { Math.Cos(an), Math.Sin(an), 0 }, { -Math.Sin(an), Math.Cos(an), 0 }, { 0, 0, 1 } };

            Move(-center.X, -center.Y);
            PointTransformation(matrix);
            Move(center.X, center.Y);
        }

        public void SetValue(double x, double y) {
            this.x = x;
            this.y = y;
        }
        

    }

    public class Polygon : IEnumerable {

        LinkedList<Point> _pointsList;
        private static Random r = new Random();

        public Point GetCentre() {
            double x = (_pointsList.Sum(point => point.X) - _pointsList.Last.Value.X) / (_pointsList.Count - 1);
            double y = (_pointsList.Sum(point => point.Y) - _pointsList.Last.Value.Y) / (_pointsList.Count - 1);
            return new Point(x, y);

        }
        public Polygon() {
            _pointsList = new LinkedList<Point>();
        }

        public Polygon(Polygon p) {
            _pointsList = new LinkedList<Point>();
            foreach(Point point in p) {
                _pointsList.AddLast(new Point(point.X, point.Y));
            }
        }

        public int PointCount => _pointsList.Count;

        public void AddPoint(Point p) => _pointsList.AddLast(p);

        public IEnumerator GetEnumerator() {
            return ((IEnumerable)_pointsList).GetEnumerator();
        }

        public System.Drawing.Point[] GetStandartPoints() {
            var t = from Point p in _pointsList
                    select new System.Drawing.Point((int)p.X, (int)p.Y);
            return t.ToArray();
        }

        internal Point GetLastPoint() => _pointsList.Last.Value;
        internal Point GetFirstPoint() => _pointsList.First.Value;

        internal void SetPoint(int index, object value1, object value2) {
            if ( index == 0) {
                _pointsList.RemoveLast();
                _pointsList.First.Value = new Point(Convert.ToInt32(value1), Convert.ToInt32(value2));
                _pointsList.AddLast(_pointsList.First.Value);
            }

            Point p = new Point(Convert.ToInt32(value1), Convert.ToInt32(value2));
            var temp = _pointsList.ElementAt(index);
            _pointsList.Find(temp).Value = p ;
        }

        internal void setRandomPosition(int rightBorder, int bottomBorder) {

            int x = r.Next() % rightBorder;
            int y = r.Next() % bottomBorder;
            int angle = r.Next() % 360;

            MoveTo(x, y);
            Rotate(angle);
            
        }

        internal void Move(double deltaX, double deltaY) {
            _pointsList.RemoveLast();
            foreach (var p in _pointsList)
                p.Move(deltaX, deltaY);
        
            _pointsList.AddLast(_pointsList.First.Value);

        }

        internal void MoveTo(double X, double Y) {
            var deltaX = GetCentre().X - X;
            var deltaY = GetCentre().Y - Y;

            Move(-deltaX, -deltaY);
        }
        

        internal void Rotate(int angle) {
            var center = GetCentre();
            _pointsList.RemoveLast();
            foreach (var p in _pointsList)
                p.Rotate(center, angle);

            _pointsList.AddLast(_pointsList.First.Value);

        }

        internal Point[] GetPoints() {
            return _pointsList.ToArray();
        }
    }

    static class PolygonIntersection {


        public static bool IsPolygonIntersect( Polygon pol1, Polygon pol2) {
            var points1 = pol1.GetPoints();
            var points2 = pol2.GetPoints();


            // beta
            if (points1[0].X == points2[0].X && points1[0].Y == points2[0].Y)
                return true;
            //


            for ( int i = 1; i<points1.Length; i++) {
                for ( int j = 1; j<points2.Length; j++) {
                    if (IsLinesIntersect(points1[i - 1], points1[i], points2[j - 1], points2[j]))
                        return true;
                }
            }

            foreach (Point p in points1) {
                if (IsDotInsidePolygon(p, points2))
                    return true;
            }

            return false;
        }

        private static bool IsDotInsidePolygon(Point point, Point[] points) {

            int counter = 0;
            var secondPoint = new Point(0, point.Y);

            
            for ( int i = 0; i<points.Length-1; i++) 
                if (IsLinesIntersect(point, secondPoint, points[i], points[i + 1])) counter++;

            return counter % 2 == 0 ? false : true;
        }

        public static bool IsLinesIntersect(Point p1, Point p2, Point p3, Point p4) {
            SortPoints(ref p1, ref p2, ref p3, ref p4);

            if (p2.X < p3.X) return false;

            // случай, когда оба отрезка вертикальные
            if ((p1.X - p2.X == 0) && (p3.X - p4.X == 0)) return BothVertical(p1, p2, p3, p4);

            // если один из отрезков вертикальный
            if (p1.X - p2.X == 0) return OneLineVertical(p1, p2, p3, p4);
            if (p3.X - p4.X == 0) return OneLineVertical(p3, p4, p1, p2);
            return CheckNotVerticalLines(p1, p2, p3, p4);
        }

        private static bool CheckNotVerticalLines(Point p1, Point p2, Point p3, Point p4) {
            double A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
            double A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
            double b1 = p1.Y - A1 * p1.X;
            double b2 = p3.Y - A2 * p3.X;

            if (A1 == A2) {
                return false; //отрезки параллельны
            }

            //Xa - абсцисса точки пересечения двух прямых
            double Xa = (b2 - b1) / (A1 - A2);

            if ((Xa < Math.Max(p1.X, p3.X)) || (Xa > Math.Min(p2.X, p4.X))) {
                return false; //точка Xa находится вне пересечения проекций отрезков на ось X 
            }
            else {
                return true;
            }
        }

        private static bool BothVertical(Point p1, Point p2, Point p3, Point p4) {
            if (p1.X == p3.X) {
                if (!((Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y)) ||
                        (Math.Min(p1.Y, p2.Y) > Math.Max(p3.Y, p4.Y)))) {
                    return true;
                }
            }
            return false;
        }

        private static bool OneLineVertical(Point p1, Point p2, Point p3, Point p4) {
            double Xa = p1.X;
            double A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
            double b2 = p3.Y - A2 * p3.X;
            double Ya = A2 * Xa + b2;

            if (p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya &&
                    Math.Max(p1.Y, p2.Y) >= Ya) {

                return true;
            }

            return false;
        }

        private static void SortPoints(ref Point p1, ref Point p2, ref Point p3, ref Point p4) {
            if (p2.X < p1.X) {

                Point tmp = p1;
                p1 = p2;
                p2 = tmp;
            }

            if (p4.X < p3.X) {

                Point tmp = p3;
                p3 = p4;
                p4 = tmp;
            }
        }
    }
}
