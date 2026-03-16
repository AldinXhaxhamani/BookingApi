using Booking.Domain;
using Booking.Domain.Apartments.DTOs;
using Booking.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Booking.Application.Apartaments.Search
{
    public class SearchPropertiesQuery : IRequest<PagedResultDto<PropertySearchResultDto>>
    {

        public string Country { get; set; } = string.Empty;
        public string? City { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        public int? Guests { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public PropertyType? PropertyType { get; set; }
       public string? Amenities { get; set; }
        public double? MinRating { get; set; }

 
        public SortBy? SortBy { get; set; }

  
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }




}
