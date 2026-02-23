using Aliare.Weather.Api.Domain.Entities;

namespace Aliare.Weather.Api.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
