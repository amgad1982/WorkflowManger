using System;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowInstance
    {
        public WorkflowInstance(IWorkflowInstanceHandeler workflowInstanceHandeler,
            WorkflowApplication workflowApplicationHost,string workflowName)
        {
            this.WorkflowName = workflowName;
            this.WorkflowInstanceHandeler = workflowInstanceHandeler;
            this.WorkflowApplicationInstance = workflowApplicationHost;
            this.WorkflowApplicationInstance.Idle = workflowInstanceHandeler.OnIdle;
            this.WorkflowApplicationInstance.OnUnhandledException = workflowInstanceHandeler.OnUnhandledException;
            this.WorkflowApplicationInstance.Unloaded = workflowInstanceHandeler.OnUnloaded;
            this.WorkflowApplicationInstance.Aborted = workflowInstanceHandeler.OnAborted;
            this.WorkflowApplicationInstance.Completed = workflowInstanceHandeler.OnCompleted;
           
        }
       
        


        public IWorkflowInstanceHandeler WorkflowInstanceHandeler { get; }
        public Guid InstanceId { get; set; }
        public string WorkflowName { get; set; }
        public WorkflowApplication WorkflowApplicationInstance { get;private  set; }
        public List<Bookmark> Bookmarks { get; private set; }
        public InstanceState State { get; set; }
    }
}
