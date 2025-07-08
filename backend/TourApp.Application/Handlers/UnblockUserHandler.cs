using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class UnblockUserHandler : IRequestHandler<UnblockUserCommand, CommandResult>
    {
        private readonly IUserRepository _userRepository;

        public UnblockUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CommandResult> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                result.Errors.Add("User not found");
                return result;
            }

            user.Unblock();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            result.Success = true;
            return result;
        }
    }
}
