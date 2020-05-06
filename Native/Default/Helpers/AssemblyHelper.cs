using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Default.Helpers
{
    public static class AssemblyHelper
    {
        public static GlobalData GetGlobalData()
        {
            var gd = new GlobalData();

            //获取所有程序集
            gd.AllAssembly = GetAllAssembly();
            var mvc = GetRuntimeAssembly("Default");
            if (mvc != null && gd.AllAssembly.Contains(mvc) == false)
            {
                gd.AllAssembly.Add(mvc);
            }


            return gd;
        }


        /// <summary>
        /// 获取所有继承 BaseController 控制器
        /// </summary>
        /// <param name="allAssemblies"></param>
        /// <returns></returns>
        public static List<Type> GetAllControllers(List<Assembly> allAssemblies)
        {
            var controllers = new List<Type>();
            foreach (var ass in allAssemblies)
            {
                var types = new List<Type>();
                try
                {
                    types.AddRange(ass.GetExportedTypes());
                }
                catch { }

                controllers.AddRange(types.Where(x => typeof(IBaseController).IsAssignableFrom(x)).ToList());
            }
            return controllers;
        }

        public static List<Type> GetControllers(Assembly allAssemblies)
        {
            var controllers = new List<Type>();
            var types = new List<Type>();
            try
            {
                types.AddRange(allAssemblies.GetExportedTypes());
            }
            catch { }

            controllers.AddRange(types.Where(x => typeof(IBaseController).IsAssignableFrom(x)).ToList());

            return controllers;
        }

        public static Assembly GetRuntimeAssembly(string name)
        {
            var path = Assembly.GetEntryAssembly().Location;
            var library = DependencyContext.Default.RuntimeLibraries.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (library == null)
            {
                return null;
            }
            var r = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
        {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
        });

            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblies = new List<string>();
            r.TryResolveAssemblyPaths(wrapper, assemblies);
            if (assemblies.Count > 0)
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblies[0]);
            }
            else
            {
                return null;
            }
        }

        public static List<Assembly> GetAllAssembly()
        {
            var rv = new List<Assembly>();
            var path = Assembly.GetEntryAssembly().Location;
            var dir = new DirectoryInfo(Path.GetDirectoryName(path));

            var dlls = dir.GetFiles("*.dll", SearchOption.AllDirectories);
            string[] systemdll = new string[]
            {
                "Microsoft.",
                "System.",
                "Swashbuckle.",
                "ICSharpCode",
                "Newtonsoft.",
                "Oracle.",
                "Pomelo.",
                "SQLitePCLRaw."
            };

            foreach (var dll in dlls)
            {
                try
                {
                    if (systemdll.Any(x => dll.Name.StartsWith(x)) == false)
                    {
                        rv.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName));
                    }
                }
                catch { }
            }
            return rv;
        }


    }
}
