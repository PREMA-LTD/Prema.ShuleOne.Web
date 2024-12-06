using CsvHelper;
using CsvHelper.Configuration;
using Prema.ShuleOne.Web.Server.Models.Location;
using System.Globalization;
using System.IO;

namespace Prema.ShuleOne.Web.Server.Database.LocationData
{
    public static class LoadLocationData
    {
        // Existing method for loading CSV from a file
        public static List<(County, Subcounty, Ward)> LoadCsvData(string filePath)
        {
            var csvContent = File.ReadAllText(filePath);
            return ParseCsvContent(csvContent);
        }

        // New method for loading CSV from raw content
        public static List<(County, Subcounty, Ward)> LoadCsvDataFromContent(string csvContent)
        {
            return ParseCsvContent(csvContent);
        }

        // Shared method to parse CSV content
        private static List<(County, Subcounty, Ward)> ParseCsvContent(string csvContent)
        {
            var records = new List<(County, Subcounty, Ward)>();

            using (var reader = new StringReader(csvContent))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var county = new County
                    {
                        id = csv.GetField<int>("COUNTY ID"),
                        name = csv.GetField<string>("COUNTY NAME")
                    };

                    var subcounty = new Subcounty
                    {
                        id = csv.GetField<int>("CONSTITUENCY ID"),
                        name = csv.GetField<string>("CONSTITUENCY NAME"),
                        fk_county_id = csv.GetField<int>("COUNTY ID")
                    };

                    var ward = new Ward
                    {
                        id = csv.GetField<int>("WARD ID"),
                        name = csv.GetField<string>("WARD NAME"),
                        fk_subcounty_id = csv.GetField<int>("CONSTITUENCY ID")
                    };

                    records.Add((county, subcounty, ward));
                }
            }

            return records;
        }
    }
}
