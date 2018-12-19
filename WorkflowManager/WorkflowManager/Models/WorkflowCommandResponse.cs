using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager.Models
{
    public class WorkflowCommandResponse
    {
        public List<Parameter> OutParameters { get; set; }
        public Guid InstanceId { get; set; }
        public List<String> NextWorkflowInstanceActions { get; set; }
    }
}
