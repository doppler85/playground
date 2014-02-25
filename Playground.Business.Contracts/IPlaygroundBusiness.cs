using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IPlaygroundBusiness
    {
        Result<Playground.Model.Playground> GetById(int playgroundID);

        Result<PagedResult<Playground.Model.Playground>> GetPlaygrounds(int page, int count, bool all);

        Result<PagedResult<Playground.Model.Playground>> FilterByUser(int page, int count, int userID);

        Result<Playground.Model.Playground> AddPlayground(Playground.Model.Playground playground);

        Result<Playground.Model.Playground> UpdatePlayground(Playground.Model.Playground playground);

        bool RemovePlayground(int playgroundID);
        
        bool AddGameToPlayGround(PlaygroundGame playgroundGame);

        bool RemoveGameFromPlayground(PlaygroundGame playgroundGame);

        bool AddUserToPlaygroound(PlaygroundUser playgroundUser);

        bool RemoveUserFromPlayground(PlaygroundUser playgroundUser);
    }
}
