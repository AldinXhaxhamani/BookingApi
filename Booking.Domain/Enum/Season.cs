using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Enum
{
    public enum Season
    {
        Spring = 1,
        Summer = 2,
        Autumn = 3,
        Winter = 4
    }

    public static class SeasonHelper
    {
        public static Season GetSeason(DateTime date)
        {
            return date.Month switch
            {
                3 or 4 or 5 => Season.Spring,
                6 or 7 or 8 => Season.Summer,
                9 or 10 or 11 => Season.Autumn,
                _ => Season.Winter
            };
        }
    }

}
