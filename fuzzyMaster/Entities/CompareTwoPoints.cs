using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzyMaster.Entities
{
    class CompareTwoPoints : IComparer<Point>
    {
        public int Compare(Point a, Point b)
        {
            if (a.GetX() > b.GetX())
                return 1;
            if (Math.Abs(a.GetX() - b.GetX()) < 0.00001)
                return 0;
            return -1;
        }
    }
}
