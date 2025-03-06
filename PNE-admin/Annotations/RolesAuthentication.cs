using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Protocol;

namespace PNE_admin.Annotations
{
    public class RolesAuthentication : ActionFilterAttribute
    {
        //roles recu qui devront etre present pour passer le check
        private readonly string[] _RolesToCheck;

        public RolesAuthentication(params string[] dataToVerify)
        {
            _RolesToCheck = dataToVerify;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            //get les roles d'une variable d'environnement
            var serializedRoles = context.HttpContext.Session.GetString("userRoles");
            if (serializedRoles != null)
            {
                List<string> roles = serializedRoles.FromJson<List<string>>();
                if (!IsUserAuthorized(roles, _RolesToCheck))
                {
                    context.Result = new BadRequestObjectResult("Action non-authorise");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Aucune Authorisation detecte");
                return;
            }
            base.OnActionExecuting(context);
        }

        private static bool IsUserAuthorized(List<string> userRoles, string[] requiredRoles)
        {
            foreach (var reqRole in requiredRoles){
                if (!userRoles.Where(x => x == reqRole).Any())
                    return false;
            }
            return true;
        }
    }
}
