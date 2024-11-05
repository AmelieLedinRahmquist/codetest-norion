using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollFeeCalculator.Interfaces;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Foreign : IVehicle
    {
        public string GetVehicleType() => "Foreign";
        public bool IsTollFree => true;
    }
}
