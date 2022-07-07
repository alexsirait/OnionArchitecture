using Onion.Application.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Service.Contract
{
    public interface IUserService
    {
        // menggunakan Task Karena ini adalah asyncronus
        Task<UserResource> Get(int Id, CancellationToken cancellationToken);
        Task<List<UserResource>> GetAll(CancellationToken cancellationToken);
        Task<UserResource> Create(UserResource resource, CancellationToken cancellationToken);
        Task<UserResource> Update(UserResource resource, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
    }
}
