using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLGV_DTSoft.Data;
using System.Security.Claims;

namespace QLGV_DTSoft.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int _idquyen;

        public CustomAuthorizeAttribute(int idquyen)
        {
            _idquyen = idquyen;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _context = context.HttpContext.RequestServices.GetService<DtsoftContext>();

            if (!context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var idVtClaim = context.HttpContext.User.FindFirstValue("idvaitro");
            var idVt = int.Parse(idVtClaim);

            var hasPermission = _context.CoQuyenTruyCaps
                .Any(qt => qt.IdVt == idVt && qt.IdQuyen == _idquyen);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }


}
