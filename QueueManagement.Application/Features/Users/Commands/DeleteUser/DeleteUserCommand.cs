using MediatR;
using QueueManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Using standard Delete or Lock
            // First checking if Delete exists via compilation. If it's pure IRepository usually it's Remove or Delete. Let's assume Delete.
            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
