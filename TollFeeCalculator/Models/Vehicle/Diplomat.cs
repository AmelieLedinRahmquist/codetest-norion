﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Diplomat : IVehicle
    {
        public string GetVehicleType() => "Diplomat";
        public bool IsTollFree => true;
    }
}
