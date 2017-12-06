using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing {
    public partial class NewFigureForm : Form {

        bool canInsert = true;
        private Polygon pol;
        private Point MovingPoint;
        private List<Polygon> polygons;

        public NewFigureForm(List<Polygon> polygons) {
            InitializeComponent();
            this.polygons = polygons;

        }

        private void NewFigureForm_Load(object sender, EventArgs e) {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns.Add("x", "X");
            dataGridView1.Columns.Add("y", "Y ");
        }

        private void pictureBox1_Click(object sender, EventArgs e) {

            if (!canInsert ) return;

            var btn = e as MouseEventArgs;
            if (MouseButtons.Left == btn.Button) {
                if (pol == null) pol = new Polygon();
                var temp = e as MouseEventArgs;
                pol.AddPoint(new Point(temp.X, temp.Y));
                UpdateDataGrid();
            }
            else {
                if (pol == null) return;
                pol.AddPoint(pol.GetFirstPoint());
                canInsert = false;
                UpdateDataGrid(true);
            }

            
            Presenter.Draw(pictureBox1, pol);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            if (!canInsert)
                GrabPoint(new Point(e.X, e.Y));
        }

        private void GrabPoint(Point point) {
            MovingPoint = null;
            foreach (Point p in pol) {
                if (Math.Abs(p.X - point.X) < 5 && Math.Abs(p.Y - point.Y) < 5)
                    MovingPoint = p;
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MovePoint(new Point(e.X, e.Y));
                Presenter.Draw(pictureBox1, pol);
                UpdateDataGrid(true);
            }
        }

        private void MovePoint(Point point) {
            if (MovingPoint != null) {
                MovingPoint.SetValue(point.X, point.Y); 
            }

        }

        private void UpdateDataGrid(bool removeLast = false) {

            if (pol == null) return;

            dataGridView1.Rows.Clear();
            foreach(Point p in pol) {
                dataGridView1.Rows.Add(p.X, p.Y);
            }
            if ( removeLast )
                dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);


        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) {

            var temp = dataGridView1.Rows[e.RowIndex].Cells;
            pol.SetPoint(e.RowIndex, temp[0].Value, temp[1].Value);

            Presenter.Draw(pictureBox1, pol);

            
        }

        private void saveBtn_Click(object sender, EventArgs e) {
            while ( numericUpDown1.Value > 0) {
                polygons.Add(pol);
                numericUpDown1.Value--;
            }
            Close();

        }
    }
}
