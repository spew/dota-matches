using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotaMatches
{
    public static class MatchSerializer
    {
        private static readonly DateTime BaseTime = new DateTime(1969, 12, 31, 17, 00, 00, DateTimeKind.Utc);

        private static int? FindValue(byte value, byte[] bytes, int curIdx, bool throwIfNotFound = true)
        {
            int? index = null;
            for (int i = curIdx; i < bytes.Length; i++)
            {
                if (bytes[i] == value)
                {
                    index = i;
                    break;
                }
            }

            if (!index.HasValue)
            {
                throw new Exception();
            }

            return index;
        }

        private static ulong ParseVarInt(byte[] bytes, ref int curIdx)
        {
            byte curValue;
            int curShift = 0;
            ulong retValue = 0;

            do
            {
                curValue = bytes[curIdx];
                retValue |= (ulong)(curValue & 0x7F) << curShift;

                curShift += 7;
                curIdx++;
            }
            while ((curValue & 0x80) != 0);

            return retValue;
        }

        private static uint ParseUInt32(byte[] bytes, ref int curIdx)
        {
            uint value = BitConverter.ToUInt32(bytes, curIdx);
            curIdx += 4;
            return value;
        }

        private static void ValidateField(byte[] bytes, ref int curIdx, byte value)
        {
            if (bytes[curIdx] != value)
            {
                throw new Exception("invalid format");
            }

            curIdx++;
        }

        private static Player DecodePlayer(Match match, byte[] bytes, ref int curIdx)
        {
            Player p = new Player()
            {
                //Match = match,
                ItemIds = new List<ulong>(),
            };

            byte[] values = bytes.Skip(curIdx).ToArray();

            bool bot = false;
            if (bytes[curIdx] <= 61)
            {
                // this is a bot?
                //MatchSerializer.ValidateField(bytes, ref curIdx, 82);
                bot = true;
                p.IsBot = true;
            }
            else
            {
                curIdx = MatchSerializer.FindValue(0x08, bytes, curIdx).Value + 1;
                p.SteamAccountId = MatchSerializer.ParseVarInt(bytes, ref curIdx);
            }

            //values = bytes.Skip(curIdx).ToArray();

            curIdx = MatchSerializer.FindValue(0x10, bytes, curIdx).Value + 1;
            // might be the team id next?
            if (bytes[curIdx] < 128)
            {
                p.TeamName = TeamName.Radiant;
            }
            else
            {
                p.TeamName = TeamName.Dire;
            }

            p.Slot = bytes[curIdx];
            curIdx++;

            curIdx = MatchSerializer.FindValue(0x18, bytes, curIdx).Value + 1;
            //MatchSerializer.ValidateField(bytes, ref curIdx, 24);
            p.HeroId = bytes[curIdx];
            curIdx++;

            MatchSerializer.ValidateField(bytes, ref curIdx, 32);
            // first item
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 40);
            // second item
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 48);
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 56);
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 64);
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 72);
            p.ItemIds.Add(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            // kills deaths assists
            MatchSerializer.ValidateField(bytes, ref curIdx, 112);
            p.Kills = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 120);
            p.Deaths = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 128);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.Assists = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            if (bytes[curIdx] == 136)
            {
                MatchSerializer.ValidateField(bytes, ref curIdx, 136);
                MatchSerializer.ValidateField(bytes, ref curIdx, 1);
                p.LeaverStatus = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            }

            MatchSerializer.ValidateField(bytes, ref curIdx, 144);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.FinalGold = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 152);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.LastHits = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 0xa0);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.Denies = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 0xa8);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.GoldPerMinute = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 0xb0);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.ExperiencePerMinute = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 184);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.GoldSpent = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 192);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.HeroDamage = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 200);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.TowerDamage = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 208);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.HeroHealing = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 216);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            p.Level = bytes[curIdx];
            curIdx++;

            if (bot)
            {
            }
            else
            {
                MatchSerializer.ValidateField(bytes, ref curIdx, 234);
                MatchSerializer.ValidateField(bytes, ref curIdx, 1);
                String playerName = Encoding.UTF8.GetString(bytes, curIdx + 1, bytes[curIdx]);
                curIdx += 1 + bytes[curIdx];

                p.Name = playerName;
            }

            values = bytes.Skip(curIdx).ToArray();

            return p;
        }

        public static Match FromBytes(byte[] bytes)
        {
            Match match = new Match()
            {
                Players = new List<Player>(),
            };

            int curIdx = 9;

            curIdx = MatchSerializer.FindValue(8, bytes, curIdx).Value + 1;
            while (bytes[curIdx + 1] != 16)
            {
                curIdx = MatchSerializer.FindValue(8, bytes, curIdx).Value + 1;
            }

            match.Season = bytes[curIdx];

            curIdx++;

            MatchSerializer.ValidateField(bytes, ref curIdx, 16);

            if (bytes[curIdx] == 1)
            {
                match.WinningTeam = TeamName.Radiant;
            }
            else
            {
                match.WinningTeam = TeamName.Dire;
            }

            curIdx++;

            MatchSerializer.ValidateField(bytes, ref curIdx, 24);
            byte[] valuesBefore = bytes.Take(curIdx).ToArray();

            byte[] values = bytes.Take(curIdx).ToArray();

            match.DurationSeconds = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 0x25);
            match.TimeDelta = MatchSerializer.ParseUInt32(bytes, ref curIdx);
            match.Time = MatchSerializer.BaseTime.AddSeconds(match.TimeDelta);

            MatchSerializer.ValidateField(bytes, ref curIdx, 0x2a);

            Player playerSummary = MatchSerializer.DecodePlayer(match, bytes, ref curIdx);
            match.Players.Add(playerSummary);

            while (bytes[curIdx] == 0x2a)
            {
                curIdx++;
                playerSummary = MatchSerializer.DecodePlayer(match, bytes, ref curIdx);
                match.Players.Add(playerSummary);
            }

            match.HasBots = match.Players.Exists(ps => ps.IsBot);
            values = bytes.Take(curIdx).ToArray();

            MatchSerializer.ValidateField(bytes, ref curIdx, 0x30);
            match.Id = MatchSerializer.ParseVarInt(bytes, ref curIdx);

            MatchSerializer.ValidateField(bytes, ref curIdx, 64);
            match.TowerStatusRadiant = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 64);
            match.TowerStatusDire = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 72);
            match.BarracksStatusRadiant = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 72);
            match.BarracksStatusDire = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 80);
            match.Cluster = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            MatchSerializer.ValidateField(bytes, ref curIdx, 96);
            match.FirstBloodTimeSeconds = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 109);
            match.ReplaySalt = MatchSerializer.ParseUInt32(bytes, ref curIdx);

            MatchSerializer.ValidateField(bytes, ref curIdx, 128);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            match.LobbyType = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            MatchSerializer.ValidateField(bytes, ref curIdx, 136);
            MatchSerializer.ValidateField(bytes, ref curIdx, 1);
            match.HumanPlayerCount = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));

            // league id was added in later... figure out what code in the message tells me this?
            if (bytes.Length < curIdx)
            {
                MatchSerializer.ValidateField(bytes, ref curIdx, 176);
                MatchSerializer.ValidateField(bytes, ref curIdx, 1);
                match.LeagueId = Convert.ToInt32(MatchSerializer.ParseVarInt(bytes, ref curIdx));
            }

            return match;
        }
    }
}
