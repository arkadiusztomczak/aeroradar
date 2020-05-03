using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroRadar
{
    public class wizzairCities
    {
        public City[] cities { get; set; }
        public object javascript { get; set; }
        public string getAirportFullname(string iata)
        {
            foreach (City c in cities)
                if (c.iata == iata) return c.shortName;
            return iata;
        }
        public string getAirportCountry(string iata)
        {
            foreach (City c in cities)
                if (c.iata == iata) return c.countryName;
            return iata;
        }
    }

    public class City
    {
        public string iata { get; set; }
        public float longitude { get; set; }
        public string currencyCode { get; set; }
        public float latitude { get; set; }
        public string shortName { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
        public Connection[] connections { get; set; }
        public string[] aliases { get; set; }
        public bool isExcludedFromGeoLocation { get; set; }
        public int rank { get; set; }
        public int?[] categories { get; set; }
    }

    public class Connection
    {
        public string iata { get; set; }
        public DateTime operationStartDate { get; set; }
        public DateTime rescueEndDate { get; set; }
        public bool isDomestic { get; set; }
        public bool isNew { get; set; }
    }

}
