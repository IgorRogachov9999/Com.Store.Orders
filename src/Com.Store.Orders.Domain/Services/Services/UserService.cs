using AutoMapper;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Exceptions;
using Com.Store.Orders.Domain.Services.Services.Contracts;

namespace Com.Store.Orders.Domain.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetByEmailAndPassowrdAsync(string email, string passwordHash, CancellationToken ct)
        {
            var user = await _userRepository.GetByEmailAndPasswordAsync(email, passwordHash, ct);

            if (user == null)
            {
                throw new DomainEntityNotFoundException("Invalid email or password");
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
