using System.Collections.Generic;

namespace Support.FloodFill
{
    /// <summary>
    /// Abstract class to facilitate FloodFill implementation.
    /// Your child class need to save its result state.
    /// Start from a id, check if a id 'IsAvailable', 'Consume' that id and spread to 'NeighborIds'
    /// NeighborIds are saved into a queue.
    /// </summary>
    public abstract class FloodFiller
    {
        protected readonly Queue<int> queue = new();
        public bool IsComplete => queue.Count == 0;
        public FloodFiller(in int firstId) => queue.Enqueue(firstId);
        /// <summary>
        /// Check if given id is available to be consumed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract bool IsAvailable(int id);
        /// <summary>
        /// Mark id as consumed, making it not available anymore.
        /// This change should be saved on a result state.
        /// </summary>
        /// <param name="id"></param>
        protected abstract void Consume(int id);
        /// <summary>
        /// Recognize all other ids that are linked/neighbors of the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract IEnumerable<int> NeighborIds(int id);
        public virtual void ExecuteOneStep()
        {
            for (var hasExecuted = false; !hasExecuted && queue.TryDequeue(out var id);)
            {
                if (IsAvailable(id))
                {
                    Consume(id);
                    hasExecuted = true;
                    foreach (var otherId in NeighborIds(id))
                    {
                        if (IsAvailable(otherId))
                        {
                            queue.Enqueue(otherId);
                        }
                    }
                }
            }
        }
        public void ExecuteUntilComplete() { while (!IsComplete) { ExecuteOneStep(); } }
        public static void ExecuteManyUntilComplete(in IReadOnlyList<FloodFiller> floodFills)
        {
            for (var allComplete = false; !allComplete;)
            {
                allComplete = true;
                for (var i = 0; i < floodFills.Count; i++)
                {
                    floodFills[i].ExecuteOneStep();
                    if (!floodFills[i].IsComplete) { allComplete = false; }
                }
            }
        }
    }
}
