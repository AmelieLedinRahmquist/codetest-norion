using System;
using System.Globalization;
using TollFeeCalculator.Models.Vehicle;
using TollFeeCalculator.Interfaces;

namespace TollFeeCalculator.Services
{

    public class TollCalculator
    {

        /// <summary>
        /// Calculates the total toll fee for a vehicle during a day based on toll passes.
        /// </summary>
        /// <param name="vehicle">The vehicle for which the toll fee is calculated</param>
        /// <param name="tollPassingTimestamps">An array of DateTime with the timestamps of all the toll passes for the vehicle on that day.</param>
        /// <returns>The total toll fee for that day, at a maximum of 60 SEK.</returns>
        public int GetTotalTollFee(IVehicle vehicle, DateTime[] tollPassingTimestamps)
        {
            if (tollPassingTimestamps.Length == 0) return 0;//Return 0 if there are no timestamps

            DateTime firstTollPassing = tollPassingTimestamps[0];
            int totalFee = 0;
            int firstTollPassingFee = CalculateTollFeeForTimestamp(firstTollPassing, vehicle);
            totalFee += firstTollPassingFee;

            DateTime previousTollPassing = firstTollPassing;
            int previousTollPassingFee = firstTollPassingFee;

            for (int i = 1; i < tollPassingTimestamps.Length; i++)
            {
                DateTime currentTollPassing = tollPassingTimestamps[i];
                int currentTollPassingFee = CalculateTollFeeForTimestamp(currentTollPassing, vehicle);

                //Calculate the difference in minutes between the current and previous toll passing
                double minutesBetweenPassings = (currentTollPassing - previousTollPassing).TotalMinutes;

                //If the difference in minutes is less than or equal to 60
                if (minutesBetweenPassings <= 60)
                {
                    //If the current toll fee is greater than or equal to the previous toll fee
                    if (currentTollPassingFee >= previousTollPassingFee)
                    {
                        totalFee -= previousTollPassingFee;
                        totalFee += currentTollPassingFee;
                    }
                }
                //If the difference between passings are more than 60 minutes, add the current toll fee to the total fee
                else
                {
                    totalFee += currentTollPassingFee;
                }
                //Update the previous toll passing and fee variables for the next iteration
                previousTollPassing = currentTollPassing;
                previousTollPassingFee = currentTollPassingFee;
            }

            //If the total fee is greater than 60, set it to 60
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        /// <summary>
        /// Calculates the toll fee for a vehicle passing a toll at a specific timestamp.
        /// </summary>
        /// <param name="tollPassingTimestamp">The date and time of the toll passing.</param>
        /// <param name="vehicle">The vehicle passing through the toll.</param>
        /// <returns>The toll fee in SEK for this toll passing.</returns>
        public int CalculateTollFeeForTimestamp(DateTime tollPassingTimestamp, IVehicle vehicle)
        {
            if (IsDateTollFree(tollPassingTimestamp) || IsVehicleTollFree(vehicle)) return 0;

            int hour = tollPassingTimestamp.Hour;
            int minute = tollPassingTimestamp.Minute;

            // Calculate the toll fee in SEK based on the hour and minute of the toll passing
            switch (hour)
            {
                case 6:
                    return (minute <= 29) ? 8 : 13; //For 6:00 to 6:29 return 8, for 6:30 to 6:59 return 13
                case 7:
                    return 18;
                case 8:
                    return (minute <= 29) ? 13 : 8; //For 8:00 to 8:29 return 13, for 8:30 to 8:59 return 8
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    return 8; //For 9:00 to 14:59 return 8
                case 15:
                    return (minute <= 29) ? 13 : 18; //For 15:00 to 15:29 return 13, for 15:30 to 15:59 return 18
                case 16:
                    return 18;
                case 17:
                    return 13;
                case 18:
                    return (minute <= 29) ? 8 : 0; //For 18:00 to 18:29 return 8, for 18:30 to 18:59 return 0
                default:
                    return 0; //For all other hours, return 0
            }
        }

        private bool IsVehicleTollFree(Vehicle vehicle)
        {
            if (vehicle == null) return false;
            string vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        private Boolean IsDateTollFree(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (year == 2013)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }

        private enum TollFreeVehicles
        {
            Motorbike = 0,
            Tractor = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}