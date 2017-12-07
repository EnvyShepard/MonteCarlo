using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace ImageProcessing {
    public static class Presenter {

        private static double minHeight = -1;

        public static void Draw(PictureBox pb, Polygon figure, bool checkIntersections = false) {
            var gr = pb.CreateGraphics();
            gr.Clear(Color.White);
            Pen pen = new Pen(Color.Black);
            DrawFigure(gr, figure, pen);
        }

        public static void Draw(PictureBox pb, List<Polygon> figures, bool checkIntersections = false) {
            var gr = pb.CreateGraphics();
            gr.Clear(Color.White);
            Pen pen = new Pen(Color.Black);

            if (figures != null) {
                foreach (var figure in figures) {
                    DrawFigure(gr, figure, pen);
                }
            }
        }

        private static void DrawFigure(Graphics gr, Polygon figure, Pen pen) {
            if (figure == null) return;

            if (figure.PointCount > 1)
                gr.DrawLines(pen, figure.GetStandartPoints());

            foreach (Point p in figure) {
                gr.DrawEllipse(new Pen(Color.Black, 3), (float)p.X - 2, (float)p.Y - 2, 4, 4);
            }
            gr.DrawEllipse(new Pen(Color.Black, 3), (float)figure.GetCentre().X - 2, (float)figure.GetCentre().Y - 2, 4, 4);
        }

        // true если пересекаются, else иначе
        public static bool CheckIntersection(List<Polygon> figures, int rightBorder, int bottomBorder) {

            if (CheckBorders(figures, rightBorder, bottomBorder)) return true;
            for (int i = 0; i < figures.Count - 1; i++)
                for (int j = i + 1; j < figures.Count; j++) 
                    if (PolygonIntersection.IsPolygonIntersect(figures[i], figures[j]))
                        return true;                
            return false;
        }

        private static bool CheckBorders(List<Polygon> figures, int rightBorder, int bottomBorder) {
            foreach (Polygon p in figures)
                foreach (Point point in p)
                    if (point.Y < 0 || point.X < 0 || point.X > rightBorder || point.Y > bottomBorder)
                        return true;
            return false;
        }

        public static double CalculateMaxHeight(List<Polygon> list) {
            double Max = 0;
            foreach( var p in list) {
                foreach (Point point in p)
                    Max = Max < point.Y ? point.Y : Max;
            }
            return Max;
        }


        public static void MonteCarloMethod(PictureBox pb, List<Polygon> polygons, int rightBorder, int bottomBorder, int iterations = -1) {

            if (minHeight == -1) minHeight = bottomBorder;

            while ( true) {
                List<Polygon> copy = new List<Polygon>();
                foreach ( Polygon p in polygons) {
                    copy.Add(new Polygon(p));
                }


                foreach( var pol in copy) {
                    pol.setRandomPosition(rightBorder, bottomBorder);
                }

                if (! CheckIntersection(copy, rightBorder, bottomBorder)) {
                    var height = CalculateMaxHeight(copy);
                    if ( minHeight > height) {
                        minHeight = height;
                        Draw(pb, copy);
                    }
                }

          
                if (iterations == 0) { break; }
                if (iterations != -1) iterations--;
            }


        }
    }
}
