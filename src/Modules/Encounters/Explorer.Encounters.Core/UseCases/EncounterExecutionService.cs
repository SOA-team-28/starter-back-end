﻿using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using FluentResults;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterExecutionService : CrudService<EncounterExecutionDto, EncounterExecution>, IEncounterExecutionService
    {
        private readonly IEncounterExecutionRepository _encounterExecutionRepository;
        private readonly IMapper _mapper;
        private readonly IInternalShoppingService _shoppingService;
        public EncounterExecutionService(IEncounterExecutionRepository encounterExecutionRepository, IMapper mapper, IInternalShoppingService shoppingService) : base(encounterExecutionRepository, mapper)
        {
            _encounterExecutionRepository = encounterExecutionRepository;
            _mapper = mapper;
            _shoppingService = shoppingService;
        }

        public Result<EncounterExecutionDto> Create(EncounterExecutionDto encounterExecutionDto, long touristId)
        {
            EncounterExecution result;
            EncounterExecution encounterExecution = new EncounterExecution();

            try
            {
                encounterExecution = _mapper.Map<EncounterExecutionDto, EncounterExecution>(encounterExecutionDto);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }

            if (touristId != encounterExecution.TouristId)
                return Result.Fail(FailureCode.Forbidden);

            try
            {
                encounterExecution.Validate();
                result = _encounterExecutionRepository.Create(encounterExecution);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
            return MapToDto(result);
        }

        public Result<EncounterExecutionDto> Get(int id)
        {
            try
            {
                var result = _encounterExecutionRepository.Get(id);
                return MapToDto(result);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<EncounterExecutionDto> Update(EncounterExecutionDto encounterExecutionDto, long touristId)
        {
            EncounterExecution encounterExecution = MapToDomain(encounterExecutionDto);
            if (touristId != encounterExecution.TouristId)
                return Result.Fail(FailureCode.InvalidArgument).WithError("Not tourist enounter execution!");
            try
            {
                var result = CrudRepository.Update(encounterExecution);
                return MapToDto(result);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }
        public Result Delete(int id, long touristId)
        {
            EncounterExecution encounterExecution;
            try
            {
                encounterExecution = _encounterExecutionRepository.Get(id);

                if (touristId != encounterExecution.TouristId)
                    return Result.Fail(FailureCode.Forbidden);

                CrudRepository.Delete(id);
                return Result.Ok();
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<PagedResult<EncounterExecutionDto>> GetAllByTourist(int touristId, int page, int pageSize)
        {
            try
            {
                var result = _encounterExecutionRepository.GetAllByTourist(touristId);
                var paged = new PagedResult<EncounterExecution>(result, result.Count());
                return MapToDto(paged);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }
        public Result<PagedResult<EncounterExecutionDto>> GetAllCompletedByTourist(int touristId, int page, int pageSize)
        {
            try
            {
                var result = _encounterExecutionRepository.GetAllCompletedByTourist(touristId);
                var paged = new PagedResult<EncounterExecution>(result, result.Count());
                return MapToDto(paged);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }

        public Result<EncounterExecutionDto> Activate(int touristId, double touristLatitude, double touristLongitude, int executionId)
        {
            try
            {
                //TODO purchased tour?
                var execution = _encounterExecutionRepository.Get(executionId);
                if(execution.IsInRange(touristLatitude, touristLongitude))
                {
                    execution.Activate();
                    execution = _encounterExecutionRepository.Update(execution);
                }
                return MapToDto(execution);
            }
            catch (KeyNotFoundException e)
            {
                return Result.Fail(FailureCode.NotFound).WithError(e.Message);
            }
        }
    }
}
