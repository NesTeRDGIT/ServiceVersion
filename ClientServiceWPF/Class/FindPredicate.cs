using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServiceWPF.Class
{
    public class FindPredicate<T, P>
    {
        public P FindValue { get; set; }
        private List<T> FindList { get; set; } = new List<T>();


        private int CurrIndex = -1;

        public T Next()
        {
            CurrIndex++;
            return CurrIndex >= FindList.Count ? default(T) : FindList[CurrIndex];
        }

        public void Clear()
        {
            FindValue = default(P);
            FindList.Clear();
            CurrIndex = -1;
        }

        public void AddVariant(IEnumerable<T> values)
        {
            FindList.AddRange(values);
        }

    }
}
