﻿using FlexateWebApi.Application.Dto;
using FlexateWebApi.Application.Interfaces;
using FlexateWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexateWebApi.Application.Services
{
    public class PeopleService : IPeopleService
    {
        public IList<Person> People { get; set; }

        public PeopleService()
        {
            People = Entity.InitializePeople();
        }

        public async Task<PeopleForListDto> GetPeople(int pageSize, int pageNo, string searchString)
        {
            var people = People.Where(p => p.Name.StartsWith(searchString));

            List<PersonForListDto> ListForPeople = new List<PersonForListDto>();

            foreach (var person in people)
            {
                ListForPeople.Add(new PersonForListDto() { Id = person.Id, Name = person.Name });
            }

            var prevPage = 1;
            if (pageNo > 1)
            {
                prevPage = pageNo - 1;
            }

            var nextPage = pageNo + 1;

            var temp1 = (double)People.Count / pageSize;
            var noOfPages = (int)Math.Round((double)People.Count / pageSize);

            var peopleToShow = ListForPeople.Skip(pageSize * (pageNo - 1)).Take(pageSize).ToList();

            var model = new PeopleForListDto()
            {
                PeopleList = peopleToShow,
                CurrentPage = pageNo,
                PageSize = pageSize,
                SearchString = searchString,
                Count = People.Count,
                PrevPage = prevPage,
                NextPage = nextPage,
                NoOfPage = noOfPages
            };

            return await Task.FromResult(model);
        }

        public async Task<SinglePersonDto> GetPersonById(int id)
        {
            var person = People.FirstOrDefault(e => e.Id == id);

            if (person == null || person.IsDeleted == true)
            {
                return null;
            }

            var personDto = new SinglePersonDto()
            {
                Name = person.Name,
                Address = person.Address,
                Age = person.Age
            };

            return await Task.FromResult(personDto);
        }

        public async Task<int?> AddNewPerson(CreatePersonDto personDto)
        {
            Person person = new Person()
            {
                Name = personDto.Name,
                Age = personDto.Age,
                Address = personDto.Address,
                IsDeleted = false,
                Id = GetLastPersonId() + 1
            };

            if (string.IsNullOrEmpty(person.Name)
                || string.IsNullOrEmpty(person.Address)
                || person.Age < 0)
            {
                return null;
            }

            People.Add(person);

            return await Task.FromResult(person.Id);
        }

        private int GetLastPersonId()
        {
            int lastId = People.LastOrDefault().Id;

            return lastId;
        }

        public async Task<bool> UpdatePerson(int personToUpdateId, UpdatePersonDto personDto)
        {
            var personToUpdate = People.FirstOrDefault(p => p.Id == personToUpdateId);

            if (personToUpdate == null)
            {
                return false;
            }

            personToUpdate.Name = personDto.Name;
            personToUpdate.Address = personDto.Address;
            personToUpdate.Age = personDto.Age;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeletePerson(int id)
        {
            var person = People.FirstOrDefault(person => person.Id == id);

            if (person == null)
            {
                return false;
            }
            
            People.Remove(person);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateWithDeleteFlag(int id)
        {
            var person = People.FirstOrDefault(person => person.Id == id);

            if (person == null)
            {
                return false;
            }

            person.IsDeleted = !person.IsDeleted;

            return await Task.FromResult(true);
        }
    }
}
