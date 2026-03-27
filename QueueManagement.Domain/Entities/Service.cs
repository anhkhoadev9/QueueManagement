using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Domain.Exceptions;

namespace QueueManagement.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public int EstimatedDurationMinus { get; private set; } // Thời gian ước tính (phút)
        public bool IsActive { get; private set; }

        public Service() { }

        public Service(string name, string description, int estimatedDurationMinus)
        {
            Name = name;
            Description = description;
            EstimatedDurationMinus = estimatedDurationMinus;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            IsDelete = false;
        }

        public void UpdateDetails(string name, string description, int estimatedDurationMinus)
        {
            Name = name;
            Description = description;
            EstimatedDurationMinus = estimatedDurationMinus;
        }

        public void LockService()
        {
            if (IsActive)
            {
                throw new DomainExceptions("Service already locked");
            }

            IsActive = false;
        }
        public void DeletedSoft()
        {
            //Nếu như là IsDelete=false thì nó chưa xóa
            if (IsDelete)
                throw new DomainExceptions(" Service already deleted! ");
            IsDelete = true;

        }
    }
}
