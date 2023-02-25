using FinalProject.Helpers;

namespace FinalProject.Services
{
    public class LangService
    {
        private readonly HttpContext _httpContext;
        public LangService(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContext = httpContextAccessor.HttpContext;
        }
        public string GetLang()
        {
            return GetCurrentLanguage.CurrentLanguage(_httpContext);
        }
    }
}
