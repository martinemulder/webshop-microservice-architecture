using System.Threading;

namespace HalfWerk.DsKlantBeheer.Spec
{
    public static class SpecFlowTestLock
    {
        // Force single threaded execution
        private static readonly Mutex _mutex = new Mutex();

        public static void Lock()
        {
            _mutex.WaitOne();
        }

        public static void Unlock()
        {
            _mutex.ReleaseMutex();
        }
    }
}
