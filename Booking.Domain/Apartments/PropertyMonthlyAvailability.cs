using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace Booking.Domain.Apartments
{
    public class PropertyMonthlyAvailability
    {

        [Key]
        public Guid Id { get; private set; }

        public Guid PropertyId { get; private set; }

        public int Year { get; private set; }

        public int Month { get; private set; }

        // datat is ruajme ne databaze si Json Array [1,2,3,4,5...31]
        public string AvailableDaysJson { get; private set; } = "[]";


        
        [NotMapped]
        public List<int> AvailableDays  //nga nje string ne databaze i konvertojme ne nje liste ne kod 
        {
            get => JsonSerializer.Deserialize<List<int>>(AvailableDaysJson) ?? new();
            private set => AvailableDaysJson = JsonSerializer.Serialize(value);
        }

        public Property Property { get; set; } = null!;

        private PropertyMonthlyAvailability() { }

        // krijojme nje muaj te ri me te gjitha datat e lira (te gjitha datat te ruajtura ne db)
        public static PropertyMonthlyAvailability CreateForMonth(
            Guid propertyId, int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);//marrim sa dite ka muaji
            var allDays = Enumerable.Range(1, daysInMonth).ToList();

            var row = new PropertyMonthlyAvailability
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                Year = year,
                Month = month
            };

            row.AvailableDays = allDays;
            return row;
        }

        // removes booked days from the available list
        public void RemoveDays(List<int> days)
        {
            var current = AvailableDays;
            current.RemoveAll(d => days.Contains(d));
            AvailableDays = current;
        }

        // restores days (owner unblocks or booking cancelled)
        public void RestoreDays(List<int> days)
        {
            var current = AvailableDays; //vektor me datat e lira
            var daysInMonth = DateTime.DaysInMonth(Year, Month);

            foreach (var day in days)
            {
                // shto ne liste datat e lira te muajit 
                if (day >= 1 && day <= daysInMonth && !current.Contains(day))
                    current.Add(day);
            }

            current.Sort();
            AvailableDays = current;
        }

        public bool IsDayAvailable(int day) => AvailableDays.Contains(day);
    }
}

