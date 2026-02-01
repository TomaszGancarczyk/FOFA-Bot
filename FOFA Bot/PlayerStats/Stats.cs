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

        internal static Stats CreateTestPlayer()
        {
            var player = new Stats
            {
                Uuid = "id",
                Username = "username",
                Faction = "Rise",
                Clan = "ROFA",
                ClanTag = "[TAG]",
                ClanRank = "Colonel",
                TimesJoinedClan = 5,
                PlaytimeHours = 500,
                JoinedGame = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                LastLogin = DateOnly.FromDateTime(DateTime.Now),
                Kills = 69,
                Deaths = 420,
                Assists = 1,
                TotalKD = 69/420,
                SessionKD = 10 / 500,
                Suicides = 10000,
                ArtifactsFound = 0,
                HighestMoney = 69420,
                BoltsThrown = 10,
                MutantKills = 10000000,
                NpcKills = 100,
                DeliveriesMade = 0,
                CachesFound = 2179846,
                SignalsFound = 54321
            };
            return player;
        }
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
