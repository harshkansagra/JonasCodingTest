using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid && ShouldValidate(actionContext))
            {
                var res = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                actionContext.Response = res;
            }
        }


        private bool ShouldValidate(HttpActionContext actionContext)
        {
            // Check if the action has the ValidateModelAttribute
            bool hasAttributeOnAction = actionContext.ActionDescriptor.GetCustomAttributes<ValidateModelAttribute>().Any();

            // Validation should occur if either the action or controller has the attribute
            return hasAttributeOnAction;
        }
    }
}