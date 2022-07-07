using Microsoft.EntityFrameworkCore;
using Onion.Application.Resource;
using Onion.Application.Service.Contract;
using Onion.Domain.Entities;
using Onion.Domain.Exceptions;
using Onion.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IDataContext _db;

        public UserService(IDataContext db)
        {
            _db = db;
        }

        public async Task<UserResource> Get(int Id, CancellationToken cancellationToken)
        {
            var user = await _db.GetQuery<User>().Where(x => x.Id == Id)
                                            .Select(x => new UserResource
                                            {
                                                Id = x.Id,
                                                Name = x.Name,
                                            }).FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new NotFoundException("User Not Found! :(");
            return user;
        }

        public async Task<List<UserResource>> GetAll(CancellationToken cancellationToken)
        {
            return await _db.GetQuery<User>().Select(x => new UserResource
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync(cancellationToken);
        }
        public async Task<UserResource> Create(UserResource userResource, CancellationToken cancellationToken)
        {
            Validate(userResource);
            var user = new User
            {
                Name = userResource.Name
            };
            _db.Create(user);
            await _db.Save(cancellationToken);

            return await Get(user.Id, cancellationToken);

        }
        public async Task<UserResource> Update(UserResource userResource, CancellationToken cancellationToken)
        {
            Validate(userResource);
            var user = _db.GetQuery<User>().FirstOrDefault(x => x.Id == userResource.Id);

            if (user == null)
                throw new NotFoundException("User Not Found! :(");
            user.Name = userResource.Name;
            await _db.Save(cancellationToken);
            return await Get(user.Id, cancellationToken);
        }

        public async Task Delete(int Id, CancellationToken cancellationToken)
        {
            var user = await _db.GetQuery<User>().FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);
            if (user == null)
                throw new NotFoundException("User Not Found! :(");
            _db.Delete(user);
            await _db.Save(cancellationToken);
        }

        private void Validate(UserResource userResource)
        {
            if (string.IsNullOrWhiteSpace(userResource.Name))
                throw new BadRequestException("Required!");
        }
    }
}
