using Default.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Default
{
    public class CustomRewriteMiddleware
    {
        private readonly RequestDelegate _next;

        //Your constructor will have the dependencies needed for database access
        public CustomRewriteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers.Add("api-version", AppSettingProvider._appSettings.ApiVersion);

            if (string.IsNullOrWhiteSpace(context.Request.Path.Value.Replace("/","").Replace("\\", "")))
            {
                context.Request.Path = "/Home/Index";
            }

            await _next.Invoke(context);

        }
    }
}
