﻿using Playground.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    public interface IGameCategoryBusiness
    {
        Result<GameCategory> GetById(int id);
        Result<PagedResult<GameCategory>> GetGameCategories(int page, int count);
        Result<GameCategory> AddGameCategory(GameCategory gameCategory);
        Result<GameCategory> UpdateGameCategory(GameCategory gameCategory);
        Result<GameCategory> DeleteGameCategory(int id);
    }
}
