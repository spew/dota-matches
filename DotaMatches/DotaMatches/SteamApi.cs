using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DotaMatches
{
    public enum EGCResults
    {
        k_EGCResultOK = 0,
        k_EGCResultNoMessage = 1,			// There is no message in the queue
        k_EGCResultBufferTooSmall = 2,		// The buffer is too small for the requested message
        k_EGCResultNotLoggedOn = 3,			// The client is not logged onto Steam
        k_EGCResultInvalidMessage = 4,		// Something was wrong with the message being sent with SendMessage
    }

    /// <summary>
    /// Class to interop with the c++ dll, in theory we could replace this with a managed c++ class
    /// and make things "simpler". This works for now though.
    /// </summary>
    public static class SteamApi
    {
        private static class DllFunctions
        {
            [DllImport("steam_api_wrapper.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte Initialize();

            [DllImport("steam_api_wrapper.dll", EntryPoint = "Shutdown", CallingConvention = CallingConvention.Cdecl)]
            public static extern void Shutdown();

            [DllImport("steam_api_wrapper.dll", EntryPoint = "SendMessage", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SendMessage(UInt32 unMsgType, byte[] data, UInt32 dataLength);

            [DllImport("steam_api_wrapper.dll", EntryPoint = "IsMessageAvailable", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte IsMessageAvailable(ref UInt32 msgSize);

            [DllImport("steam_api_wrapper.dll", EntryPoint = "RetrieveMessage", CallingConvention = CallingConvention.Cdecl)]
            public static extern int RetrieveMessage(ref UInt32 msgType, byte[] dest, UInt32 destLength, ref UInt32 msgSize);
        }

        public static bool Initialize()
        {
            byte result = DllFunctions.Initialize();
            return result != 0;
        }

        public static void Shutdown()
        {
            DllFunctions.Shutdown();
        }

        public static EGCResults SendMessage(UInt32 msgType, byte[] data)
        {
            int result = DllFunctions.SendMessage(msgType, data, Convert.ToUInt32(data.Length));
            return (EGCResults)result;
        }

        public static bool IsMessageAvailable(ref uint msgSize)
        {
            unsafe
            {
                int result = DllFunctions.IsMessageAvailable(ref msgSize);
                return result != 0;
            }
        }

        public static EGCResults RetrieveMessage(ref UInt32 msgType, byte[] dest, ref UInt32 msgSize)
        {
            int result = DllFunctions.RetrieveMessage(ref msgType, dest, Convert.ToUInt32(dest.Length), ref msgSize);
            return (EGCResults)result;
        }
    }
}
