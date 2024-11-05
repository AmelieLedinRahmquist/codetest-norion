﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TollFeeCalculator.Interfaces;

namespace TollFeeCalculator.Models.Vehicle
{
    public class Military : IVehicle
    {
        public string GetVehicleType() => "Military";
        public bool IsTollFree => true;
    }
}
