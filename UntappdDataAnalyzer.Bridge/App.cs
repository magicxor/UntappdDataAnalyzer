using Bridge.Html5;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using UntappdDataAnalyzer.Core.Models;
using UntappdDataAnalyzer.Core.Services;

namespace UntappdDataAnalyzer.Bridge
{
    public class App
    {
        public static void AddJquery(HTMLElement head)
        {
            var scriptJquery = new HTMLScriptElement()
            {
                Src = "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js",
                CrossOrigin = "anonymous",
            };
            scriptJquery.SetAttribute("integrity", "sha256-FgpCb/KJQlLNfOu91ta32o/NMZxltwRo8QtmkMRdAu8=");
            head.AppendChild(scriptJquery);
        }

        public static void AddHighcharts(HTMLElement head)
        {
            var scriptHighcharts = new HTMLScriptElement()
            {
                Src = "https://cdnjs.cloudflare.com/ajax/libs/highcharts/6.0.7/highcharts.js",
                CrossOrigin = "anonymous",
            };
            scriptHighcharts.SetAttribute("integrity", "sha256-F0xKYvUdYPCgKKgKGtEjxwHXKSRbwKP+2mOlgGoR0Fs=");
            head.AppendChild(scriptHighcharts);
        }

        public static void AddBootstrap(HTMLElement head)
        {
            var scriptBootstrap = new HTMLScriptElement()
            {
                Src = "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js",
                CrossOrigin = "anonymous",
            };
            scriptBootstrap.SetAttribute("integrity", "sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl");
            head.AppendChild(scriptBootstrap);

            var linkBootstrap = new HTMLLinkElement()
            {
                Href = "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css",
                Rel = "stylesheet",
            };
            linkBootstrap.SetAttribute("integrity", "sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm");
            linkBootstrap.SetAttribute("crossorigin", "anonymous");
            head.AppendChild(linkBootstrap);
        }

        public static void AddChartsRender(HTMLElement head)
        {
            var scriptChartsRender = new HTMLScriptElement()
            {
                Src = "ChartsRender.js",
            };
            head.AppendChild(scriptChartsRender);
        }

        public static void AddDivAsRow(HTMLDivElement container, string divId)
        {
            var row = new HTMLDivElement() { ClassName = "row" };
            var div = new HTMLDivElement() { Id = divId, ClassName = "col-md" };
            row.AppendChild(div);
            container.AppendChild(row);
        }

        public static void Main()
        {
            AddJquery(Document.Head);
            AddBootstrap(Document.Head);
            AddHighcharts(Document.Head);
            AddChartsRender(Document.Head);

            var divContainer = new HTMLDivElement() { ClassName = "container-fluid" };
            Document.Body.AppendChild(divContainer);

            AddDivAsRow(divContainer, "medianRatingByCountry");
            AddDivAsRow(divContainer, "countByCountry");
            AddDivAsRow(divContainer, "medianRatingByBrewery");
            AddDivAsRow(divContainer, "countByBrewery");
            AddDivAsRow(divContainer, "medianRatingByStyle");
            AddDivAsRow(divContainer, "countByStyle");

            var row = new HTMLDivElement { ClassName = "row" };
            divContainer.AppendChild(row);
            var col = new HTMLDivElement { ClassName = "mx-auto" };
            row.AppendChild(col);
            var formGroup = new HTMLDivElement { ClassName = "form-group" };
            col.AppendChild(formGroup);
            var input = new HTMLInputElement { Type = InputType.File, Accept = ".json" };
            formGroup.AppendChild(input);
            var small = new HTMLElement("small") { ClassList = { "form-text", "text-muted" } };
            formGroup.AppendChild(small);

            var button = new HTMLButtonElement
            {
                InnerHTML = "Process",
                ClassList = { "btn", "btn-primary" },
                OnClick = (clickEvent) =>
                {
                    if (input.Files.Length > 0 && input.Files[0] != null)
                    {
                        var reader = new FileReader();
                        reader.OnLoad = loadEvent =>
                        {
                            var rawDataString = reader.Result.ToString();

                            var data = JsonConvert.DeserializeObject<IList<Checkin>>(rawDataString);

                            var dataAnalyzer = new DataAnalyzer();

                            var statsByCountry = dataAnalyzer.GetStatistics(data, checkin => checkin.BreweryCountry, checkin => checkin.RatingScore);
                            var statsByBrewery = dataAnalyzer.GetStatistics(data, checkin => checkin.BreweryName, checkin => checkin.RatingScore);
                            var statsByStyle = dataAnalyzer.GetStatistics(data, checkin => checkin.BeerType, checkin => checkin.RatingScore);
                            
                            var medianRatingByCountry = statsByCountry.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByCountry = statsByCountry.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianRatingByBrewery = statsByBrewery.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByBrewery = statsByBrewery.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianRatingByStyle = statsByStyle.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByStyle = statsByStyle.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByCountry), nameof(countByCountry), medianRatingByCountry.ToArray(), countByCountry.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByCountry), countByCountry.ToArray());
                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByBrewery), nameof(countByBrewery), medianRatingByBrewery.ToArray(), countByBrewery.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByBrewery), countByBrewery.ToArray());
                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByStyle), nameof(countByStyle), medianRatingByStyle.ToArray(), countByStyle.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByStyle), countByStyle.ToArray());
                        };
                        reader.ReadAsText(input.Files[0]);
                    }
                }
            };
            col.AppendChild(button);
        }
    }
}