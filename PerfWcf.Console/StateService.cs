using System;
using Serilog;

namespace PerfWcf.Console
{
    public class StateService
    {
        public static event StateLocalSessionEventHandler State_DirectSessionRequested;

        public static void SpawnDirectSession(string WebSessionId, string UserIdentifier, int UserId, bool IsDialerSession, bool IsNativeSfdcSession, bool IsCOLV, int SessionId = 0)
        {
            try
            {
                if (State_DirectSessionRequested != null)
                {
                    StateSessionEventArgs e = new StateSessionEventArgs(UserIdentifier, UserId, IsDialerSession, IsNativeSfdcSession, IsCOLV, SessionId);
                    State_DirectSessionRequested(WebSessionId, e);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SpawnDirectSession");
            }
        }
    }
}
