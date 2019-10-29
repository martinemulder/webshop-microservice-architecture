using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HalfWerk.FeWebshopUiTests.Spec
{
    public class SpecFlowTestLock
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
