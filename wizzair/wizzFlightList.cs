using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroRadar.wizz
{

    public class wizzFlightList
    {
        public bool isFlightChange { get; set; }
        public bool isSeniorOrStudent { get; set; }
        public Flightlist[] flightList { get; set; }
        public int adultCount { get; set; }
        public int childCount { get; set; }
        public int infantCount { get; set; }
        public bool wdc { get; set; }
    }

    public class Flightlist
    {
        public string departureStation { get; set; }
        public string arrivalStation { get; set; }
        public string departureDate { get; set; }
    }
}
