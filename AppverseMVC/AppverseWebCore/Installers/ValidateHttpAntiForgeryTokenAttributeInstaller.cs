using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Appverse.Web.Core.Controllers;
using Appverse.Web.Core.Attributes;
using System.Web.Http.Filters;

namespace Appverse.Web.Core.Installers
{
    public class ValidateHttpAntiForgeryTokenAttributeInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                .BasedOn<IFilter>().WithServiceBase()
                                .LifestyleTransient());


            //var a = container.Resolve<ValidateHttpAntiForgeryTokenAttribute>();
            //var b = container.Resolve<System.Web.Http.Filters.IFilterProvider>();            
            
            //container.Register(Classes.FromThisAssembly()
            //                    .BasedOn<IController>()
            //                    .LifestyleTransient());
        }
    }
}