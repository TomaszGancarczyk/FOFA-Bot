using Discord;
using FOFA_Bot.Data;
using FOFA_Bot.Bot;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessageGenerator
    {
        private static EmbedBuilder? EmbedMessage;
        internal async static Task<AttendanceMessage> CreateAttendanceMessageFromTemplate(string template)
        {
            Logger.LogInformation($"Creating attendance message from template");
            EmbedMessage = null;
            AttendanceMessage message = new();
            DateTime eventDateTime;

            switch (template)
            {
                case "Tournament":
                    eventDateTime = await GetEventDateTime(BotData.GetTournamentHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.DarkGreen, false);
                    break;
                case "Brawl":
                    eventDateTime = await GetEventDateTime(BotData.GetBrawlHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.Green, false);
                    break;
                case "Base Capture":
                    eventDateTime = await GetEventDateTime(BotData.GetBaseCaptureHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.LightOrange, false);
                    break;
                case "Golden Drop":
                    eventDateTime = await GetEventDateTime(BotData.GetGoldenDropHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.Gold, false);
                    break;
            }

            return message;
        }

        internal async static Task<AttendanceMessage> CreateCustomAttendanceMessage(string EventName, DateTime eventDateTime)
        {
            Logger.LogInformation($"Creating custom attendance message");
            EmbedMessage = null;
            AttendanceMessage message = new();

            await GenerateMessageFromData(EventName, eventDateTime, Color.Gold, false);

            message.embedMessage = EmbedMessage;
            return message;
        }


        private async static Task GenerateMessageFromData(string EventName, DateTime eventDateTime, Color color, bool isCustom)
        {
            EmbedBuilder embedMessage = new();
            Logger.LogInformation($"Creating {EventName} message");

            long eventUnix = await GetUnixFromDateTime(eventDateTime);


            //TODO
            await MemberHandler.CreateMembersList();
            List<Member> members = await MemberHandler.GetMembers();
            List<IEmote> squadEmotes = GetSquadEmotes();


            embedMessage.WithTitle($"{eventDateTime.DayOfWeek} {EventName}")
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n" +
                $"Lineup: https://discord.com/channels/710884253457711134/1272270948115939339")
                .WithColor(Color.LightOrange);


            EmbedMessage = embedMessage;
        }

        private async static Task<DateTime> GetEventDateTime(int eventHour)
        {
            Logger.LogInformation($"Creating event DateTime");
            DateTime eventDateTime = DateTime.Today;
            if (eventHour < DateTime.Now.Hour) eventDateTime.AddDays(1);
            eventDateTime.AddHours(eventHour);
            Logger.LogInformation($"Event DateTime set for {eventDateTime}");
            return eventDateTime;
        }
        private async static Task<long> GetUnixFromDateTime(DateTime dateTime) => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        private static List<IEmote> GetSquadEmotes()
        {
            List<IEmote> squadEmotes = [];
            squadEmotes.Add(new Emoji("🔳")); //Unassigned
            squadEmotes.Add(new Emoji("🟦")); //1
            squadEmotes.Add(new Emoji("🟥")); //2
            squadEmotes.Add(new Emoji("🟩")); //3
            squadEmotes.Add(new Emoji("🟪")); //4
            squadEmotes.Add(new Emoji("🟧")); //5
            squadEmotes.Add(new Emoji("🟨")); //6
            squadEmotes.Add(new Emoji("⬜")); //Reseve
            return squadEmotes;
        }
    }
}
