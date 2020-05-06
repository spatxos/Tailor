using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Service
{
    public interface IBaseController
    {
        public bool IsOverride()
        {
            StackTrace ss = new StackTrace(true);
            MethodBase mb = ss.GetFrame(1).GetMethod();
            return !(GetType().GetMethod(mb.Name).DeclaringType == mb.DeclaringType);
        }
    }
}
