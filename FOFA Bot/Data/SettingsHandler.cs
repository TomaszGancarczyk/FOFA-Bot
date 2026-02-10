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
        internal static bool GetAutomaticReminder()
        {
            LoadJson();
            return JsonBotData.AutomaticReminder;
        }
        internal static bool GetAutomnaticSignupMessage()
        {
            LoadJson();
            return JsonBotData.AutomnaticSignupMessage;
        }
        internal static bool GetAutomnaticNadeMessage()
        {
            LoadJson();
            return JsonBotData.AutomnaticNadeMessage;
        }
        internal static void SetAutomaticReminder(bool status)
        {
            LoadJson();
            JsonBotData.AutomaticReminder = status;
            File.WriteAllText("..\\..\\..\\Data\\Settings.json", JsonBotData.ToString());
        }
        internal static void SetAutomnaticSignupMessage(bool status)
        {
            LoadJson();
            JsonBotData.AutomnaticSignupMessage = status;
            File.WriteAllText("..\\..\\..\\Data\\Settings.json", JsonBotData.ToString());
        }
        internal static void SetAutomnaticNadeMessage(bool status)
        {
            LoadJson();
            JsonBotData.AutomnaticNadeMessage = status;
            File.WriteAllText("..\\..\\..\\Data\\Settings.json", JsonBotData.ToString());
        }
    }
}
