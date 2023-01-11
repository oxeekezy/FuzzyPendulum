using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace fuzzyMaster.Entities
{
    internal class Function
    {
        public List<Point> function = new List<Point>();
        CompareTwoPoints compare = new CompareTwoPoints();
        public double x = 0;

        public Function(List<Point> func)
        {
            func.Sort(compare.Compare);
            function = func;
        }
        public Function()
        {
            function = new List<Point>();
        }

        public Function(string funcInString)
        {
            string[] splittedFunc = funcInString.Split(new string[] { ") (" }, 1000000, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in splittedFunc)
            {
                string[] dd = s.Replace("(", "").Replace(")", "").Split(' ');
                function.Add(new Point(Convert.ToDouble(dd[0]), Convert.ToDouble(dd[1])));
            }
        }

        public double VALUE { get => CalcIntersection(x); }

        public double CENTER { get => CalcIntegral(); }

        private double CalcIntegral()
        {
            double numerator = 0;
            double denominator = 0;
            for (int i = 0; i < function.Count - 1; i++)
            {
                double y1 = function[i].GetX();
                double y2 = function[i + 1].GetX();
                double a = (function[i].GetY() - function[i + 1].GetY()) / (function[i].GetX() - function[i + 1].GetX());
                double c = function[i].GetY() - a * function[i].GetX();

                //numerator += (a * ((Math.Pow(y1, 3) / 3) - (Math.Pow(y2, 3) / 3)) + c * ((Math.Pow(y1, 2) / 2) - (Math.Pow(y2, 2) / 2)));
                //denominator += (a * ((Math.Pow(y1, 2) / 2) - (Math.Pow(y2, 2) / 2)) + c * (y1) - (y2));

                numerator += (Math.Pow(y2, 2) - Math.Pow(y1, 2)) / 2.0;
                denominator += y2 - y1;
            }
            return numerator / denominator;
        }

        internal Function GetCopy()
        {
            return new Function(new List<Point>(function));
        }

        private double CalcIntersection(double x)
        {
            for (int i = 0; i < function.Count - 1; i++)
            {
                if (function[i].GetX() <= x && (function[i + 1].GetX() >= x))
                    return CalcSegment(function[i], function[i + 1], x);
            }
            return 0;
        }

        private double CalcSegment(Point point1, Point point2, double x)
        {
            double a = (point1.GetY() - point2.GetY()) / (point1.GetX() - point2.GetX());
            double c = point1.GetY() - a * point1.GetX();
            return a * x + c;
        }

        internal void Save(string path)
        {
            throw new NotImplementedException();
        }

        internal void SetX(double value)
        {
            x = value;
        }

        internal string GetSaveString()
        {
            return string.Join(" ", function.Select(x => $"({x.GetX()} {x.GetY()})"));
        }

        internal ListViewItem GetString()
        {
            return new ListViewItem(string.Join(" ", function.Select(x => $"({x.GetX()};{x.GetY()})")));
        }

        public void AddPaint(Point a)
        {
            function.Add(a);
            function.Sort(compare.Compare);
        }
    }
}
