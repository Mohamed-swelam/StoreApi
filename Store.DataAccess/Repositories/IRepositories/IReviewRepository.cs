﻿using Store.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.IRepositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        void Update(Review review);
    }
}
