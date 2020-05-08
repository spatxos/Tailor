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
            //��Controllerδ���Ӱ汾���߲����ְ汾����ʱ���Զ����Ӳ����ְ汾����
            if (!(controller.ControllerType.IsDefined(typeof(ApiVersionAttribute)) || controller.ControllerType.IsDefined(typeof(ApiVersionNeutralAttribute))))
            {
                if (controller.Attributes is List<object>
                    attributes)
                {
                    attributes.Add(new ApiVersionNeutralAttribute());
                }
            }
            //��Controllerδ����ApiController����ʱ���Զ�����
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
