using Newtonsoft.Json.Linq;

namespace FOFA_Bot.Data
{
    internal class SettingsHandler
    {
        private static dynamic JsonBotData = "";
        private static void LoadJson()
        {
            using StreamReader reader = new("..\\..\\..\\Data\\Settings.json");
            string json = reader.ReadToEnd();
            JsonBotData = JObject.Parse(json);
        }
        internal static bool GetAutomnaticSignupMessage()
        {
            LoadJson();
            return JsonBotData.signups;
        }
        internal static bool GetAutomaticSignupQuestion()
        {
            LoadJson();
            return JsonBotData.questions;
        }
        internal static bool GetAutomaticReminder()
        {
            LoadJson();
            return JsonBotData.reminders;
        }
        internal static Dictionary<string, bool> GetAutomaticSettingsRofa()
        {
            Logger.LogInformation("    Getting settings data");
            LoadJson();
            Dictionary<string, bool> dict = new()
            {
                { "signups", (bool)JsonBotData.signups},
                { "questions", (bool)JsonBotData.questions},
                { "reminders", (bool)JsonBotData.reminders},
            };
            return dict;
        }
        internal static void SetAutomaticSettingsRofa(Dictionary<string, bool> settings)
        {
            LoadJson();
            JsonBotData.signups = settings["signups"];
            JsonBotData.questions = settings["questions"];
            JsonBotData.reminders = settings["reminders"];
            File.WriteAllText("..\\..\\..\\Data\\Settings.json", JsonBotData.ToString());
        }
    }
}
