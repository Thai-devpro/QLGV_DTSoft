using System.Security.Claims;

namespace QLGV_DTSoft.Helper
{
    public class UserHelper
    {
        public static int? GetLoggedInUserKhuvucId(ClaimsPrincipal user)
        {
            var khuvucIdClaim = user.FindFirstValue("idKhuvuc");
            if (!string.IsNullOrEmpty(khuvucIdClaim) && int.TryParse(khuvucIdClaim, out int khuvucId))
            {
                return khuvucId;
            }
            return null;
        }
    }
}
