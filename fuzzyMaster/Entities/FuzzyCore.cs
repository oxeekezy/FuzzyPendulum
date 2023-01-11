using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fuzzyMaster.Entities
{
    internal class FuzzyCore
    {
        public double[,] R = new double[0, 0];

        FuzzySet A;
        FuzzySet B;

        public double y = 0;
        eVariants currentMathType = eVariants.v1;

        public void AddNewA(List<Point> points)
        {
            A.AddFunk(points);
            UpdateR();
        }
        public void AddNewA(Function function)
        {
            A.AddFunk(function);
            UpdateR();
        }

        internal object[] GetVariants()
        {
            return Enum.GetNames(eVariants.v1.GetType());
        }

        public void AddNewB(List<Point> points)
        {
            B.AddFunk(points);
            UpdateR();
        }
        public void AddNewB(Function function)
        {
            B.AddFunk(function);
            UpdateR();
        }

        public Function GetA(int index)
        {
            return A.GetFunk(index);
        }
        public Function GetB(int index)
        {
            return B.GetFunk(index);
        }

        public List<Function> GetAllA() 
        {
            return A.GetFunks();
        }

        public List<Function> GetAllB()
        {
            return B.GetFunks();
        }

        private void UpdateR()
        {
            if (A.COUNT >= 1 && B.COUNT >= 1)
            {
                double[,] r = R;
                double[,] rez = new double[A.COUNT, B.COUNT];
                for (int i = 0; i < r.GetLength(0); i++)
                    for (int j = 0; j < r.GetLength(1); j++)
                    {
                        rez[i, j] = r[i, j];
                    }
                R = rez;
            }
        }

        internal ListViewItem[] GetItemsA()
        {
            var rez = new List<ListViewItem>();
            foreach (var f in A.functions)
            {
                rez.Add(f.GetString());
            }
            return rez.ToArray();
        }
        internal ListViewItem[] GetItemsB()
        {
            var rez = new List<ListViewItem>();
            foreach (var f in B.functions)
            {
                rez.Add(f.GetString());
            }
            return rez.ToArray();
        }

        internal void ChangeR(int i, int j, double value)
        {
            R[i, j] = value;
        }

        private List<double> GetFuncValues()
        {
            return A.functions.Select(x => x.VALUE).ToList();

        }

        public void SetXA(double value)
        {
            A.SetX(value);
        }


        double CalcT(double a, double b)
        {
            switch (currentMathType)
            {
                case eVariants.v1:
                    return Math.Min(a, b);
                case eVariants.v2:
                    return a * b;
                case eVariants.v3:
                    return Math.Max(0, a + b - 1);
                case eVariants.v4:
                    return Math.Abs(a - 1) < 0.001 ? b : (Math.Abs(b - 1) < 0.001 ? a : 0);
                case eVariants.v5:
                    return (a * b) / (Math.Max(a, Math.Max(b, y)));
                case eVariants.v6:
                    return (a * b) / (y + (1 - y) * (a + b - a * b));
                case eVariants.v7:
                    return 1.0 / (1 + (Math.Pow((Math.Pow((1.0 / a - 1), y)) + (Math.Pow((1.0 / b - 1), y)), 1 / y)));
                case eVariants.v8:
                    return 1.0 / (-1 + (Math.Pow((Math.Pow(a, -y)) + (Math.Pow(b, -y)), 1 / y)));

            }
            return 0;
        }
        double CalcS(double a, double b)
        {
            switch (currentMathType)
            {
                case eVariants.v1:
                    return Math.Max(a, b);
                case eVariants.v2:
                    return a + b - a * b;
                case eVariants.v3:
                    return Math.Min(1, a + b);
                case eVariants.v4:
                    return Math.Abs(a) < 0.001 ? b : (Math.Abs(b) < 0.001 ? a : 0);
                case eVariants.v5:
                    return ((1 - a) * (1 - b)) / (Math.Max(1 - a, Math.Max(1 - b, y)));
                case eVariants.v6:
                    return (a + b - (2 - y) * a * b) / (y - (1 - y) * (a * b));
                case eVariants.v7:
                    return 1.0 / (1 + (Math.Pow((Math.Pow((1.0 / a - 1), -y)) + (Math.Pow((1.0 / b - 1), -y)), 1 / y)));
                case eVariants.v8:
                    return 1 - (1.0 / ((Math.Pow((Math.Pow(1 - a, -y)) + (Math.Pow(1 - b, -y)), 1 / y))));

            }
            return 0;
        }

        public double GetResult(eVariants r)
        {
            currentMathType = r;
            List<double> valA = GetFuncValues();
            List<double> miyB = new List<double>();
            for (int i = 0; i < B.COUNT; i++)
            {
                List<double> Trez = GetTValues(valA, i);
                miyB.Add(CalcSValue(Trez));
            }
            var centers = B.GetListCenter();

            return B.GetResult(centers, miyB);
        }

        private double CalcSValue(List<double> trez)
        {
            return CalcS(trez[0], trez.Count == 2 ? trez[1] : CalcSValue(MinimazeOne(trez)));
        }

        private List<double> MinimazeOne(List<double> res)
        {
            res.RemoveAt(0);
            return res;
        }

        private List<double> GetTValues(List<double> valA, int index)
        {
            List<double> r = new List<double>();
            for (int i = 0; i < valA.Count; i++)
            {
                r.Add(CalcT(valA[i], R[i, index]));
            }
            return r;
        }
        public void Save(string path)
        {
            List<string> s = new List<string>();
            s.AddRange(A.GetSaveText());
            s.Add("///");
            s.AddRange(B.GetSaveText());
            s.Add("///");
            s.AddRange(GetRText());
            File.WriteAllLines(path, s);
        }
        public void ReadFromFile(string path)
        {
            A = new FuzzySet();
            B = new FuzzySet();

            string file = File.ReadAllText(path);
            string[] s = file.Split(new string[] { "///" }, 3, StringSplitOptions.RemoveEmptyEntries);
            A.ReadFromFile(s[0].Split(new string[] { "\r\n" }, 10000, StringSplitOptions.RemoveEmptyEntries).ToList());
            B.ReadFromFile(s[1].Split(new string[] { "\r\n" }, 10000, StringSplitOptions.RemoveEmptyEntries).ToList());
            R = new double[A.COUNT, B.COUNT];
            ReadRFromFile(s[2]);
        }

        private void ReadRFromFile(string v)
        {
            var r = v.Split(new string[] { "\r\n" }, 10000, StringSplitOptions.RemoveEmptyEntries).ToList().Select(x => x.Split(' ')).ToList();
            for (int i = 0; i < R.GetLength(0); i++)
            {
                for (int j = 0; j < R.GetLength(1); j++)
                {
                    R[i, j] = Convert.ToDouble(r[i][j]);
                }
            }
        }

        private List<string> GetRText()
        {
            List<string> s = new List<string>();
            for (int i = 0; i < R.GetLength(0); i++)
            {
                string ss = "";
                for (int j = 0; j < R.GetLength(1); j++)
                {
                    ss += R[i, j].ToString() + " ";
                }
                s.Add(ss);
            }
            return s;
        }
    }
}

