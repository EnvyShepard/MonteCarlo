using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.IO;

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

        public static void Positioning(PictureBox pb, List<Polygon> polygons, int rightBorder, int bottomBorder) {
            

            PolCounter it = new PolCounter(polygons, rightBorder, bottomBorder, polygons.Count);

            it.def();
            while ( it.inc()) {
                
                var t = it.SetPolygons();
                
                if (!CheckIntersection(t, rightBorder, bottomBorder)) {
                    Draw(pb, t);
                }
            }


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
        
        public static void SaveToFile(String filePath, Dictionary<String, Polygon> polygon){


            var file = File.Create(filePath);
            file.Close();
            StreamWriter sw = File.CreateText(filePath);
            foreach (var val in polygon)
            {
                sw.Write(val.Key);
                var tmp = val.Value.GetStandartPoints();
                foreach (var p in tmp)
                {
                    sw.Write(",");
                    sw.Write(p.X);
                    sw.Write(":");
                    sw.Write(p.Y);
                }
                sw.WriteLine();
            }
            sw.Close();
        }
    }

    class PolCounter {

        List<Polygon> list;

        int[] limits;

        public int[] vector;

        private int count;

        public PolCounter(List<Polygon> list, int maxX, int maxY, int count) {
            this.list = list;
            limits = new int[3];
            limits[0] = maxX;
            limits[1] = maxY;
            limits[2] = 360;
            this.count = count;
            vector = new int[count * 3];
        }

        public void def() {
            vector = vector.Select(x => x = 0).ToArray();
        }

        public bool inc() {

            for (int i = 0; i < vector.Length; i += 3) {
                for (int j = 0; j < 2; j++) {
                    if (vector[i + j] < limits[j]) {
                        vector[i + j]++;                       
                        return true;
                    }
                    else {
                        vector[i + j] = 0; 
                    }
                }
            }
            return false;
        }

        // Эта штука еле работает. Первый вариант
        
        public List<Polygon> SetPolygons() {
           
            for(int i = 0; i < count; i++) {
                list[i].MoveTo(vector[i * 3], vector[i * 3 + 1]);
            }
            return list;    
            
        }
        

        public int[] GetPolygonsVector() {
            return vector;

        }
    }
}
