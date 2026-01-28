using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessageGenerator
    {
        private static EmbedBuilder? EmbedMessage;
        private static AttendanceMessage? AttendanceMessage;
        internal static AttendanceMessage CreateAttendanceMessageFromTemplate(string template)
        {
            Logger.LogInformation($"Creating attendance message from template");
            AttendanceMessage = new();
            DateTime eventDateTime;

            switch (template)
            {
                case "Tournament":
                    eventDateTime = GetEventDateTime(BotData.GetTournamentHour());
                    GenerateMessageFromData(template, eventDateTime, Color.DarkGreen);
                    break;
                case "Brawl":
                    eventDateTime = GetEventDateTime(BotData.GetBrawlHour());
                    GenerateMessageFromData(template, eventDateTime, Color.Green);
                    break;
                case "Base Capture":
                    eventDateTime = GetEventDateTime(BotData.GetBaseCaptureHour());
                    GenerateMessageFromData(template, eventDateTime, Color.LightOrange);
                    break;
                case "Golden Drop":
                    eventDateTime = GetEventDateTime(BotData.GetGoldenDropHour());
                    GenerateMessageFromData(template, eventDateTime, Color.Gold);
                    break;
            }


            AttendanceMessage.embedMessage = EmbedMessage;
            AttendanceMessage = AddMessageButtons(AttendanceMessage);
            return AttendanceMessage;
        }
        internal static AttendanceMessage CreateCustomAttendanceMessage(string EventName, DateTime eventDateTime)
        {
            Logger.LogInformation($"Creating custom attendance message");
            AttendanceMessage = null;

            GenerateMessageFromData(EventName, eventDateTime, Color.Green);

            AttendanceMessage.embedMessage = EmbedMessage;
            return AttendanceMessage;
        }


        private static void GenerateMessageFromData(string EventName, DateTime eventDateTime, Color color)
        {
            Logger.LogInformation($"Creating {EventName} attendance message");
            EmbedMessage = null;
            AttendanceMessage.Date = eventDateTime;

            EmbedBuilder embedMessage = CreateBaseMessage(EventName, eventDateTime, color);
            MemberHandler.CreateMembersList();
            embedMessage = AddMessageFields(embedMessage);
            embedMessage = AddFooterMessage(embedMessage);

            EmbedMessage = embedMessage;
            Logger.LogInformation($"Embed message created");
        }

        private static EmbedBuilder CreateBaseMessage(string EventName, DateTime eventDateTime, Color color)
        {
            Logger.LogInformation($"Creating base message...");
            EmbedBuilder embedMessage = new();
            long eventUnix = GetUnixFromDateTime(eventDateTime);
            embedMessage.WithTitle($"{eventDateTime.DayOfWeek} {EventName}")
                .WithDescription($"<t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n" +
                $"Lineup: https://discord.com/channels/710884253457711134/1279534231256694845\n" +
                $"<@&{BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id}>")
                .WithColor(color);
            return embedMessage;
        }
        internal static EmbedBuilder AddMessageFields(EmbedBuilder embedMessage)
        {
            embedMessage.Fields = [];
            Logger.LogInformation($"Creating message fields...");
            List<IEmote> squadEmotes = GetSquadEmotes();
            List<Member> members = MemberHandler.GetMembers();
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
        internal static EmbedBuilder AddFooterMessage(EmbedBuilder embedMessage)
        {
            Logger.LogInformation($"Creating footer...");
            List<Member> members = MemberHandler.GetMembers();
            int present = members.Where(m => m.status == true).Count();
            int absent = members.Where(m => m.status == false).Count();
            int usigned = members.Where(m => m.status == null).Count();
            embedMessage.WithFooter($"____________________________________________________________________________________________________\n{present} Present, {absent} Absent, {usigned} Unsigned");
            return embedMessage;
        }

        internal static DateTime GetEventDateTime(double eventHour)
        {
            Logger.LogInformation($"Creating event DateTime");
            DateTime eventDateTime = DateTime.Today;
            if (eventHour < (DateTime.Now.Hour + 1)) eventDateTime = eventDateTime.AddDays(1);
            eventDateTime = eventDateTime.AddHours(eventHour);
            Logger.LogInformation($"Event DateTime set for {eventDateTime}");
            return eventDateTime;
        }
        private static long GetUnixFromDateTime(DateTime dateTime) => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
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
        private static AttendanceMessage AddMessageButtons(AttendanceMessage message)
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
