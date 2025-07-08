using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Application.Services;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class BlockUserHandler : IRequestHandler<BlockUserCommand, CommandResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public BlockUserHandler(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<CommandResult> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                result.Errors.Add("User not found");
                return result;
            }

            user.Block();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            // Send notification email
            var reason = user switch
            {
                Domain.Entities.Tourist t when t.InvalidProblemsCount >= 10 => "Too many invalid problem reports",
                Domain.Entities.Guide g when g.CancelledToursCount >= 10 => "Too many cancelled tours",
                _ => "Violation of terms of service"
            };

            await _emailService.SendAccountBlockedNotificationAsync(user.Email, reason);

            result.Success = true;
            return result;
        }
    }
}
