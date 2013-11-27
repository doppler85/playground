using Playground.Data.Contracts;
using Playground.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Data
{
    public class GameCompetitionTypeRepository : EFRepository<GameCompetitionType>, IGameCompetitionTypeRepository
    {
        public GameCompetitionTypeRepository(DbContext context) : base(context) { }

        public override GameCompetitionType GetById(int id)
        {
            throw new InvalidOperationException("Cannot return a single GameCompetitionType object by single id value.");
        }

        public override GameCompetitionType GetById(long id)
        {
            throw new InvalidOperationException("Cannot return a single GameCompetitionType object by single id value.");
        }

        public override void Delete(int id)
        {
            throw new InvalidOperationException("Cannot delete GameCompetitionType object by single id value.");
        }

        public IQueryable<GameCompetitionType> GetByGameId(int id)
        {
            return DbSet.Include("CompetitionType").Where(gc => gc.GameID == id);
        }

        public IQueryable<GameCompetitionType> GetAvailableByGameId(int id)
        {
            
            return DbSet.Include("CompetitionType").Where(gc => gc.GameID != id);
        }

        public GameCompetitionType GetByIds(int gameId, int comeptitionTypeId)
        {
            return DbSet.FirstOrDefault(gc => gc.GameID == gameId && gc.CompetitionTypeID == comeptitionTypeId);
        }

        public void Delete(int gameId, int comeptitionTypeId)
        {
            var entity = GetByIds(gameId, comeptitionTypeId);
            if (entity == null) return;
            Delete(entity);
        }
    }
}
