using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DotaMatches
{
    public class SteamMessenger
    {
        private void ClearMessageQueue()
        {
            uint msgSize = 0;
            while (SteamApi.IsMessageAvailable(ref msgSize))
            {
                byte[] buffer = new byte[msgSize];

                uint retMsgType = 0, retMsgSize = 0;
                SteamApi.RetrieveMessage(ref retMsgType, buffer, ref retMsgSize);
            }
        }

        public byte[][] SendMessagesAndWaitForReplies(uint msgType, IList<byte[]> datas)
        {
            this.ClearMessageQueue();
            Thread.Sleep(10);

            foreach (byte[] d in datas)
            {
                EGCResults result = SteamApi.SendMessage(msgType, d);
                if (result != EGCResults.k_EGCResultOK)
                {
                    throw new Exception(result.ToString());
                }
            }

            Thread.Sleep(10);

            byte[] buffer;
            uint msgSize = 0;

            List<byte[]> responses = new List<byte[]>();

            for (int timeoutCount = 0; timeoutCount < 75 * datas.Count && responses.Count < datas.Count; timeoutCount++)
            {
                while (SteamApi.IsMessageAvailable(ref msgSize))
                {
                    buffer = new byte[msgSize];
                    uint retMsgType = 0, retMsgSize = 0;

                    SteamApi.RetrieveMessage(ref retMsgType, buffer, ref retMsgSize);
                    if (buffer.Length != retMsgSize)
                    {
                        throw new Exception("size mismatch");
                    }

                    if (retMsgType == msgType + 1)
                    {
                        timeoutCount = 0;
                        responses.Add(buffer);
                        if (responses.Count == datas.Count)
                        {
                            break;
                        }
                    }

                    Thread.Sleep(25);
                }

                Thread.Sleep(150);
            }

            if (responses.Count < datas.Count)
            {
                throw new Exception(String.Format("timeout waiting for response for message type: {0}.", msgType));
            }

            return responses.ToArray();
        }

        public byte[] SendMessageAndWaitForReply(uint msgType, byte[] data)
        {
            this.ClearMessageQueue();
            EGCResults result = SteamApi.SendMessage(msgType, data);
            if (result != EGCResults.k_EGCResultOK)
            {
                throw new Exception(result.ToString());
            }

            Thread.Sleep(50);

            int timeoutCount = 0;

            byte[] buffer;

            while (timeoutCount < 35)
            {
                uint msgSize = 0;
                Stopwatch watch = Stopwatch.StartNew();
                while (SteamApi.IsMessageAvailable(ref msgSize))
                {
                    buffer = new byte[msgSize];

                    uint retMsgType = 0, retMsgSize = 0;

                    SteamApi.RetrieveMessage(ref retMsgType, buffer, ref retMsgSize);

                    if (buffer.Length != retMsgSize)
                    {
                        throw new Exception("size mismatch");
                    }

                    if (retMsgType == msgType + 1)
                    {
                        watch.Stop();
                        return buffer;
                    }
                }

                watch.Stop();
                Thread.Sleep(25);
                timeoutCount++;
            }

            throw new Exception(String.Format("timeout waiting for response for message type: {0}.", msgType));
        }
    }
}
