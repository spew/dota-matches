using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaMatches
{
    public enum ResultType
    {
        Success,
        Exception,
        NotFound,
    };

    public class MatchFetchResult
    {
        public ulong MatchId { get; set; }
        public Match Match { get; set; }
        public Exception Exception { get; set; }
        public ResultType Result { get; set; }
        public byte[] Raw { get; set; }
    }

    public class MatchFetcher
    {
        private SteamMessenger messenger = null;
        private const uint MsgType = 2147490743;

        private byte[] invalidGameResponse = new byte[]
        {
            0xb8, 0x1b, 0x00, 0x80, 0x09, 0x00, 0x00, 
            0x00, 0x59,0x27, 0x00, 0x00,0x00, 0x00, 0x00, 
            0x00, 0x00, 0x08, 0x0f, 
        };

        private byte[] fetchGameMessage = new byte[]
        {
            0xb7, 0x1b, 0x00, 0x80, 0x09, 0x00, 0x00, 0x00, 
            0x51, 0x27, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x08, //0x9a, 0xea, 0xc0, 0x09
        };

        public MatchFetcher(SteamMessenger messenger)
        {
            this.messenger = messenger;
        }

        /// <summary>
        /// For best performance you will want to call this method with 15-20 match ids and retryFailures set to false.
        /// With these settings you can achieve 15+ matches downloaded per second.
        /// </summary>
        /// <param name="matchIds">List of match ids to retrieve</param>
        /// <param name="retryFailures">If true then we retry all hte failures and return why they failed</param>
        /// <returns>An array of MatchResult objects</returns>
        public MatchFetchResult[] FetchGames(ulong[] matchIds, bool retryFailures = false)
        {
            var messages = matchIds.Select(id => this.BuildMessage(id)).ToArray();
            byte[][] responses = this.messenger.SendMessagesAndWaitForReplies(MatchFetcher.MsgType, messages);
            
            var validResponses = responses.Where(r => r.Length > 19);

            List<MatchFetchResult> matches = new List<MatchFetchResult>();
            foreach (var r in validResponses)
            {
                MatchFetchResult result = new MatchFetchResult();

                try
                {
                    var m = MatchSerializer.FromBytes(r);
                    result.MatchId = m.Id;
                    result.Match = m;
                    result.Result = ResultType.Success;
                    result.Raw = r;
                }
                catch
                {
                    continue;
                }

                matches.Add(result);
            }

            if (retryFailures)
            {
                var idSet = new HashSet<ulong>(matchIds);
                var missingIds = idSet.Except(matches.Select(m => m.MatchId));

                foreach (var id in missingIds)
                {
                    var message = this.BuildMessage(id);
                    var result = this.FetchGame(id);

                    MatchFetchResult mr = new MatchFetchResult()
                    {
                        MatchId = id,
                        Raw = result,
                    };

                    if (result.Length <= 19)
                    {
                        mr.Result = ResultType.NotFound;
                    }
                    else
                    {
                        try
                        {
                            var m = MatchSerializer.FromBytes(result);
                            mr.Match = m;
                            mr.Result = ResultType.Success;

                        }
                        catch (Exception e)
                        {
                            mr.Exception = e;
                            mr.Result = ResultType.Exception;
                        }
                    }

                    matches.Add(mr);
                }
            }

            return matches.ToArray();
        }

        // TODO: return a Match object instead of bytes here?!
        public byte[] FetchGame(ulong matchId)
        {
            byte[] summary = null;

            byte[] message = this.BuildMessage(matchId);
            summary = messenger.SendMessageAndWaitForReply(2147490743, message);

            return summary;
        }

        private byte[] BuildMessage(ulong matchId)
        {
            byte[] byteValue = DotaMatchesUtils.PackInteger(matchId);

            byte[] message = new byte[this.fetchGameMessage.Length + byteValue.Length];
            Array.Copy(this.fetchGameMessage, 0, message, 0, this.fetchGameMessage.Length);
            Array.Copy(byteValue, 0, message, this.fetchGameMessage.Length, byteValue.Length);

            return message;
        }
    }
}
