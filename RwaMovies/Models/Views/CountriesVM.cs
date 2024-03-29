﻿using RwaMovies.Models.Shared;

namespace RwaMovies.Models.Views
{
    public class CountriesVM
    {
        public List<CountryDTO> Countries { get; set; } = null!;
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int PageCount { get; set; }
    }
}
