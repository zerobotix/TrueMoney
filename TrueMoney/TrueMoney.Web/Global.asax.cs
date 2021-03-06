﻿using System;
using System.Data.Entity;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;
using log4net.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TrueMoney.Common;
using TrueMoney.DependencyInjection;
using TrueMoney.Services.Mapping;
using TrueMoney.Web.Auth_Identity_Startup;

namespace TrueMoney.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            //var logger = DependencyResolver.Current.GetService<ILog>();
            var logger = LogManager.GetLogger("TrueMoney.Logger");
            logger.Error(exception.Message, exception);

#if !DEBUG
            Server.ClearError();
            Response.Clear();

            var httpException = exception as HttpException;
            var errorCode = httpException == null ? (int)HttpStatusCode.InternalServerError : httpException.GetHttpCode();

            //var url = $"/Error/Code/{errorCode}";

            var url = $"/Error/Code?id={errorCode}&exceptionMessage={WebUtility.UrlEncode(exception.Message)}";

            Server.TransferRequest(url);

            //Server.TransferRequest("/Error/Code/404");
            //Context.RewritePath("~/Views/Error/404.cshtml");
            //HttpContext.Current.RewritePath("somefile.aspx");
            //Server.Transfer("~/Views/Error/404.cshtml");
            //Response.StatusCode = (int)HttpStatusCode.NotFound;

            //System.Diagnostics.Debug.WriteLine(exception);
            //Response.Redirect("/Home/Error");\
#endif
        }
    }
}
