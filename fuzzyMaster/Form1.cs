using fuzzyMaster.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace fuzzyMaster
{
    public partial class Form1 : Form
    {
        ChartCore inputs;
        ChartCore outputs;
        ChartCore pendulum;

        FuzzyCore fuzzyRotate = new FuzzyCore();
        FuzzyCore fuzzyStable = new FuzzyCore();

        Timer timer = new Timer();

        double currentAngle = 0;
        double currentCycle = 0;
        
        bool direction = false;
        bool doThis = false;   
        bool resize = false;

        eSituation situation = eSituation.sway;


        public Form1()
        {
            InitializeComponent();
            inputs = new ChartCore(this.inputChart, enableHorizontal: false, maxRangeY:1, maxRangeX:(int)inputRangeX.Value, positiveY: true);
            outputs = new ChartCore(this.outputChart, enableHorizontal: false, maxRangeY:1, maxRangeX: (int)outputRangeX.Value, positiveY: true);
            pendulum = new ChartCore(this.bob);

            
            timer.Tick += Timer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            upLabel.Visible = false;
            upLimit.Visible = false;
            lowLabel.Visible = false;
            lowLimit.Visible = false;
            leftLabel.Visible = false;
            rightLabel.Visible = false;
            numericUpDown1.Visible = false;
            numericUpDown2.Visible = false; 

            upLimit.Maximum = 1000;
            upLimit.Minimum = -1000;

            lowLimit.Maximum = 1000;
            lowLimit.Minimum = -1000;

            upLimit.Value = 200;
            lowLimit.Value = -200;

            inputRangeX.Minimum = 10;
            inputRangeX.Maximum = 1000;
            inputRangeX.Value = 200;

            tickUpDown.Minimum = 100;
            tickUpDown.Maximum = 2000;
            tickUpDown.Value = 300;

            outputRangeX.Minimum = 10;
            outputRangeX.Maximum = 1000;
            outputRangeX.Value = 50;

            filesList.Items.Add("1.txt");
            filesList.Items.Add("2.txt");

            filesList.SelectedIndex = 0;
            debugBox.EnableAutoDragDrop = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentAngle < -180) 
                currentAngle = 360 + currentAngle;
            
            if (currentAngle > 180)
                currentAngle = -360 + currentAngle;
            

            if (situation == eSituation.sway && ((140 < currentAngle && currentAngle > 0) || (currentAngle < -140 && currentAngle < 0)))
            {
                situation = eSituation.stabilization;
                currentAngle = currentAngle > 0 ? 180 - currentAngle : currentAngle + 180;
            }

            double oldAngle = currentAngle;

            if (situation == eSituation.sway)
                SwayAlgo(oldAngle);
            
            if (situation == eSituation.stabilization)
                StableAlgo(oldAngle);
            
            currentCycle += 0.2;
            direction = !direction;
            //Debug.WriteLine($"{currentAngle} {situation}");

            if(currentDebug.Checked)
                debugBox.Text = $"Current angle: {Math.Round(currentAngle,3)}\t Situation: {situation}\n";
            else
                debugBox.Text += $"Current angle: {Math.Round(currentAngle, 3)}\t Situation: {situation}\n";
        }

        public void SwayAlgo(double oldAngle) 
        {
            fuzzyRotate.SetXA(currentAngle);
            double speed = fuzzyRotate.GetResult(eVariants.v1);
            currentAngle -= speed / 10.0;
            currentAngle = direction ? currentAngle + 2.0 * oldAngle : currentAngle - 2.0 * oldAngle;
            pendulum.AddPointsToSeries("Pendulum-Range", new List<Point> { new Point(currentCycle, currentAngle) });
        }

        public void StableAlgo(double oldAngle) 
        {
            fuzzyStable.SetXA(currentAngle);
            double speed = fuzzyStable.GetResult(eVariants.v1); 
            currentAngle -= speed * 1.3;
            currentAngle = direction ? currentAngle - oldAngle / 5.0 : currentAngle + oldAngle / 5.0;
            pendulum.AddPointsToSeries("Pendulum-Range", new List<Point> { new Point(currentCycle, currentAngle > 0 ? 180 - currentAngle : -currentAngle + 180) });
        }
        public void ABRFromFile(string path) 
        {
            FuzzyCore fuzzy = new FuzzyCore();
            fuzzy.ReadFromFile(path);

            var A = fuzzy.GetAllA();
            var B = fuzzy.GetAllB();

            int n = 1;

            foreach (var a in A)
            {
                inputs.AddSeries($"input-{n}", Color.Red).AddPointsToSeries(a.function);
                n++;
            }

            n = 1;
            foreach (var b in B)
            {
                outputs.AddSeries($"output-{n}", Color.Black).AddPointsToSeries(b.function);
                n++;
            }

            matrixGrid.Rows.Clear();
            matrixGrid.Columns.Clear();

            int rows = fuzzy.R.GetUpperBound(0) + 1;
            int cols = fuzzy.R.Length / rows;

            for (int i = 0; i < cols; i++) 
            {
                matrixGrid.Columns.Add($"{i}", " ");
                matrixGrid.Columns[i].Width = 30;
            }

            for (int i = 0; i < rows; i++) 
            {
                matrixGrid.Rows.Add();

                for (int j = 0; j < cols; j++) 
                {
                    matrixGrid[j,i].Value = fuzzy.R[i,j];
                }
            }
            
        }

        private void startSim_Click(object sender, EventArgs e)
        {
            if (!doThis)
            {
                timer.Interval = (int)tickUpDown.Value;

                pendulum.AddSeries("Pendulum-Range", Color.Green, type: SeriesChartType.Spline);
                pendulum.AddSeries("HORIZONTAL 180", Color.Black).AddPointsToSeries(new List<Point> {new Point(0,180), new Point(10000, 180) });
                pendulum.ChangeXRangeRight(10);

                fuzzyRotate.ReadFromFile("1.txt");
                fuzzyStable.ReadFromFile("2.txt");
                timer.Start();
                doThis = true;
            }
            else 
            {
                timer.Stop();
                doThis = false;

                fuzzyRotate = new FuzzyCore();
                fuzzyStable = new FuzzyCore();

                currentAngle = 0;
                currentCycle = 0;
            }
                
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void bob_Click(object sender, EventArgs e)
        {

        }

        private void bob_DoubleClick(object sender, EventArgs e)
        {
            if (!resize)
            {
                groupBox4.Location = new System.Drawing.Point(12, 12);
                groupBox4.Width = 1083;
                groupBox4.Height = 599;
                resize = true;

                upLabel.Visible = true;
                upLimit.Visible = true; 

                lowLabel.Visible = true;    
                lowLimit.Visible = true;

                leftLabel.Visible = true;
                rightLabel.Visible = true;
                numericUpDown1.Visible = true;
                numericUpDown2.Visible = true;
            }
            else 
            {
                groupBox4.Location = new System.Drawing.Point(639, 306);
                groupBox4.Width = 456;
                groupBox4.Height = 296;
                resize = false;

                upLabel.Visible = false;
                upLimit.Visible = false;

                lowLabel.Visible = false;
                lowLimit.Visible = false;

                leftLabel.Visible = false;
                rightLabel.Visible = false;
                numericUpDown1.Visible = false;
                numericUpDown2.Visible = false;
            }
        }

        private void filesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ABRFromFile(filesList.SelectedItem.ToString());
        }

        private void inputRangeX_ValueChanged(object sender, EventArgs e)
        {
            inputs.ChangeXRange((int)inputRangeX.Value);
        }

        private void outputRangeX_ValueChanged(object sender, EventArgs e)
        {
            outputs.ChangeXRange((int)outputRangeX.Value);
        }

        private void upLimit_ValueChanged(object sender, EventArgs e)
        {
            try 
            {
                pendulum.ChangeYUpper((int)upLimit.Value);
            }
            catch 
            {
                MessageBox.Show("ACHACHA!!!");
                pendulum.ChangeYUpper(200);
            }
            
        }

        private void lowLimit_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                pendulum.ChangeYLower((int)lowLimit.Value);
            }
            catch 
            {
                MessageBox.Show("ACHACHA!!!");
                pendulum.ChangeYLower(-200);
            }
            
        }

        private void inputChart_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pendulum.ChangeXRangeLeft((int)numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            pendulum.ChangeXRangeRight((int)numericUpDown2.Value);
        }
    }
}
