using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessageGenerator
    {
        private static EmbedBuilder? EmbedMessage;
        private static AttendanceMessage? AttendanceMessage;
        internal async static Task<AttendanceMessage> CreateAttendanceMessageFromTemplate(string template)
        {
            Logger.LogInformation($"Creating attendance message from template");
            AttendanceMessage = new();
            DateTime eventDateTime;

            switch (template)
            {
                case "Tournament":
                    eventDateTime = await GetEventDateTime(BotData.GetTournamentHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.DarkGreen);
                    break;
                case "Brawl":
                    eventDateTime = await GetEventDateTime(BotData.GetBrawlHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.Green);
                    break;
                case "Base Capture":
                    eventDateTime = await GetEventDateTime(BotData.GetBaseCaptureHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.LightOrange);
                    break;
                case "Golden Drop":
                    eventDateTime = await GetEventDateTime(BotData.GetGoldenDropHour());
                    await GenerateMessageFromData(template, eventDateTime, Color.Gold);
                    break;
            }


            AttendanceMessage.embedMessage = EmbedMessage;
            AttendanceMessage = await AddMessageButtons(AttendanceMessage);
            return AttendanceMessage;
        }
        internal async static Task<AttendanceMessage> CreateCustomAttendanceMessage(string EventName, DateTime eventDateTime)
        {
            Logger.LogInformation($"Creating custom attendance message");
            AttendanceMessage = null;

            await GenerateMessageFromData(EventName, eventDateTime, Color.Green);

            AttendanceMessage.embedMessage = EmbedMessage;
            return AttendanceMessage;
        }


        private async static Task GenerateMessageFromData(string EventName, DateTime eventDateTime, Color color)
        {
            Logger.LogInformation($"Creating {EventName} attendance message");
            EmbedMessage = null;
            AttendanceMessage.Date = eventDateTime;

            EmbedBuilder embedMessage = await CreateBaseMessage(EventName, eventDateTime, color);
            await MemberHandler.CreateMembersList();
            embedMessage = await AddMessageFields(embedMessage);
            embedMessage = await AddFooterMessage(embedMessage);

            EmbedMessage = embedMessage;
            Logger.LogInformation($"Embed message created");
        }

        private async static Task<EmbedBuilder> CreateBaseMessage(string EventName, DateTime eventDateTime, Color color)
        {
            Logger.LogInformation($"Creating base message...");
            EmbedBuilder embedMessage = new();
            long eventUnix = await GetUnixFromDateTime(eventDateTime);
            embedMessage.WithTitle($"{eventDateTime.DayOfWeek} {EventName}")
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n" +
                $"Lineup: https://discord.com/channels/710884253457711134/1272270948115939339")
                .WithColor(color);
            return embedMessage;
        }
        internal async static Task<EmbedBuilder> AddMessageFields(EmbedBuilder embedMessage)
        {
            embedMessage.Fields = [];
            Logger.LogInformation($"Creating message fields...");
            List<IEmote> squadEmotes = GetSquadEmotes();
            List<Member> members = await MemberHandler.GetMembers();
            List<Member> handledMembers = [];
            Logger.LogInformation($"{members.Count} members found");
            for (int squadCount = 0; squadCount <= 8; squadCount++)
            {
                string? squadMembers = "";
                foreach (Member member in members) if (!handledMembers.Contains(member) && squadCount == member.squad)
                    {
                        squadMembers += AddMemberAndStatus(member.discordUser.DisplayName, member.status);
                        handledMembers.Add(member);
                    }
                if (squadCount < 7 && squadMembers != "")
                {
                    Logger.LogInformation($"Created Squad {squadCount} field");
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Squad {squadCount}", squadMembers, true);
                }
                else if (squadCount == 7 && squadMembers != "")
                {
                    Logger.LogInformation($"Created Squad Reserve");
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Reserve", squadMembers, true);
                }
                if (squadCount == 0 && squadMembers != "")
                {
                    Logger.LogInformation($"Created Squad Unassigned");
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Unassigned", squadMembers, true);
                }
            }
            if (members.Count > handledMembers.Count)
                Logger.LogError($"Unable to handle {members.Count - handledMembers.Count} members");
            else
                Logger.LogInformation($"Handled all members");
            return embedMessage;
        }
        private async static Task<EmbedBuilder> AddFooterMessage(EmbedBuilder embedMessage)
        {
            Logger.LogInformation($"Creating footer...");
            List<Member> members = await MemberHandler.GetMembers();
            int present = members.Where(m => m.status == true).Count();
            int absent = members.Where(m => m.status == false).Count();
            int usigned = members.Where(m => m.status == null).Count();
            embedMessage.WithFooter($"____________________________________________________________________________________________________\n{present} Present, {absent} Absent, {usigned} Unsigned");
            return embedMessage;
        }

        internal async static Task<DateTime> GetEventDateTime(double eventHour)
        {
            Logger.LogInformation($"Creating event DateTime");
            DateTime eventDateTime = DateTime.Today;
            if (eventHour < (DateTime.Now.Hour + 1)) eventDateTime = eventDateTime.AddDays(1);
            eventDateTime = eventDateTime.AddHours(eventHour);
            Logger.LogInformation($"Event DateTime set for {eventDateTime}");
            return eventDateTime;
        }
        private async static Task<long> GetUnixFromDateTime(DateTime dateTime) => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        private static List<IEmote> GetSquadEmotes()
        {
            List<IEmote> squadEmotes = [];
            squadEmotes.Add(new Emoji("🔳")); //0 - Unassigned
            squadEmotes.Add(new Emoji("🟦")); //1
            squadEmotes.Add(new Emoji("🟥")); //2
            squadEmotes.Add(new Emoji("🟩")); //3
            squadEmotes.Add(new Emoji("🟪")); //4
            squadEmotes.Add(new Emoji("🟧")); //5
            squadEmotes.Add(new Emoji("🟨")); //6
            squadEmotes.Add(new Emoji("⬜")); //7 - Reseve
            return squadEmotes;
        }
        private static string AddMemberAndStatus(string displayName, bool? status)
        {
            if (status == null) return $"{new Emoji("⚫")} {displayName}\n";
            else if (status == true) return $"{new Emoji("🟢")} {displayName}\n";
            else if (status == false) return $"{new Emoji("🔴")} {displayName}\n";
            return null;
        }
        private async static Task<AttendanceMessage> AddMessageButtons(AttendanceMessage message)
        {
            Logger.LogInformation($"Adding attendance buttons");
            ComponentBuilder buttons = new ComponentBuilder()
                .WithButton("Present", "presentButton", ButtonStyle.Success)
                .WithButton("Absent", "absentButton", ButtonStyle.Danger);
            Logger.LogInformation($"Created buttons");
            message.messageButtons = buttons;
            return message;
        }
    }
}
