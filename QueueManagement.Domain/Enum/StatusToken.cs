using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Enum
{
     

    public enum StatusToken : byte
    {

        //Token hợp lệ và sử dụng được
        [EnumMember(Value = "active")]
        Active = 1,
        //Token đã hết hạn
        [EnumMember(Value = "expired")]
        Expired = 2,

        //Đã bị thu hồi
        [EnumMember(Value = "revoked")]
        Revoked = 3,
        //Đưa vào danh sách đen
        [EnumMember(Value = "blacklisted")]
        Blacklisted = 4,
        //Sử dụng refresh token
        [EnumMember(Value = "used")]
        Used = 5,
        //Token không hợp lệ
        [EnumMember(Value = "invalid")]
        Invalid = 6
    }
}