﻿using System;
using System.Collections.Generic;

namespace WorkflowManager
{
    public interface IWorkflowManager
    {
        Guid NewWorkFlow(string workflowName, IDictionary<string, object> parameters);
        List<WorkflowInstance> GetWorkflows(Action<WorkflowInstance> filter = null);
        List<string> GetBookMarks(Guid workflowInstanceId);
        WorkflowInstance LoadWorkflow(Guid instanceId);
        void Terminate(Guid instanceId);
        WorkflowInstance LoadWorkFlowWithBookMarkResume(Guid instanceId, string bookmarkName,object value);
    }
}
