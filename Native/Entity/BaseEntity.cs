using Newtonsoft.Json;
using System;

namespace Entity
{
    [Serializable]
    public class BaseEntity
    {
        public override string ToString()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch
            {
                return "";
            }
        }
    }
}
