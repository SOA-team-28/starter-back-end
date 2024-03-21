using Explorer.Stakeholders.API.Dtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IObjectRequestService
    {
        Result<ObjectRequestDto> Create(ObjectRequestDto request);
        Result<ObjectRequestDto> Create(int mapObjectId, int authorId, string status);
        Result<ObjectRequestDto> Update(ObjectRequestDto request);
        Result<List<ObjectRequestDto>> GetAll();
        Result<ObjectRequestDto> AcceptRequest(int id, string notificationComment);
        Result<ObjectRequestDto> RejectRequest(int id, string notificationComment);
    }
}
