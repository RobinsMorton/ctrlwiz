using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVTXBoxCtr
{
    public class ExAppViewpoint : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= ControlledApplication_DocumentOpened;
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
