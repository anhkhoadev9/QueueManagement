using QueueManagement.Domain.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Entities
{
    public class Account: BaseEntity
    {
        public string Code { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public Guid UserId { get; set; }
        public readonly ICollection<User>users = new Collection<User>();
        public Account() { }    
     
    }
}
