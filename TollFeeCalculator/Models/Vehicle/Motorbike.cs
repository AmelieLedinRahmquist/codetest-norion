using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Motorbike : IVehicle
    {
        public string GetVehicleType() => "Motorbike";
        public bool IsTollFree => true;
    }
}
