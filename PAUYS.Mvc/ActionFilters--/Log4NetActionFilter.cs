using log4net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PAUYS.Mvc.ActionFilters__
{
    public class Log4NetActionFilter : ActionFilterAttribute
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Log4NetActionFilter));

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Action başlatılmadan önce loglama
            var controllerName = filterContext.Controller.ToString();
            var actionName = filterContext.ActionDescriptor.DisplayName;
            var user = filterContext.HttpContext.User.Identity.Name;

            logger.Info($"User {user} started {actionName} action in {controllerName} controller at {DateTime.Now}");
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Action tamamlandıktan sonra loglama
            var controllerName = filterContext.Controller.ToString();
            var actionName = filterContext.ActionDescriptor.DisplayName;

            if (filterContext.Exception != null)
            {
                // Exception loglama
                logger.Error($"Exception in {actionName} action of {controllerName}: {filterContext.Exception.Message}", filterContext.Exception);
            }
            else
            {
                // Normal akış
                logger.Info($"{actionName} action in {controllerName} executed successfully at {DateTime.Now}");
            }

            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            // Action'ın sonuç döndürülmesinden sonra loglama
            logger.Info($"Result for Action {filterContext.RouteData.Values["action"]} executed at {DateTime.Now}");

            base.OnResultExecuted(filterContext);
        }
    }
}
