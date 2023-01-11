using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace fuzzyMaster
{
    public class ChartCore
    {
        private Chart someChart;
        private Series lastSeries;
        public ChartCore(Chart chart, double maxRangeX = 10, double maxRangeY = 10, bool enableGrid = false, bool enableHorizontal = true, bool positiveY = false)
        {
            this.someChart = chart;

            SetCustomChartOptions(maxRangeX, maxRangeY, enableGrid, positiveY);

            if (enableHorizontal)
                SetHorizontal(new Point(-maxRangeX, 0), new Point(maxRangeX, 0));
        }

        public ChartCore(Chart chart)
        {
            this.someChart = chart;
        }

        public ChartCore AddSeries(string name, Color color, int lineWidth = 3, SeriesChartType type = SeriesChartType.Line)
        {
            DeleteSeries(name);

            someChart.Series.Add(name);
            someChart.Series[name].ChartType = type;
            someChart.Series[name].BorderWidth = lineWidth;
            someChart.Series[name].Color = color;

            lastSeries = someChart.Series[name];

            return this;
        }

        public ChartCore AddPointsToSeries(string name, List<Point> points)
        {
            AddPoints(someChart.Series[name], points);
            return this;
        }

        public ChartCore AddPointsToSeries(List<Point> points)
        {
            AddPoints(lastSeries, points);
            return this;
        }

        public void DeleteSeries(string name)
        {
            if (someChart.Series.FindByName(name) != null)
                someChart.Series.Remove(someChart.Series.FindByName(name));
        }

        public int GetSeriesCount() 
        {
            return someChart.Series.Count;
        }

        public void ChangeXRange(int range) 
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Minimum = -range;
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Maximum = range;
        }

        public void ChangeXRangeLeft(int range)
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Minimum = range;
        }

        public void ChangeXRangeRight(int range)
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Maximum = range;
        }

        public void ChangeYUpper(int value) 
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.Maximum = value;
        }

        public void ChangeYLower(int value) 
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.Minimum = value;
        }

        private void AddPoints(Series series, List<Point> points)
        {
            foreach (var point in points)
            {
                series.Points.AddXY(point.GetX(), point.GetY());
            }
        }

        private void SetHorizontal(Point left, Point right)
        {
            List<Point> horizontal = new List<Point>() { left, right };
            AddSeries("horizontal", Color.Black, lineWidth: 1).AddPointsToSeries(horizontal);
        }

        private void SetCustomChartOptions(double maxRangeX, double maxRangeY, bool enableGrid, bool positiveY)
        {
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Minimum = -maxRangeX;
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.Maximum = maxRangeX;

            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.Minimum = -maxRangeY;
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.Maximum = maxRangeY;

            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisX.MajorGrid.Enabled = enableGrid;
            someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.MajorGrid.Enabled = enableGrid;

            if(positiveY)
                someChart.ChartAreas[someChart.ChartAreas.Count - 1].AxisY.Minimum = 0;

        }
    }
}
