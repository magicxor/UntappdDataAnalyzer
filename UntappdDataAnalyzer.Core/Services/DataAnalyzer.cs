using System;
using System.Collections.Generic;
using System.Linq;
using UntappdDataAnalyzer.Core.Extensions;
using UntappdDataAnalyzer.Core.Models;

namespace UntappdDataAnalyzer.Core.Services
{
    public class DataAnalyzer
    {
        public IList<DataItem<TKey, double?>> GetStatistics<TModel, TKey>(
            IEnumerable<TModel> data,
            Func<TModel, TKey> keySelector,
            Func<TModel, double?> valueSelector)
        {
            Func<IList<double?>, double> averageSelector = (values) => { return values.Where(v => v.HasValue).Average(v => v.Value); };
            Func<IList<double?>, double> medianSelector = (values) => { return values.Where(v => v.HasValue).Select(v => v.Value).Median(); };
            return GetStatistics(data, keySelector, valueSelector, averageSelector, medianSelector);
        }

        public IList<DataItem<TKey, int?>> GetStatistics<TModel, TKey>(
            IEnumerable<TModel> data,
            Func<TModel, TKey> keySelector,
            Func<TModel, int?> valueSelector)
        {
            Func<IList<int?>, double> averageSelector = (values) => { return values.Where(v => v.HasValue).Average(v => v.Value); };
            Func<IList<int?>, double> medianSelector = (values) => { return values.Where(v => v.HasValue).Select(v => v.Value).Median(); };
            return GetStatistics(data, keySelector, valueSelector, averageSelector, medianSelector);
        }

        public IList<DataItem<TKey, TValue>> GetStatistics<TModel, TKey, TValue>(
            IEnumerable<TModel> data,
            Func<TModel, TKey> keySelector,
            Func<TModel, TValue> valueSelector,
            Func<IList<TValue>, double> averageSelector,
            Func<IList<TValue>, double> medianSelector)
        {
            var result = data
                .GroupBy(
                    keySelector,
                    valueSelector,
                    (key, values) => new
                    {
                        Key = key,
                        Values = values.ToList(),
                    })
                .Select(group => new DataItem<TKey, TValue>
                {
                    Key = group.Key,
                    Values = group.Values,
                    Count = group.Values.Count,
                    Average = averageSelector(group.Values),
                    Median = medianSelector(group.Values),
                })
                .OrderBy(grouping => grouping.Key)
                .ToList();
            return result;
        }
    }
}
