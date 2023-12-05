﻿using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ITouristWalletRepository : ICrudRepository<TouristWallet>
    {
        public TouristWallet GetByUser(long userId);
        public TouristWallet PaymentAdventureCoins(long userId, int coins);
    }
}
