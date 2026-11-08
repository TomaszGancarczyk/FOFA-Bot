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
        internal static bool GetAutomnaticSignupMessage(string clan)
        {
            LoadJson();
            if (clan == "ROFA") return JsonBotData.rofaSignups;
            else if (clan == "BOFA") return JsonBotData.bofaSignups;
            return false;
        }
        internal static bool GetAutomaticReminderRofa(string clan)
        {
            LoadJson();
            if (clan == "ROFA") return JsonBotData.rofaReminders;
            else if (clan == "BOFA") return JsonBotData.bofaReminders;
            return false;
        }
        internal static Dictionary<string, bool> GetAutomaticSettings(string clan)
        {
            Logger.LogInformation($"    Getting settings data for {clan}");
            LoadJson();
            if (clan == "ROFA")
            {
                return new Dictionary<string, bool>
                {
                    { "signups", (bool)JsonBotData.rofaSignups},
                    { "reminders", (bool)JsonBotData.rofaReminders},
                };
            }
            else if (clan == "BOFA")
            {
                return new Dictionary<string, bool>
                {
                    { "signups", (bool)JsonBotData.bofaSignups},
                    { "reminders", (bool)JsonBotData.bofaReminders},
                };

            }
            return null;
        }
        internal static void SetAutomaticSettings(Dictionary<string, bool> settings, string clan)
        {
            LoadJson();
            if (clan == "ROFA")
            {
                JsonBotData.rofaSignups = settings["signups"];
                JsonBotData.rofaReminders = settings["reminders"];
            }
            else if (clan == "BOFA")
            {
                JsonBotData.bofaSignups = settings["signups"];
                JsonBotData.bofaReminders = settings["reminders"];
            }
            File.WriteAllText("..\\..\\..\\Data\\Settings.json", JsonBotData.ToString());
        }
    }
}