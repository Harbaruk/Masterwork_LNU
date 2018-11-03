using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starter.API.Attributes;
using Starter.Common.DomainTaskStatus;

namespace Starter.API.Controllers
{
    /// <summary>
    /// Base controller with model validation and error handling
    /// </summary>
    /// <see cref="ErrorHandleAttribute"/>
    /// <seealso cref="ValidateModelAttribute"/>
    [ServiceFilter(typeof(ErrorHandleAttribute))]
    [ServiceFilter(typeof(ValidateModelAttribute))]
    public abstract class AbstractController : Controller
    {
        private readonly DomainTaskStatus _taskStatus;

        public AbstractController(DomainTaskStatus taskStatus)
        {
            _taskStatus = taskStatus;
        }
    }
}