using System.Diagnostics.CodeAnalysis;

namespace FOFA_Bot.PlayerStats
{
#pragma warning disable IDE1006
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
#pragma warning restore IDE1006
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}