﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollFeeCalculator.Interfaces;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Tractor : IVehicle
    {
        public string GetVehicleType() => "Tractor";
        public bool IsTollFree => true;
    }
}
