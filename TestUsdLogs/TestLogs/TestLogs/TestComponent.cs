using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Csr;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestLogs
{
    public class TestComponent : DynamicsBaseHostedControl
    {
        public string FullTypeName { get; set; }


        private LoggerManager logger = null;
        public LoggerManager Logger
        {
            get
            {
                if (FullTypeName == null)
                {
                    logger =
                        new LoggerManager("NULL");
                }
                if (logger == null)
                {
                    logger =
                        new LoggerManager(FullTypeName);
                }
                return logger;
            }
            internal set
            {
                logger = value;
            }
        }


        public TestComponent(Guid appId, string appName, string appInit)
           : base(appId, appName, appInit)
        {

            System.Diagnostics.Debugger.Launch();
            Logger.Log("Inicializado componente");
        }


        protected override void DoAction(RequestActionEventArgs args)
        {
            if (args.Action.ToLower() == "log")
            {
                Logger.Log(args.Data);
            }
            base.DoAction(args);
        }


    }
}
