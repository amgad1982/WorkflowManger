using System;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowInstance
    {
        public WorkflowInstance(bool isPersistable,bool isTrackable,IWorkflowInstanceHandeler workflowInstanceHandeler,
            WorkflowApplication workflowApplicationHost,string workflowName,IInstanceRepository instanceRepository)
        {
            this.WorkflowName = workflowName;
            this.IsPersistable = IsPersistable;
            this.IsTackable = isTrackable;
            this.WorkflowInstanceHandeler = workflowInstanceHandeler;
            this.WorkflowApplicationInstance = workflowApplicationHost;
            this.WorkflowApplicationInstance.Idle = workflowInstanceHandeler.OnIdle;
            this.WorkflowApplicationInstance.OnUnhandledException = workflowInstanceHandeler.OnUnhandledException;
            this.WorkflowApplicationInstance.Unloaded = workflowInstanceHandeler.OnUnloaded;
            this.WorkflowApplicationInstance.Aborted = workflowInstanceHandeler.OnAborted;
            this.WorkflowApplicationInstance.Completed = workflowInstanceHandeler.OnCompleted;

        }
       
        public IWorkflowInstanceHandeler WorkflowInstanceHandeler { get; }
        public bool IsPersistable { get; set; }
        public bool IsTackable { get; set; }
        public Guid InstanceId { get; set; }
        public string WorkflowName { get; set; }
        public WorkflowApplication WorkflowApplicationInstance { get;private  set; }
        public List<Bookmark> Bookmarks { get; private set; }
        public WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
