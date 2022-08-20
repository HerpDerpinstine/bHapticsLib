using System.Threading;

namespace bHapticsLib.Internal
{
    /// <summary>
    /// Handles Multi-Threaded Tasking
    /// </summary>
    public abstract class ThreadedTask
    {
        private Thread thread;

        internal bool IsAlive()
            => thread?.IsAlive ?? false;

        /// <summary>
        /// Initializes the Task's Thread
        /// </summary>
        /// <returns>
        /// If the initialization was successful or not.
        /// </returns>
        public bool BeginInit()
        {
            if (!BeginInitInternal())
                return false;
            RunThread();
            return true;
        }
        internal abstract bool BeginInitInternal();

        /// <summary>
        /// Aborts the Task's Thread
        /// </summary>
        /// <returns>
        /// If the abortion was successful or not.
        /// </returns>
        public bool EndInit()
        {
            if (!EndInitInternal())
                return false;
            KillThread();
            return true;
        }
        internal abstract bool EndInitInternal();

        internal abstract void WithinThread();
        private void RunThread()
        {
            if (IsAlive())
                KillThread();

            thread = new Thread(WithinThread);
            thread.Start();
        }
        private void KillThread()
        {
            if (!IsAlive())
                return;
#pragma warning disable SYSLIB0006
            thread.Abort();
#pragma warning restore SYSLIB0006
            thread = null;
        }
    }
}
