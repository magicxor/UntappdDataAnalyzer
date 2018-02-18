using System.Collections.Generic;

namespace UntappdDataAnalyzer.Core.Models
{
    public class DataItem<TKey, TValue>
    {
        public TKey Key { get; set; }
        public IList<TValue> Values { get; set; }
        public int Count { get; set; }
        public double Average { get; set; }
        public double Median { get; set; }
    }
}
