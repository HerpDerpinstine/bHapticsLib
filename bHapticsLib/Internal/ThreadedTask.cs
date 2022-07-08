using System.Threading;

namespace bHapticsLib.Internal
{
    internal abstract class ThreadedTask
    {
        private Thread thread;

        internal bool IsAlive()
            => (thread == null) ? false : thread.IsAlive;

        internal abstract bool BeginInitInternal();
        internal void BeginInit()
        {
            if (BeginInitInternal())
                RunThread();
        }

        internal abstract bool EndInitInternal();
        internal void EndInit()
        {
            if (EndInitInternal())
                KillThread();
        }

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

            thread.Abort();
            thread = null;
        }
    }
}
