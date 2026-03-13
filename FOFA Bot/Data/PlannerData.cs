namespace FOFA_Bot.Data
{
    internal class PlannerData(string discordName, string inGameName)
    {
        internal string discordName = discordName;
        internal string inGameName = inGameName;
        internal int priority = 0;
    }
}
