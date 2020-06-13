using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroRadar
{
    class resultJson
    {
        public class Flight
        {
            public string origin { get; set; }
            public string originCode { get; set; }
            public string destination { get; set; }
            public string destinationCode { get; set; }
            public int price { get; set; }
            public string beginDate { get; set; }
            public string endDate { get; set; }
            public Route[] route { get; set; }
        }

        public class Route
        {
            public string departureFrom { get; set; }
            public string departureFromCode { get; set; }
            public string arriveTo { get; set; }
            public string arriveToCode { get; set; }
            public string carrier { get; set; }
            public string flight { get; set; }
            public int price { get; set; }
            public string departure { get; set; }
            public string arrival { get; set; }
        }

    }


}
