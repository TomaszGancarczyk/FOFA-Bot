namespace FOFA_Bot.PlayerStats
{
    internal class Stats
    {
        internal required string Uuid { get; set; }
        //image
        internal required string Username { get; set; }
        internal required string Faction { get; set; }
        internal required string Clan { get; set; }
        internal required string ClanTag { get; set; }
        internal required string ClanRank { get; set; }
        internal required int TimesJoinedClan { get; set; }
        internal required int PlaytimeHours { get; set; }
        internal required DateOnly JoinedGame { get; set; }
        internal required DateOnly LastLogin { get; set; }
        internal required int Kills { get; set; }
        internal required int Deaths { get; set; }
        internal required int Assists { get; set; }
        internal double TotalKD { get; set; }
        internal double SessionKD { get; set; }
        internal required int Suicides { get; set; }
        internal required int ArtifactsFound { get; set; }
        internal required int HighestMoney { get; set; }
        internal required int BoltsThrown { get; set; }
        internal required int MutantKills { get; set; }
        internal required int NpcKills { get; set; }
        internal required int DeliveriesMade { get; set; }
        internal required int CachesFound { get; set; }
        internal required int SignalsFound { get; set; }
    }




    //  PlayerStats
    //    public string Uuid { get; set; }
    //    public string Username { get; set; }
    //    public string Status { get; set; }
    //    public string Alliance { get; set; }
    //    public DateTime LastLogin { get; set; } //maybe string
    //    public List<string> DisplayedAchievements { get; set; }
    //    public Clan Clan { get; set; }
    //    public List<Stat> Stats { get; set; }
    //
    //          PlayerStats -> Clan
    //
    //  Clan
    //    public Info info { get; set; }
    //    public Member member { get; set; }
    //
    //          PlayerStats -> Clan -> Info
    //
    //  Info
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string tag { get; set; }
    //    public int level { get; set; }
    //    public int levelPoints { get; set; }
    //    public DateTime registrationTime { get; set; }
    //    public string alliance { get; set; }
    //    public string description { get; set; }
    //    public string leader { get; set; }
    //    public int memberCount { get; set; }
    //
    //          PlayerStats -> Clan -> Member
    //
    //  Member
    //    public string name { get; set; }
    //    public string rank { get; set; }
    //    public DateTime joinTime { get; set; }
    //
    //          PlayerStats -> Stat
    //
    //  Stat
    //    public string id { get; set; }
    //    public string type { get; set; }
    //    public JToken value { get; set; }
}
