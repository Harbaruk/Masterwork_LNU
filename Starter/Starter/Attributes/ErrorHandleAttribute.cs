using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Starter.Common.DomainTaskStatus;

namespace Starter.API.Attributes
{
    public class ErrorHandleAttribute : ActionFilterAttribute
    {
        private readonly DomainTaskStatus _domainTask;

        public ErrorHandleAttribute(DomainTaskStatus domainTask)
        {
            _domainTask = domainTask;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (_domainTask.HasErrors)
            {
                context.Result = new BadRequestObjectResult(_domainTask.ErrorCollection);
            }
        }
    }
}