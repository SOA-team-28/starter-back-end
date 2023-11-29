﻿using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain.Encounters;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Tours;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterDatabaseRepository:IEncounterRepository
    {
        private readonly EncountersContext _dbContext;

        public EncounterDatabaseRepository(EncountersContext encountersContext)
        {
            _dbContext = encountersContext;
        }

        public Encounter Create(Encounter encounter)
        {
            _dbContext.Encounter.Add(encounter);
            _dbContext.SaveChanges();

            return encounter;
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Encounter Get(long id)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Encounter> GetPaged(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Encounter Update(Encounter entity)
        {
            throw new NotImplementedException();
        }
    }
}
