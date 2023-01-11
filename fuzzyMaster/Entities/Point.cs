using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzyMaster
{
    public class Point
    {
        double _x;
        double _y;

        public Point(double x, double y) 
        {
            _x = x;
            _y = y;
        }

        public Point(string pointWithSplitter) 
        {
            _x = Convert.ToDouble(pointWithSplitter.Split(';')[0]);
            _y = Convert.ToDouble(pointWithSplitter.Split(';')[1]);
        }

        public double GetX() 
        {
            return _x;
        }

        public double GetY() 
        {
            return _y;
        }

        public string XYtoString() 
        {
            return $"{_x};{_y}";
        }
    }
}
