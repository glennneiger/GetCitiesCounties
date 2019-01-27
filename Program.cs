using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace GetCitiesCounties
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Hitting Wikipedia");

            var uri = new Uri("https://en.wikipedia.org/wiki/List_of_United_States_counties_and_county_equivalents");
            var client = new HttpClient();
            var rs = client.GetAsync(uri).Result;
            if (rs.IsSuccessStatusCode)
            {
                var htmlContent = rs.Content.ReadAsStringAsync().Result;
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);
                var list = new List<dynamic>();
                var state = string.Empty;
                var nodes = htmlDoc.DocumentNode.SelectNodes("//table[@class='wikitable sortable']//tr");

                Console.WriteLine(@"Processing Rows");

                int rowIndex = 0;
                foreach (var row in nodes)
                {
                    if (rowIndex++ > 0)
                    {
                        var county = row.SelectNodes("td")[0].InnerText;
                        var checkState = row.SelectNodes("td")[1].InnerText;
                        if (! row.SelectNodes("td")[1].InnerText.Replace(",","").Replace("\n","").Trim().All(char.IsDigit))
                        {
                            state = row.SelectNodes("td")[1].InnerText;
                        }


                        list.Add(new
                        {
                            County = county,
                            State = state
                        });
                    }
                }

                var json = JsonConvert.SerializeObject(list);
                File.WriteAllText(@"countyDataMin.json", json);

                Console.WriteLine(@"Done, extracted cities and states to json file C:\test.json");
                Console.ReadLine();
            }
        }
    }
}
