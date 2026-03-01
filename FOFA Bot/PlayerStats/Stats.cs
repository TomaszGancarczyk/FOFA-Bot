using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FOFA_Bot.PlayerStats
{
    //internal class Stats
    //{
    //    internal required string Uuid { get; set; }
    //    //image
    //    internal required string Username { get; set; }
    //    internal required string Faction { get; set; }
    //    internal required string Clan { get; set; }
    //    internal required string ClanTag { get; set; }
    //    internal required string ClanRank { get; set; }
    //    internal required int TimesJoinedClan { get; set; }
    //    internal required int PlaytimeHours { get; set; }
    //    internal required DateOnly JoinedGame { get; set; }
    //    internal required DateOnly LastLogin { get; set; }
    //    internal required int Kills { get; set; }
    //    internal required int Deaths { get; set; }
    //    internal required int Assists { get; set; }
    //    internal double TotalKD { get; set; }
    //    internal double SessionKD { get; set; }
    //    internal required int Suicides { get; set; }
    //    internal required int ArtifactsFound { get; set; }
    //    internal required int HighestMoney { get; set; }
    //    internal required int BoltsThrown { get; set; }
    //    internal required int MutantKills { get; set; }
    //    internal required int NpcKills { get; set; }
    //    internal required int DeliveriesMade { get; set; }
    //    internal required int CachesFound { get; set; }
    //    internal required int SignalsFound { get; set; }
    //}
    public class PlayerStats
    {
        public string username { get; set; }
        public string uuid { get; set; }
        public string status { get; set; }
        public string alliance { get; set; }
        public DateTime lastLogin { get; set; }
        public List<string> displayedAchievements { get; set; }
        public Clan clan { get; set; }
        public List<Stat> stats { get; set; }
    }
    public class Clan
    {
        public Info info { get; set; }
        public Member member { get; set; }
    }

    public class Info
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tag { get; set; }
        public int level { get; set; }
        public int levelPoints { get; set; }
        public DateTime registrationTime { get; set; }
        public string alliance { get; set; }
        public string description { get; set; }
        public string leader { get; set; }
        public int memberCount { get; set; }
    }

    public class Member
    {
        public string name { get; set; }
        public string rank { get; set; }
        public DateTime joinTime { get; set; }
    }


    public class Stat
    {
        public string id { get; set; }
        public string type { get; set; }
        public object value { get; set; }
    }

}
