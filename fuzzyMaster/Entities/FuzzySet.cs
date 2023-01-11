using fuzzyMaster.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzyMaster
{
    internal class FuzzySet
    {
        public List<Function> functions;

        public int COUNT { get => functions.Count; }

        public FuzzySet()
        {
            functions = new List<Function>();
        }
        public void AddFunk(List<Point> points)
        {
            functions.Add(new Function(points));
        }
        public void AddFunk(Function functions)
        {
            this.functions.Add(functions);
        }

        internal Function GetFunk(int index)
        {
            return functions[index];
        }

        internal List<Function> GetFunks()
        {
            return functions;
        }

        public List<double> GetListCenter()
        {
            List<double> result = new List<double>();

            foreach (var f in functions)
                result.Add(f.CENTER);
            

            return result;
        }

        public double GetResult(List<double> centers, List<double> values)
        {
            double upper = 0;
            double downer = 0;
            if (centers.Count == values.Count)
            {
                for (int i = 0; i < centers.Count; i++)
                {
                    upper += centers[i] * values[i];
                    downer += values[i];
                }
                return upper / downer;
            }
            return -1000;
        }

        internal void SetX(double value)
        {
            foreach (var f in functions)
                f.SetX(value);
        }
        public void Save(string path)
        {
            foreach (var f in functions)
                f.Save(path);
            
        }

        internal List<string> GetSaveText()
        {
            List<string> s = new List<string>();
            foreach (var f in functions)
                s.Add(f.GetSaveString());
            
            return s;
        }

        public void ReadFromFile(List<string> sss)
        {
            foreach (string s in sss)
                functions.Add(new Function(s));
        }
    }
}
