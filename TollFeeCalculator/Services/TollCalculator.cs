﻿using System;
using System.Globalization;
using Nager.Date;
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
            if (vehicle == null)//Check if the vehicle variable is null
            {
                throw new ArgumentNullException("Vehicle cannot be null");
            }
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
                    //If the current toll fee is greater than or equal to the previous toll fee, subtract the previous fee and add the current fee to the total fee
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

        /// <summary>
        /// Checks if a vehicle is toll free.
        /// </summary>
        /// <param name="vehicle">The vehicle that is checked.</param>
        /// <returns>True if the vehicle is toll free, false otherwise.</returns>
        private bool IsVehicleTollFree(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            return vehicle.IsTollFree;
        }

        /// <summary>
        /// Checks if a date is toll free.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the date is toll free, false otherwise.</returns>
        private bool IsDateTollFree(DateTime date)
        {
            //Checks if the date is a Saturday, Sunday or if the month is July and returns true if it is
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || date.Month == 7)
            {
                return true;
            }

            //Using the the Nager.Date library to get Swedish public holidays for the current year
            var holidays = Nager.Date.PublicHolidays.GetHolidays("SE", date.Year);

            // Check if the date is a public holiday and return true if it is
            foreach (var holiday in holidays)
            {
                if (holiday.Date == date.Date)
                {
                    return true;
                }
            }

            // Check if the date is the day before a public holiday and return true if it is
            foreach (var holiday in holidays)
            {
                if (holiday.Date == date.AddDays(1).Date)
                {
                    return true;
                }
            }
            return false;
        }
    }
}