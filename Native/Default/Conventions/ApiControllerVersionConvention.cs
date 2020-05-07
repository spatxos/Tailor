using Default.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Default.Conventions
{
    public class ApiControllerVersionConvention : IControllerModelConvention
    {
        public readonly string _currentNamespace;
        public readonly string _userNamespace;
        public ApiControllerVersionConvention(string currentNamespace, string userNamespace)
        {
            _currentNamespace = currentNamespace;
            _userNamespace = userNamespace;
        }
        public void Apply(ControllerModel controller)
        {
            var gd = AssemblyHelper.GetGlobalData();

            var currentAssembly = gd.AllAssembly.Where(x => x.ManifestModule.Name == $"{_currentNamespace}.dll").FirstOrDefault();

            var userAssembly = gd.AllAssembly.Where(x => x.ManifestModule.Name == $"{_userNamespace}.dll").FirstOrDefault();

            var current_controllers = AssemblyHelper.GetControllers(currentAssembly);
            var user_controllers = AssemblyHelper.GetControllers(userAssembly);
            if (current_controllers.Where(o => controller.ControllerType.FullName == o.FullName).Any())
            {
                if (user_controllers.Where(o => controller.ControllerType.FullName.Replace(_currentNamespace, "") == o.FullName.Replace(_userNamespace, "")).Any())
                {
                    if (controller.Attributes is List<object>
                        attributes)
                    {
                        attributes.Add(new ApiVersionAttribute("1.0"));
                    }
                }
                else
                {
                    if (controller.Attributes is List<object> attributes)
                    {
                        attributes.Add(new ApiVersionAttribute("2.0"));
                    }
                }
            }
            else
            {
                if (user_controllers.Where(o => controller.ControllerType.FullName == o.FullName).Any())
                {
                    if (controller.Attributes is List<object> attributes)
                    {
                        attributes.Add(new ApiVersionAttribute("2.0"));
                    }
                }
                else
                {
                    if (controller.Attributes is List<object>
                        attributes)
                    {
                        attributes.Add(new ApiVersionNeutralAttribute());
                    }
                }
            }
            if (!(controller.ControllerType.IsDefined(typeof(ApiVersionAttribute)) || controller.ControllerType.IsDefined(typeof(ApiVersionNeutralAttribute)) || controller.ControllerType.IsDefined(typeof(ApiControllerAttribute))))
            {
                if (controller.Attributes is List<object>
                    attributes)
                {
                    attributes.Add(new ApiVersionNeutralAttribute());
                }
            }
        }
    }
}
