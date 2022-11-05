using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Modules;
using OTP.Ring.Business;
using OTP.Ring.Common;

namespace OTP.Ring.Web.Utility
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        // A Ninject "kernel" is the thing that can supply object instances
        private IKernel kernel = new StandardKernel(new RingBLServices());

        // ASP.NET MVC calls this to get the controller for each request
        protected override IController GetControllerInstance(RequestContext context, Type controllerType)
        {
            if (controllerType == null) return null;
            return (IController) kernel.Get(controllerType);
        }

       // Configures how abstract service types are mapped to concrete implementations
       private class RingBLServices : NinjectModule
      {

           public override void Load()
           {
               //Due to language code is set per request, only use InTransientScope or InRequestScope
               string parameterName = "connectionString";
               string parameterNameReport = "reportConnectionString";

               string database = ExtensionMethods.GetConfigurationSetting(Constant.DataConnectionString);
               string report = ExtensionMethods.GetConfigurationSetting(Constant.ReportConnectionString);

               Bind<IActionItemBL>()
                 .To<ActionItemBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IBenchmarkBL>()
                 .To<BenchmarkBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<ICommentsBL>()
                 .To<CommentsBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IDecodeBL>()
                 .To<DecodeBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IFundingBL>()
                 .To<FundingBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IReportBL>()
                 .To<ReportBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database)
                 .WithConstructorArgument(parameterNameReport, report);

               Bind<IResourceBL>()
                 .To<ResourceBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<ISportBL>()
                 .To<SportBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IUserBL>()
                 .To<UserBL>()
                 .InRequestScope()
                 .WithConstructorArgument(parameterName, database);

               Bind<IEvaluationsBL>()
                  .To<EvaluationsBL>()
                  .InRequestScope()
                  .WithConstructorArgument(parameterName, database);

               Bind<IResultsBL>()
                   .To<ResultsBL>()
                   .InRequestScope()
                   .WithConstructorArgument(parameterName, database);
           }
       }
    }
}