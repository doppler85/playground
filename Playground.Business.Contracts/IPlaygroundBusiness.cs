﻿using Playground.Model;
using Playground.Model.Util;
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

        Result<Playground.Model.ViewModel.Playground> GetById(int playgroundID, int userID);

        Result<PagedResult<Playground.Model.Playground>> GetPlaygrounds(int page, int count, bool all);

        Result<PagedResult<Playground.Model.Playground>> FilterByUser(int page, int count, int userID);

        Result<Playground.Model.Playground> AddPlayground(Playground.Model.Playground playground);

        Result<Playground.Model.Playground> UpdatePlayground(Playground.Model.Playground playground);

        Result<List<Playground.Model.Playground>> SearchInArea(Location startLocation, Location endLocation, int maxResults);

        Result<PagedResult<Playground.Model.ViewModel.Playground>> SearchPlaygrounds(SearchAreaArgs args, int userID);

        int TotalPlaygroundsCound();

        bool RemovePlayground(int playgroundID);
        
        bool AddGameToPlayGround(PlaygroundGame playgroundGame);

        bool RemoveGameFromPlayground(PlaygroundGame playgroundGame);

        bool AddUserToPlaygroound(PlaygroundUser playgroundUser);

        bool RemoveUserFromPlayground(PlaygroundUser playgroundUser);

        Result<Playground.Model.ViewModel.PlaygroundStats> GetStats(int playgroundID);

    }
}


