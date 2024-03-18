﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IObjectRequestRepository 
    {
        ObjectRequest AcceptRequest(int id);
        ObjectRequest RejectRequest(int id);
        ObjectRequest GetRequestByMapObjectId(int mapObjectId);
    }
}