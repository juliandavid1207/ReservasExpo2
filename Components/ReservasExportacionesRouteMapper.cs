using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Components
{
    public class ReservasExportacionesRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("ReservasExportaciones", "Default", "{controller}/{action}",
                                     new[] { "AllNet.Modules.ReservasExportaciones.Components.Controllers" });
        }
    }
}