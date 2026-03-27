using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Application.Features.Auth.Commands.Revoked
{
    public class RevokedAllByUserIdCommandHandler : IRequestHandler<RevokedAllByUserIdCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RevokedAllByUserIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(RevokedAllByUserIdCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.RefreshTokenRepository.RevokedAllAsync(request.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
