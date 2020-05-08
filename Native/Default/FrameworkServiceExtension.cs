using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Service;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Default.Provider;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Default.Helpers;
using Default.Conventions;

namespace Default
{
    public static class FrameworkServiceExtension
    {
        public static IServiceCollection AddFrameworkService(this IServiceCollection services,
            WebHostBuilderContext webHostBuilderContext = null
        )
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();

            var configBuilder = new ConfigurationBuilder();

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
            {
                var binLocation = Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrEmpty(binLocation))
                {
                    var binPath = new FileInfo(binLocation).Directory?.FullName;
                    if (File.Exists(Path.Combine(binPath, "appsettings.json")))
                    {
                        Directory.SetCurrentDirectory(binPath);
                        configBuilder.SetBasePath(binPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                    }
                }
            }
            else
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }

            if (webHostBuilderContext != null)
            {
                var env = webHostBuilderContext.HostingEnvironment;
                configBuilder
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            }

            var config = configBuilder.Build();

            new AppSettingProvider().Initial(config);

            var gd = AssemblyHelper.GetGlobalData();

            var currentNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            var currentAssembly = gd.AllAssembly.Where(x => x.ManifestModule.Name == $"{currentNamespace}.dll").FirstOrDefault();

            StackTrace ss = new StackTrace(true);
            MethodBase mb = ss.GetFrame(ss.FrameCount - 1).GetMethod();

            var userNamespace = mb.DeclaringType.Namespace;

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddRazorPages()
                .AddRazorRuntimeCompilation()
            .ConfigureApplicationPartManager(m =>
            {
                var feature = new ControllerFeature();

                if (currentAssembly != null)
                {
                    m.ApplicationParts.Add(new AssemblyPart(currentAssembly));
                }
                m.PopulateFeature(feature);
                services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
            })
            .AddControllersAsServices()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            {
                if (currentAssembly != null)
                {
                    options.FileProviders.Add(
                    new EmbeddedFileProvider(
                        currentAssembly,
                        currentNamespace // your external assembly's base namespace
                    )
                );
                }
            });
            services.AddSingleton<ILoginUserService, LoginUserService>();

            //services.AddMvc(options =>
            //{
            //    options.Conventions.Add(new ApiControllerVersionConvention(currentNamespace, userNamespace));
            //});

            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                //o.ApiVersionReader = new UrlSegmentApiVersionReader();
                //o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"), new QueryStringApiVersionReader("api-version"));
                //o.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"));
                o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"));
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
            });

            return services;
        }

        public static IApplicationBuilder UseFrameworkService(this IApplicationBuilder app, Action<IRouteBuilder> customRoutes = null)
        {
            app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (ConnectionResetException) { }
                if (context.Response.StatusCode == 404)
                {
                    await context.Response.WriteAsync(string.Empty);
                }
            });

            app.UseMiddleware<CustomRewriteMiddleware>();

            if (customRoutes != null)
            {
                app.UseMvc(customRoutes);
            }
            else
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "areaRoute",
                        template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

            }

            return app;
        }
    }


    /// <summary>
    /// 解决IIS InProgress下CurrentDirectory获取错误的问题
    /// </summary>
    internal class CurrentDirectoryHelpers

    {

        internal const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]

        private static extern IntPtr GetModuleHandle(string lpModuleName);



        [System.Runtime.InteropServices.DllImport(AspNetCoreModuleDll)]

        private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);



        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]

        private struct IISConfigurationData

        {

            public IntPtr pNativeApplication;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzFullApplicationPath;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzVirtualApplicationPath;

            public bool fWindowsAuthEnabled;

            public bool fBasicAuthEnabled;

            public bool fAnonymousAuthEnable;

        }



        public static void SetCurrentDirectory()

        {

            try

            {

                // Check if physical path was provided by ANCM

                var sitePhysicalPath = Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH");

                if (string.IsNullOrEmpty(sitePhysicalPath))

                {

                    // Skip if not running ANCM InProcess

                    if (GetModuleHandle(AspNetCoreModuleDll) == IntPtr.Zero)

                    {

                        return;

                    }



                    IISConfigurationData configurationData = default(IISConfigurationData);

                    if (http_get_application_properties(ref configurationData) != 0)

                    {

                        return;

                    }



                    sitePhysicalPath = configurationData.pwzFullApplicationPath;

                }



                Environment.CurrentDirectory = sitePhysicalPath;

            }

            catch

            {

                // ignore

            }

        }

    }
}
