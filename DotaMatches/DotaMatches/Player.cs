using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotaMatches
{
    [DebuggerDisplay("Name={Name}, Level={Level}, K={Kills}, D={Deaths}, A={Assists}")]
    public class Player
    {
        public ulong SteamAccountId { get; set; }
        public String Name { get; set; }
        public int Slot { get; set; }
        public int Level { get; set; }
        public Hero Hero { get { return (Hero)this.HeroId; } }
        public int HeroId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int FinalGold { get; set; }
        public int LastHits { get; set; }
        public int Denies { get; set; }
        public int GoldPerMinute { get; set; }
        public int ExperiencePerMinute { get; set; }
        public int GoldSpent { get; set; }
        public int HeroDamage { get; set; }
        public int LeaverStatus { get; set; }
        public int TowerDamage { get; set; }
        public int HeroHealing { get; set; }
        public bool IsBot { get; set; }
        public TeamName TeamName { get; set; }
        public List<ulong> ItemIds { get; set; }
        public List<Item> ItemNames { get { return this.ItemIds.Select(i => (Item)i).ToList(); } }
    }
}
