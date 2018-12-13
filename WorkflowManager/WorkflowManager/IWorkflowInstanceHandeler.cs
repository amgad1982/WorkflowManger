using System.Activities;

namespace WorkflowManager
{
    public interface IWorkflowInstanceHandeler
    {
        void OnAborted(WorkflowApplicationAbortedEventArgs e);

        void OnCompleted(WorkflowApplicationCompletedEventArgs e);
        void OnUnloaded(WorkflowApplicationEventArgs e);
        UnhandledExceptionAction OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e);

        void OnIdle(WorkflowApplicationIdleEventArgs e);
    }
}
