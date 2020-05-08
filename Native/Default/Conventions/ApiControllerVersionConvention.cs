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
        public ApiControllerVersionConvention()
        {
        }
        public void Apply(ControllerModel controller)
        {
            //当Controller未增加版本或者不区分版本属性时，自动增加不区分版本属性
            if (!(controller.ControllerType.IsDefined(typeof(ApiVersionAttribute)) || controller.ControllerType.IsDefined(typeof(ApiVersionNeutralAttribute))))
            {
                if (controller.Attributes is List<object>
                    attributes)
                {
                    attributes.Add(new ApiVersionNeutralAttribute());
                }
            }
            //当Controller未增加ApiController属性时，自动增加
            if (!controller.ControllerType.IsDefined(typeof(ApiControllerAttribute)))
            {
                if (controller.Attributes is List<object>
                    attributes)
                {
                    attributes.Add(new ApiControllerAttribute());
                }
            }
        }
    }
}
