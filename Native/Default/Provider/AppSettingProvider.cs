using Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Default.Provider
{
    public class AppSettingProvider
    {
        private static AppSettings _myappSettings;
        public static AppSettings _appSettings { get { return _myappSettings; } }

        public void Initial(IConfiguration configuration)
        {
            _myappSettings = new AppSettings()
            {
                ApiVersion = configuration["AppSettings:ApiVersion"]
            };
        }

    }
}
