using System.Threading;

namespace bHapticsLib.Internal
{
    public abstract class ThreadedTask
    {
        private Thread thread;

        internal bool IsAlive()
            => thread?.IsAlive ?? false;

        internal abstract bool BeginInitInternal();
        public bool BeginInit()
        {
            if (!BeginInitInternal())
                return false;
            RunThread();
            return true;
        }

        internal abstract bool EndInitInternal();
        public bool EndInit()
        {
            if (!EndInitInternal())
                return false;
            KillThread();
            return true;
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
