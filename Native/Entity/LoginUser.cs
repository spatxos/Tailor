using System;

namespace Entity
{
    public class LoginUser:BaseEntity
    {
        public Guid UserGuid { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
