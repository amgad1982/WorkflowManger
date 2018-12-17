using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager.Models
{
    public class WorkflowCommand
    {
        public List<Parameter> Parameters { get; set; }
        public WorkflowCommandType Command { get; set; }
        public ActionParameter Action { get; set; }
        public Guid WorkflowInstanceID { get; set; }

    }

    public class ActionParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
