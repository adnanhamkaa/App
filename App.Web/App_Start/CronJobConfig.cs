using App.Web;
using App.Web.Services.Contracts;
using Microsoft.Owin;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(Startup))]
namespace App.Web {
    
    public partial class Startup {
        public void StartCronJobs() {

        }

        public void StartJob<T>() where T : IJobService {
            try {
                DependencyResolver.Current.GetService<T>()?.StartWorker();
            } catch (Exception exc) {
                
            }
            
        }
    }
}