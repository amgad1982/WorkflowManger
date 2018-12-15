using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class WorkFlowCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid InstanceId { get; set; }
    }
    public class WorkflowCommandOutMessage
    {
        public string Message { get; set; }
    }
}