using Newtonsoft.Json;

namespace rtd_priviledge.Utilities
{
    public class ConvertUtil
    {
        public static string obj2string(Object obj)
        {

            //return JsonConvert.SerializeObject(obj, Formatting.Indented);
            return JsonConvert.SerializeObject(obj, Formatting.None,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
        }
    }
}
