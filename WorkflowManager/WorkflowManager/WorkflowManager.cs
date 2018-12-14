using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.DurableInstancing;

namespace WorkflowManager
{
    public class WorkflowManager : IWorkflowManager,IWorkflowInstanceHandeler
    {
        List< WorkflowInstance> _managedWorkflows;
        ILogger _infoLogger;
        ILogger _errorLogger;
        bool _isPersistable;
        bool _isTrackable;
        string _workflowRepositoryPath;
        string _connectionString;
        IInstanceRepository _repositor;
        public WorkflowManager(string workflowRepositoryPath,bool persitable,bool trackable,ILogger infoLogger,ILogger errorLogger)
        {
            this._managedWorkflows = new List<WorkflowInstance>();
            this._workflowRepositoryPath = workflowRepositoryPath;
            this._infoLogger = infoLogger;
            this._errorLogger = errorLogger;
            this._isPersistable = persitable;
            this._isTrackable = trackable;
            this._connectionString = ConfigurationManager.ConnectionStrings["workflowconnection"].ConnectionString;
            this._repositor = new InstanceRepository(this._connectionString, this._workflowRepositoryPath, this._errorLogger);
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
            var activity = Helpers.LoadWorkflowActiovityFromXaml(this._workflowRepositoryPath + workflowName, this._errorLogger);
            if (activity != null)
            {
                WorkflowApplication app = new WorkflowApplication(activity);

                if (this._isPersistable)
                {
                    //setup persistence
                    InstanceStore store = new SqlWorkflowInstanceStore(
                     this._connectionString);
                    InstanceHandle handle = store.CreateInstanceHandle();
                    InstanceView view = store.Execute(handle,
                      new CreateWorkflowOwnerCommand(), TimeSpan.FromSeconds(30));
                    handle.Free();
                    store.DefaultInstanceOwner = view.InstanceOwner;
                    app.InstanceStore = store;
                    app.PersistableIdle = (e) => { return PersistableIdleAction.Unload; };
                    
                }
                try
                {
                    app.Run();
                    var wfinstance = new WorkflowInstance(this, app, workflowName);
                    wfinstance.State = InstanceState.Created;
                    this._repositor.SaveWorkflowInstanceState(app.Id, workflowName, InstanceState.Created, string.Empty);
                    this._managedWorkflows.Add(wfinstance);
                    return app.Id;
                }
                catch(Exception ex)
                {
                    this._errorLogger.Log(ex.Message, LoggerInfoTypes.Error);
                }
                
            }
            return Guid.Empty;
        }

        public void OnAborted(WorkflowApplicationAbortedEventArgs e)
        {
            this._repositor.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Aborted, string.Empty);
        }

        public void OnCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            this._repositor.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Completed, string.Empty);
        }

        public void OnIdle(WorkflowApplicationIdleEventArgs e)
        {
            
            this._repositor.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Idle, string.Empty);
        }

        public UnhandledExceptionAction OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            this._errorLogger.Log("istance id:" + e.InstanceId + " has exception :" + e.UnhandledException.Message, LoggerInfoTypes.Error);
            return UnhandledExceptionAction.Abort;
        }

        public void OnUnloaded(WorkflowApplicationEventArgs e)
        {
            this._repositor.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Unloaded, string.Empty);
        }

        public void Terminate(Guid instanceId)
        {
            //future work terminate list of workflows, we need to add loadworkflows by list of ids.
            this._managedWorkflows.First(wf => wf.InstanceId == instanceId).WorkflowApplicationInstance.Terminate("terminated by user");
            this._repositor.UpdateWorkflowInstanceState(instanceId, InstanceState.Terminated, string.Empty);
        }

      
    }
}
