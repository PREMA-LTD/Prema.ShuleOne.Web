using CsvHelper;
using CsvHelper.Configuration;
using Prema.ShuleOne.Web.Server.Models.Location;
using System.Formats.Asn1;
using System.Globalization;

namespace Prema.ShuleOne.Web.Server.Database.LocationData
{
    public static class LoadLocationData
    {
        public static List<(County, Subcounty, Ward)> LoadCsvData(string filePath)
        {
            var records = new List<(County, Subcounty, Ward)>();

            using (var reader = new StreamReader(filePath))
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
