using System.Web;
using System.Web.Mvc;

namespace _420_476.Devoir3.Samuel.Octeau
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
