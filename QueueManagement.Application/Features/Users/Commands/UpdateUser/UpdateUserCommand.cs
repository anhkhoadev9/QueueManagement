using MediatR;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enum;
using QueueManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public StatusUser StatusUser { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.UpdateProfile(request.FullName, request.Email, request.PhoneNumber, request.BirthDay, request.StatusUser);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
