using System;
using Bridge.Html5;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using UntappdDataAnalyzer.Core.Extensions;
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
            AddDivAsRow(divContainer, "countByServingType");
            AddDivAsRow(divContainer, "medianRatingByFlavorProfile");
            AddDivAsRow(divContainer, "countByFlavorProfile");
            AddDivAsRow(divContainer, "medianAbvByDayOfWeek");
            divContainer.AppendChild(new HTMLHRElement());
            AddDivAsRow(divContainer, "additionalStats");

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

                            var medianRating = dataAnalyzer.GetMedian(data.Where(c => c.RatingScore.HasValue).Select(c => c.RatingScore.Value));
                            var medianAbv = dataAnalyzer.GetMedian(data.Where(c => c.BeerAbv.HasValue).Select(c => c.BeerAbv.Value));
                            var medianIbu = dataAnalyzer.GetMedian(data.Where(c => c.BeerIbu.HasValue).Select(c => c.BeerIbu.Value));

                            var dataByFlavorProfile = data.Aggregate(new List<Checkin>(), (checkins, checkin) =>
                            {
                                if (!string.IsNullOrWhiteSpace(checkin.FlavorProfiles))
                                {
                                    var flavorProfiles = checkin.FlavorProfiles.Split(',');
                                    foreach (var flavorProfile in flavorProfiles)
                                    {
                                        var checkinCopy = checkin.DeepCopy();
                                        checkinCopy.FlavorProfiles = flavorProfile;
                                        checkins.Add(checkinCopy);
                                    }
                                }
                                return checkins;
                            });

                            var statsByCountry = dataAnalyzer.GetStatistics(data.Where(d => !string.IsNullOrEmpty(d.BreweryCountry)), checkin => checkin.BreweryCountry, checkin => checkin.RatingScore);
                            var statsByBrewery = dataAnalyzer.GetStatistics(data.Where(d => !string.IsNullOrEmpty(d.BreweryName)), checkin => checkin.BreweryName, checkin => checkin.RatingScore);
                            var statsByStyle = dataAnalyzer.GetStatistics(data.Where(d => !string.IsNullOrEmpty(d.BeerType)), checkin => checkin.BeerType, checkin => checkin.RatingScore);
                            var statsByServingType = dataAnalyzer.GetStatistics(data.Where(d => !string.IsNullOrEmpty(d.ServingType)), checkin => checkin.ServingType, checkin => checkin.RatingScore);
                            var statsByFlavorProfile = dataAnalyzer.GetStatistics(dataByFlavorProfile, checkin => checkin.FlavorProfiles, checkin => checkin.RatingScore);
                            var statsByDayOfWeek = dataAnalyzer.GetStatistics(data, checkin => checkin.CreatedAt.DayOfWeek, checkin => checkin.BeerAbv);

                            var medianRatingByCountry = statsByCountry.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByCountry = statsByCountry.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianRatingByBrewery = statsByBrewery.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByBrewery = statsByBrewery.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianRatingByStyle = statsByStyle.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByStyle = statsByStyle.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var countByServingType = statsByServingType.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianRatingByFlavorProfile = statsByFlavorProfile.OrderByDescending(item => item.Median).Select(item => new { name = item.Key, y = item.Median }).ToList();
                            var countByFlavorProfile = statsByFlavorProfile.OrderByDescending(item => item.Count).Select(item => new { name = item.Key, y = item.Count }).ToList();

                            var medianAbvByDayOfWeek = statsByDayOfWeek.OrderBy(item => item.Key).Select(item => new { name = item.Key.ToString(), y = Math.Round(item.Median, 2) }).ToList();
                            var countByDayOfWeek = statsByDayOfWeek.OrderBy(item => item.Key).Select(item => new { name = item.Key.ToString(), y = item.Count }).ToList();

                            Document.GetElementById("additionalStats").AppendChild(new HTMLParagraphElement()
                            {
                                ClassName = "text-center",
                                InnerHTML = $"<b>{nameof(medianRating)}: {medianRating}, {nameof(medianAbv)}: {medianAbv}, {nameof(medianIbu)}: {medianIbu}</b>",
                            });

                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByCountry), nameof(countByCountry), medianRatingByCountry.ToArray(), countByCountry.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByCountry), countByCountry.ToArray());
                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByBrewery), nameof(countByBrewery), medianRatingByBrewery.ToArray(), countByBrewery.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByBrewery), countByBrewery.ToArray());
                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByStyle), nameof(countByStyle), medianRatingByStyle.ToArray(), countByStyle.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByStyle), countByStyle.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByServingType), countByServingType.ToArray());
                            Script.Call("renderRatingAndCountColumnChart", nameof(medianRatingByFlavorProfile), nameof(countByFlavorProfile), medianRatingByFlavorProfile.ToArray(), countByFlavorProfile.ToArray());
                            Script.Call("renderAsPieChart", nameof(countByFlavorProfile), countByFlavorProfile.ToArray());
                            Script.Call("renderValueAndCountColumnChart", nameof(medianAbvByDayOfWeek), nameof(countByDayOfWeek), medianAbvByDayOfWeek.ToArray(), countByDayOfWeek.ToArray());
                        };
                        reader.ReadAsText(input.Files[0]);
                    }
                }
            };
            col.AppendChild(button);
        }
    }
}