
namespace FinalProject.Helpers
{
    public static class GetCurrentLanguage
    {
        public static string CurrentLanguage(HttpContext httpContext)
        {
            string cookieName = ".AspNetCore.Culture";
            string language = "az";
            if (httpContext.Request.Cookies.TryGetValue(cookieName, out string value))
            {
                if (value.Contains("az"))
                {
                    language = "az";
                }
                else
                {
                    language = "en";
                }
            }
            else
            {
                language = "az";
            }
            return language;
        }
    }
}
