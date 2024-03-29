﻿using AutoMapper;
using FlexateWebApi.Application.Dto.Offices;
using FlexateWebApi.Application.Interfaces;
using FlexateWebApi.Domain.Model;
using FlexateWebApi.Infrastructure.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlexateWebApi.Application.Services
{
    public class OfficesService : IOfficesService
    {
        private readonly IOfficesRepository _officesRepository;
        private readonly IMapper _mapper;

        public OfficesService(IOfficesRepository officesRepository, IMapper mapper)
        {
            _officesRepository = officesRepository;
            _mapper = mapper;
        }

        public async Task<OfficesForListDto> GetOffices(int pageSize, int pageNo, string searchString, CancellationToken cancellationToken)
        {
            var offices = await _officesRepository.GetOffices(pageSize, pageNo, searchString, cancellationToken);

            var noOfOffices = await _officesRepository.GetNoOfOffices(cancellationToken);

            List<OfficeForListDto> officesDto = _mapper.Map<List<OfficeForListDto>>(offices);

            var officesForListDto = new OfficesForListDto()
            {
                OfficesList = officesDto,
                CurrentPage = pageNo,
                Count = noOfOffices,
                PageSize = pageSize
            };
            return officesForListDto;
        }

        public async Task<SingleOfficeDto> GetOfficeById(int id, CancellationToken cancellationToken)
        {
            var office = await _officesRepository.GetOfficeById(id, cancellationToken);

            if (office == null || office.IsDeleted == true)
            {
                return null;
            }

            var officeDto = _mapper.Map<SingleOfficeDto>(office);

            return officeDto;
        }

        public async Task<int?> AddNewOffice(CreateOfficeDto officeDto, CancellationToken cancellationToken)
        {
            Office office = new Office()
            {
                City = officeDto.City,
                Address = officeDto.Address,
                PersonId = officeDto.PersonId,
                IsDeleted = false
            };

            if (string.IsNullOrEmpty(office.City)
                || string.IsNullOrEmpty(office.Address))
            {
                return null;
            }

            int id = await _officesRepository.AddOffice(office, cancellationToken);

            return id;
        }

        public async Task<bool> DeleteOffice(int id, CancellationToken cancellationToken)
        {
            try
            {
                return await _officesRepository.DeleteOffice(id, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateOffice(int id, UpdateOfficeDto officeDto, CancellationToken cancellationToken)
        {
            if (officeDto == null)
            {
                return false;
            }

            Office office = new Office()
            {
                Id = id,
                City = officeDto.City,
                Address = officeDto.Address,
                PersonId = officeDto.PersonId
            };

            return await _officesRepository.UpdateOffice(office, cancellationToken);
        }

        public async Task<bool> UpdateWithDeletionFlag(int id, CancellationToken cancellationToken)
        {
            try
            {
                return await _officesRepository.UpdateWithDeletionFlag(id, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
