using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomMovie.Extensions
{
    internal static class TaskExtensions
    {
        public static async void FireAndForgetSafeAsync(this Task task, ILogger logger = null, bool continueOnCapturedContext = true)
        {
            try
            {
                if (continueOnCapturedContext)
                {
                    await task.ConfigureAwait(true);
                }
                else
                {
                    await task.ConfigureAwait(false);
                }
            }
#pragma warning disable CA1031 // Intentional: Exceptions should only be reported to prevent app crash.
            catch (Exception e)
#pragma warning restore CA1031
            {
                logger?.LogError(e, "Error in async/void function.");

#if DEBUG
                if (logger == null)
                {
                    Debug.Write(e.ToString());
                    Debug.Fail("Error in async/void function.");
                    Debugger.Break();
                }
#endif
            }
        }
    }
}
