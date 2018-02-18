using System;
using Newtonsoft.Json;

namespace UntappdDataAnalyzer.Core.Models
{
    public class Checkin
    {
        [JsonProperty(PropertyName = "beer_name")]
        public string BeerName;

        [JsonProperty(PropertyName = "brewery_name")]
        public string BreweryName;

        [JsonProperty(PropertyName = "beer_type")]
        public string BeerType;

        [JsonProperty(PropertyName = "beer_abv")]
        public double? BeerAbv;

        [JsonProperty(PropertyName = "beer_ibu")]
        public int? BeerIbu;

        [JsonProperty(PropertyName = "comment")]
        public string Comment;

        [JsonProperty(PropertyName = "venue_name")]
        public string VenueName;

        [JsonProperty(PropertyName = "venue_city")]
        public string VenueCity;

        [JsonProperty(PropertyName = "venue_state")]
        public string VenueState;

        [JsonProperty(PropertyName = "venue_country")]
        public string VenueCountry;

        [JsonProperty(PropertyName = "venue_lat")]
        public double? VenueLat;

        [JsonProperty(PropertyName = "venue_lng")]
        public double? VenueLng;

        [JsonProperty(PropertyName = "rating_score")]
        public double? RatingScore;

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt;

        [JsonProperty(PropertyName = "checkin_url")]
        public string CheckinUrl;

        [JsonProperty(PropertyName = "beer_url")]
        public string BeerUrl;

        [JsonProperty(PropertyName = "brewery_url")]
        public string BreweryUrl;

        [JsonProperty(PropertyName = "brewery_country")]
        public string BreweryCountry;

        [JsonProperty(PropertyName = "brewery_city")]
        public string BreweryCity;

        [JsonProperty(PropertyName = "brewery_state")]
        public string BreweryState;

        [JsonProperty(PropertyName = "flavor_profiles")]
        public string FlavorProfiles;

        [JsonProperty(PropertyName = "purchase_venue")]
        public string PurchaseVenue;

        [JsonProperty(PropertyName = "serving_type")]
        public string ServingType;
    }
}
