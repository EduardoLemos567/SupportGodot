using Support.Diagnostics;

namespace Support;

/// <summary>
/// Describes the status of a process.
/// Its controlled by the inner class Control.
/// </summary>
public class ProcessStatus
{
    /// <summary>
    /// Controller used to change status from ProcessStatus it possesses.
    /// </summary>
    public class Controller
    {
        public ProcessStatus Status { get; private set; }
        public Controller(ProcessStatus processStatus) => Status = processStatus;
        public void Started()
        {
            Debug.Assert(!Status.HasStarted, "Should not start twice (was started before).");
            Status.HasStarted = true;
        }
        public void Completed(bool withErrors = false, string? errorDetails = null)
        {
            Debug.Assert(!Status.IsCompleted, "Should not complete twice (was completed before).");
            Status.IsCompleted = true;
            Status.HasErrors = withErrors;
            Status.ErrorDetails = errorDetails;
        }
    }
    private ProcessStatus() { }
    /// <summary>
    /// Has starting the process, if not completed is considered pendent.
    /// </summary>
    public bool HasStarted { get; private set; }
    /// <summary>
    /// Is pending to finish, if has started and not completed.
    /// </summary>
    public bool IsPending => HasStarted && !IsCompleted;
    /// <summary>
    /// Process has finished, should not change any status.
    /// </summary>
    public bool IsCompleted { get; private set; }
    public bool IsCompletedWithoutErrors => IsCompleted && !HasErrors;
    /// <summary>
    /// Completed with errors. Can or cannot have details.
    /// </summary>
    public bool HasErrors { get; private set; }
    /// <summary>
    /// Details for the error on completed.
    /// </summary>
    public string? ErrorDetails { get; private set; }
    public static Controller Create() => new(new());
}