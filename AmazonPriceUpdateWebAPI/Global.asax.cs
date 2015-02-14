using AmazonPriceUpdateWebAPI.ScheduledJobUtils;
using PriceUpdateWebAPIBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AmazonPriceUpdateWebAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            PriceUpdateContext.Initialize(APIConfigManager.AWSAccessKey, APIConfigManager.AWSSecretKey,APIConfigManager.ServiceAccountId,APIConfigManager.ServiceAccountPassword, "smtp.gmail.com", "Automated Mail", APIConfigManager.TablesToCreate);

            //Make endpoint and timeout configurable
            HeartbeatManager hbManager = new HeartbeatManager(@"http://localhost:59132/api/amazonproductapi", new TimeSpan(0,1,0));
            Task hbTask = Task.Factory.StartNew(hbManager.CheckServiceHeartbeat, TaskCreationOptions.LongRunning);
            JobScheduler.Start();
        }
    }
}