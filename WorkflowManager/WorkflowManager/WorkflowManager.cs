using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.DurableInstancing;
using System.Threading;

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
        IInstanceRepository _repository;
        public WorkflowManager(string workflowRepositoryPath,bool persitable,bool trackable,ILogger infoLogger,ILogger errorLogger)
        {
            this._managedWorkflows = new List<WorkflowInstance>();
            this._workflowRepositoryPath = workflowRepositoryPath;
            this._infoLogger = infoLogger;
            this._errorLogger = errorLogger;
            this._isPersistable = persitable;
            this._isTrackable = trackable;
            this._connectionString = ConfigurationManager.ConnectionStrings["workflowconnection"].ConnectionString;
            this._repository = new InstanceRepository(this._connectionString, this._workflowRepositoryPath, this._errorLogger);
        }
        public List<string> GetBookMarks(Guid workflowInstanceId)
        {
            return _managedWorkflows.FirstOrDefault(wf => wf.InstanceId == workflowInstanceId)?.Bookmarks;
        }

        public List<WorkflowInstance> GetWorkflows(Action<WorkflowInstance> filter = null)
        {
            //determined upon requests.
            throw new NotImplementedException();
        }
        public WorkflowInstance LoadWorkflow(Guid instanceId)
        {
            var instance = this._repository.LoadWorkflowInstance(instanceId);
            var activity = Helpers.LoadWorkflowActiovityFromXaml(this._workflowRepositoryPath + instance.WorkflowName, this._errorLogger);
            if (activity != null)
            {
                WorkflowApplication app = new WorkflowApplication(activity);
                app.Extensions.Add(() =>
                {
                    TrackingParticipant participant = new LoggerTrackingParticipant(this._infoLogger);
                    participant.TrackingProfile = CreateTrackingProfile();
                    return participant;
                });
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
                    app.PersistableIdle = (e) => { return PersistableIdleAction.Persist; };

                }
                try
                {
                    app.Load(instanceId);
                    app.Run();
                    
                    instance.SetApplicationhost(app);
                    instance.SetWorkflowInstanceHandeler(this);
                   
                    instance.State = InstanceState.Loaded;
                    instance.InstanceId = instanceId;
                    this._managedWorkflows.Add(instance);
                    this._repository.UpdateWorkflowInstanceState(app.Id, InstanceState.Loaded, instance.Bookmarks.ConvertStringListToCommaSeparatedString());
                    
                    return instance;
                }
                catch (Exception ex)
                {
                    this._errorLogger.Log(ex.Message, LoggerInfoTypes.Error);
                }
              
            }
            return null;
        }

        public WorkflowInstance LoadWorkFlowWithBookMarkResume(Guid instanceId, string bookmarkName,object value)
        {
            var instance = this.LoadWorkflow(instanceId);
            
            instance.ExecutedBookMarkResult =instance.WorkflowApplicationInstance.ResumeBookmark(bookmarkName, value);
            return instance;
        }

        public Guid NewWorkFlow(string workflowName, IDictionary<string, object> parameters)
        {
            var activity = Helpers.LoadWorkflowActiovityFromXaml(this._workflowRepositoryPath + workflowName, this._errorLogger);
            if (activity != null)
            {
                WorkflowApplication app = new WorkflowApplication(activity);
                app.Extensions.Add(() =>
                {
                    TrackingParticipant participant = new LoggerTrackingParticipant(this._infoLogger);
                     participant.TrackingProfile = CreateTrackingProfile();
                    return participant;
                });
               
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
                    app.PersistableIdle = (e) => { return PersistableIdleAction.Persist; };
                    
                }
                try
                {
                    app.Run();
                    var wfinstance = new WorkflowInstance(workflowName,this, app);
                    wfinstance.State = InstanceState.Created;
                    wfinstance.InstanceId = app.Id;
                    this._managedWorkflows.Add(wfinstance);
                    this._repository.SaveWorkflowInstanceState(app.Id, workflowName, InstanceState.Created, string.Empty);
                    return app.Id;
                }
                catch(Exception ex)
                {
                    this._errorLogger.Log(ex.Message, LoggerInfoTypes.Error);
                }
                
            }
            return Guid.Empty;
        }
        TrackingProfile CreateTrackingProfile()
        {
            TrackingProfile trackingProfile = new TrackingProfile()
            {
                Name = "CustomTrackingProfile",
                Queries =
                {
                    new CustomTrackingQuery()
                    {
                        Name = "*",
                        ActivityName = "*"
                    },
                    new WorkflowInstanceQuery()
                    {
                        States = {"*"}
                    }
                }
            };

            if (this._isTrackable)
            {
                trackingProfile.Queries.Add(
                    new ActivityStateQuery()
                    {

                        ActivityName = "*",
                        States = { "*" },
                        Variables = { "*" }
                    });
                trackingProfile.Queries.Add(
                    new ActivityScheduledQuery() { ChildActivityName = "*" });
            }

            return trackingProfile;
        }
        public void OnAborted(WorkflowApplicationAbortedEventArgs e)
        {
            this._repository.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Aborted, string.Empty);
        }

        public void OnCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            this._repository.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Completed, string.Empty);
        }

        public void OnIdle(WorkflowApplicationIdleEventArgs e)
        {
            this._managedWorkflows.First(wf => wf.InstanceId == e.InstanceId).SetBookMarks( e.Bookmarks.Select(b => b.BookmarkName).ToList());
            this._repository.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Idle, e.Bookmarks.ConvertBookmakListToCommaSeparatedString());
        }

        public UnhandledExceptionAction OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            this._errorLogger.Log("istance id:" + e.InstanceId + " has exception :" + e.UnhandledException.Message, LoggerInfoTypes.Error);
            return UnhandledExceptionAction.Abort;
            
        }

        public void OnUnloaded(WorkflowApplicationEventArgs e)
        {
            //this._repository.UpdateWorkflowInstanceState(e.InstanceId, InstanceState.Unloaded, this._managedWorkflows.First(wf => wf.InstanceId == e.InstanceId).Bookmarks.ConvertStringListToCommaSeparatedString());
        }

       

        public void Terminate(Guid instanceId)
        {
            //future work terminate list of workflows, we need to add loadworkflows by list of ids.
            this._managedWorkflows.First(wf => wf.InstanceId == instanceId).WorkflowApplicationInstance.Terminate("terminated by user");
            this._repository.UpdateWorkflowInstanceState(instanceId, InstanceState.Terminated, string.Empty);
        }

      
    }
}
