using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
namespace FinalProject.Areas.StoreManage.ViewComponents
{
    public class StoreManageHeaderViewComponent : ViewComponent
    {
        private readonly Database _context;
        public StoreManageHeaderViewComponent(Database context)
        {
            _context = context;
          
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            
            return View(await Task.FromResult(store));
        }
    }
}
