using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class LoginUserService : ILoginUserService
    {
        public LoginUser GetLoginUser()
        {
            return new LoginUser()
            {
                UserGuid = Guid.NewGuid(),
                CreateDate = DateTime.Now
            };
        }
    }
}
