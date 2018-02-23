using Newtonsoft.Json;
using UntappdDataAnalyzer.Core.Models;

namespace UntappdDataAnalyzer.Core.Extensions
{
    public static class CheckinExtensions
    {
        public static Checkin DeepCopy(this Checkin checkin)
        {
            return JsonConvert.DeserializeObject<Checkin>(JsonConvert.SerializeObject(checkin));
        }
    }
}
