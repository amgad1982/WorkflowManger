using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager
{
    public interface IInstanceRepository
    {
        void SaveWorkflowInstanceState(Guid instanceId,string workflowName, InstanceState instanceStatus, string instanceNextBookMarks);
        void UpdateWorkflowInstanceState(Guid instanceId, InstanceState instanceStatus, string instanceNextBookMarks);
    }
}
