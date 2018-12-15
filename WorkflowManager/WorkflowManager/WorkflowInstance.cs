using System;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowInstance
    {
        public WorkflowInstance(string workflowName,IWorkflowInstanceHandeler workflowInstanceHandeler=null,
            WorkflowApplication workflowApplicationHost=null)
        {
            this.WorkflowName = workflowName;
            SetApplicationhost(workflowApplicationHost);
            SetWorkflowInstanceHandeler(workflowInstanceHandeler);
            
        }
       
        

        public void SetBookMarks(List<string> bookmarks)
        {
            this.Bookmarks = bookmarks;
        }
        public void SetApplicationhost(WorkflowApplication application)
        {
            this.WorkflowApplicationInstance = application;
        }
        public void SetWorkflowInstanceHandeler(IWorkflowInstanceHandeler workflowInstanceHandeler)
        {
            if (workflowInstanceHandeler != null)
            {
                this.WorkflowInstanceHandeler = workflowInstanceHandeler;
                this.WorkflowApplicationInstance.Idle = workflowInstanceHandeler.OnIdle;
                this.WorkflowApplicationInstance.OnUnhandledException = workflowInstanceHandeler.OnUnhandledException;
                this.WorkflowApplicationInstance.Unloaded = workflowInstanceHandeler.OnUnloaded;
                this.WorkflowApplicationInstance.Aborted = workflowInstanceHandeler.OnAborted;
                this.WorkflowApplicationInstance.Completed = workflowInstanceHandeler.OnCompleted;
            }
        }
        public IWorkflowInstanceHandeler WorkflowInstanceHandeler { get; private set; }
        public Guid InstanceId { get; set; }
        public string WorkflowName { get; set; }
        public WorkflowApplication WorkflowApplicationInstance { get;private  set; }
        public List<string> Bookmarks { get; private set; }
        public InstanceState State { get; set; }
    }
}
