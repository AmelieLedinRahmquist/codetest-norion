﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Emergency : IVehicle
    {
        public string GetVehicleType() => "Emergency";
        public bool IsTollFree => true;
    }
}
