using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {

        Dictionary<string, Polygon> polygons;

        public Form1()
        {
            InitializeComponent();
            polygons = new Dictionary<string, Polygon>();
        }

        private void Form1_Load(object sender, EventArgs e) {
           
        }

        private void btnNewFigure_Click(object sender, EventArgs e) {


            var newFigureForm = new NewFigureForm(polygons) {
                Visible = true
            };
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
           // throw new NotImplementedException();
            //var m = e as MouseEventArgs;

            //if ( m.Button == MouseButtons.Left) {
            //    if (_isCreatingFigure) presenter.AddFigurePoint(new Point(m.X, m.Y));
               
            //}
            //else if ( m.Button == MouseButtons.Right) {
            //    if (_isCreatingFigure) {
            //        presenter.AddLastFigurePoint();
            //        _isCreatingFigure = false;
            //    }
                
            //}
        }

        private void btnTrajectory_Click(object sender, EventArgs e) {
           

            
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
          
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
           
            //if (e.Button == MouseButtons.Left) {
            //    presenter.MovePoint(new Point(e.X, e.Y));
            //}
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
          
            //presenter.EndMoving();
        }

        

        private void BtnStart_Click(object sender, EventArgs e) {
            timer1.Start();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            timer1.Stop();
        }

        private void DrawBtn_Click(object sender, EventArgs e) {
            Graphics g = pictureBox1.CreateGraphics();
            
            Presenter.MonteCarloMethod(pictureBox1, polygons.Values.ToList(), (int)numericUpDown1.Value,(int) numericUpDown2.Value);
            g.DrawLine(new Pen(Color.Black), (int)numericUpDown1.Value, 0, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
            g.DrawLine(new Pen(Color.Black), 0, (int)numericUpDown2.Value ,(int)numericUpDown1.Value, (int)numericUpDown2.Value);
           
        }
    }
}
