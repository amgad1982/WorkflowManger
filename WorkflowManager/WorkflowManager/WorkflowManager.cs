using System;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowManager : IWorkflowManager,IWorkflowInstanceHandeler
    {
        IDictionary<Guid, WorkflowInstance> _managedWorkflows;
        ILogger _infoLogger;
        ILogger _errorLogger;
        bool _isPersistable;
        bool _isTrackable;
        public WorkflowManager(bool persitable,bool trackable,ILogger infoLogger,ILogger errorLogger)
        {
            this._managedWorkflows = new Dictionary<Guid, WorkflowInstance>();
            this._infoLogger = infoLogger;
            this._errorLogger = errorLogger;
            this._isPersistable = persitable;
            this._isTrackable = trackable;
        }
        public List<BookMark> GetBookMarks(Guid workflowInstanceId)
        {
            throw new NotImplementedException();
        }

        public List<WorkflowInstance> GetWorkflows(Action<WorkflowInstance> filter = null)
        {
            throw new NotImplementedException();
        }

        public WorkflowInstance LoadWorkflow(Guid instanceId)
        {
            throw new NotImplementedException();
        }

        public Guid NewWorkFlow(string workflowName, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public void OnAborted(WorkflowApplicationAbortedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnIdle(WorkflowApplicationIdleEventArgs e)
        {
            throw new NotImplementedException();
        }

        public UnhandledExceptionAction OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnUnloaded(WorkflowApplicationEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Terminate(Guid instanceId)
        {
            throw new NotImplementedException();
        }

        WorkflowInstance LoadWorkflowInstance()
        {

        }
    }
}
