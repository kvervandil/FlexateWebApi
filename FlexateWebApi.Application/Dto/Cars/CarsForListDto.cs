﻿using AutoMapper;
using FlexateWebApi.Application.Mapping;
using FlexateWebApi.Domain.Model;
using System;
using System.Collections.Generic;

namespace FlexateWebApi.Application.Dto.Cars
{
        public class CarsForListDto
        {
            public List<CarForListDto> CarsList { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public string SearchString { get; set; }
            public int Count { get; set; }
            public bool IsFirstPage
            {
                get
                {
                    return CurrentPage == 1;
                }
            }
            public bool IsLastPage
            {
                get
                {
                    return NoOfPages == CurrentPage;
                }
            }
            public int NoOfPages => (int)Math.Ceiling((double)Count / PageSize);
        }
}
