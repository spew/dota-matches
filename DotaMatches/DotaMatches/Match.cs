using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotaMatches
{
    public enum TeamName
    {
        Radiant = 1,
        Dire = 2,
    }

    [DebuggerDisplay("{Id}")]
    public class Match
    {

        public ulong Id { get; set; }
        public DateTime Time { get; set; }
        public ulong TimeDelta { get; set; }
        public List<Player> Players { get; set; }
        public uint ReplaySalt { get; set; }
        public TeamName WinningTeam { get; set; }
        public int Season { get; set; }
        public int DurationSeconds { get; set; }
        public bool HasBots { get; set; }
        public int TowerStatusRadiant { get; set; }
        public int TowerStatusDire { get; set; }
        public int BarracksStatusRadiant { get; set; }
        public int BarracksStatusDire { get; set; }
        public int Cluster { get; set; }
        public int FirstBloodTimeSeconds { get; set; }
        public int LobbyType { get; set; }
        public int HumanPlayerCount { get; set; }
        public int LeagueId { get; set; }

    }
}
