using Com.Store.Orders.Domain.Services.Dto;

namespace Com.Store.Orders.Domain.Services.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDto> GetByEmailAndPassowrdAsync(string email, string passwordHash, CancellationToken ct);
    }
}
