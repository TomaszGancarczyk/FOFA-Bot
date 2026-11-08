using Discord;
using FOFA_Bot.Bot;

namespace FOFA_Bot.Attendance
{
    internal class MessageGenerator
    {
        internal static Message? CreateAttendanceMessage(string clan)
        {
            Logger.LogInformation($"    Creating attendance message from template for {clan}");
            Message attendanceMessage = new()
            {
                Clan = clan
            };
            attendanceMessage.EmbedMessage = GenerateMessageFromData(attendanceMessage);
            if (attendanceMessage.EmbedMessage == null)
            {
                return null;
            }
            attendanceMessage = AddMessageButtons(attendanceMessage, clan);
            return attendanceMessage;
        }

        internal static EmbedBuilder? GenerateMessageFromData(Message attendanceMessage)
        {
            Logger.LogInformation($"[message] Creating weekly attendance message");
            EmbedBuilder embedMessage = CreateBaseMessage(attendanceMessage);
            MemberHandler.CreateMembersList(attendanceMessage.Clan);
            embedMessage = AddMessageFields(embedMessage, attendanceMessage.Clan);
            embedMessage = AddFooterMessage(embedMessage, attendanceMessage.Clan);
            Logger.LogInformation($"    Embed message created");
            return embedMessage;
        }

        private static EmbedBuilder CreateBaseMessage(Message attendanceMessage)
        {
            Logger.LogInformation($"    Creating base message");
            DateTime nextThursday = new
            (
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day + ((int)DayOfWeek.Thursday - (int)DateTime.Today.DayOfWeek + 7) % 7,
                19,
                0,
                0
            );
            attendanceMessage.ThursdayDate = nextThursday;
            string descriptionString = "";
            for (int i = 0; i < 3; i++)
            {
                long eventUnix = GetUnixFromDateTime(nextThursday.AddDays(i));
                descriptionString += $"{nextThursday.DayOfWeek + i}: <t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n";
            }
            if (attendanceMessage.Clan == "ROFA")
            {
                long eventUnix = GetUnixFromDateTime(nextThursday.AddDays(3).AddHours(-1));
                descriptionString += $"Sunday: <t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n";
            }
            string lineup = "";
            if (attendanceMessage.Clan == "ROFA") lineup = Format.Bold($"- Lineup: https://discord.com/channels/710884253457711134/1279534231256694845\n");
            else if (attendanceMessage.Clan == "BOFA") lineup = Format.Bold($"- Lineup: https://discord.com/channels/710884253457711134/1519798128138780722\n");
            EmbedBuilder embedMessage = new()
                {
                    Color = Color.DarkGreen,
                    Title = $"{nextThursday:dd.MM} - {nextThursday.AddDays(3):dd.MM} Clan Wars",
                    Description = descriptionString + lineup
                };
            return embedMessage;
        }
        internal static EmbedBuilder AddMessageFields(EmbedBuilder embedMessage, string clan)
        {
            embedMessage.Fields = [];
            Logger.LogInformation($"    Creating message fields");
            List<IEmote> squadEmotes = GetSquadEmotes();
            List<Member> members = MemberHandler.GetMembers(clan);
            List<Member> handledMembers = [];
            Logger.LogInformation($"    {members.Count} members found");
            string days = "Thu | Fri | Sat | ";
            if (clan == "ROFA") days += ("Sun | Member");
            else if (clan == "BOFA") days += ("Member");
            for (int squadCount = 1; squadCount <= 9; squadCount++)
            {
                string? squadMembers = "";
                foreach (Member member in members) if (!handledMembers.Contains(member) && squadCount == member.squad)
                    {
                        string? newMember = null;
                        if (clan == "BOFA") member.status = member.status.Take(3).ToArray();
                        if (member.discordUser == null) continue;
                        if (member.inGameName != null) newMember = AddMemberAndStatus(member.inGameName, member.status, member.squadleader);
                        else newMember = AddMemberAndStatus(member.discordUser.DisplayName, member.status, member.squadleader);
                        if (newMember == null)
                            continue;
                        squadMembers += newMember;
                        handledMembers.Add(member);
                    }
                if (squadCount < 7 && squadMembers != "")
                {
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Squad {squadCount}\n{days}", squadMembers, false);
                }
                else if (squadCount == 7 && squadMembers != "")
                {
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Reserve\n{days}", squadMembers, false);
                }
                else if (squadCount == 8 && squadMembers != "")
                {
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Unassigned\n{days}", squadMembers, false);
                }
            }
            if (members.Count > handledMembers.Count)
                Logger.LogError($"    Unable to handle {members.Count - handledMembers.Count} members");
            else
                Logger.LogInformation($"    Handled all members");
            return embedMessage;
        }
        internal static EmbedBuilder AddFooterMessage(EmbedBuilder embedMessage, string clan)
        {
            Logger.LogInformation($"    Creating footer");
            string footerString = ($"____________________________________________________________________________________________________");
            List<Member> members = MemberHandler.GetMembers(clan);
            for (int i = 0; i < 3; i++)
            {
                int present = members.Where(m => m.status[i] == true).Count();
                int absent = members.Where(m => m.status[i] == false).Count();
                int usigned = members.Where(m => m.status[i] == null).Count();
                DayOfWeek weekday = DayOfWeek.Thursday + i;
                footerString += $"\n{weekday}: {present} Present, {absent} Absent, {usigned} Unsigned";
            }
            if (clan == "ROFA") footerString += $"\n{DayOfWeek.Sunday}: {members.Where(m => m.status[3] == true).Count()} Present, {members.Where(m => m.status[3] == false).Count()} Absent, {members.Where(m => m.status[3] == null).Count()} Unsigned";
            embedMessage.WithFooter(footerString);
            return embedMessage;
        }

        internal static DateTime GetEventDateTime(double eventHour)
        {
            Logger.LogInformation($"    Creating event DateTime");
            DateTime eventDateTime = DateTime.Today;
            if (eventHour < (DateTime.Now.Hour + 1)) eventDateTime = eventDateTime.AddDays(1);
            eventDateTime = eventDateTime.AddHours(eventHour);
            Logger.LogInformation($"    Event DateTime set for {eventDateTime}");
            return eventDateTime;
        }
        private static long GetUnixFromDateTime(DateTime dateTime) => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        private static List<IEmote> GetSquadEmotes()
        {
            List<IEmote> squadEmotes = [];
            squadEmotes.Add(new Emoji("🟦")); //1
            squadEmotes.Add(new Emoji("🟥")); //2
            squadEmotes.Add(new Emoji("🟩")); //3
            squadEmotes.Add(new Emoji("🟪")); //4
            squadEmotes.Add(new Emoji("🟧")); //5
            squadEmotes.Add(new Emoji("🐈‍")); //6
            squadEmotes.Add(new Emoji("⬜")); //7 - Reseve
            squadEmotes.Add(new Emoji("🔳")); //8 - Unassigned
            return squadEmotes;
        }
        private static string? AddMemberAndStatus(string displayName, bool?[] status, bool squadleader)
        {
            if (squadleader) displayName = $"**{displayName}**";
            string memberRow = "";
            foreach (bool? dayStatus in status)
            {
                if (dayStatus == null) memberRow += $"{new Emoji("⚫")} {Format.Bold("\\||")} ";
                else if (dayStatus == true) memberRow += $"{new Emoji("🟢")} {Format.Bold("\\||")} ";
                else if (dayStatus == false) memberRow += $"{new Emoji("🔴")} {Format.Bold("\\||")} ";
            }
            if (memberRow.Length > 0) return $"{memberRow} {displayName} \n";
            return null;
        }
        internal static Message AddMessageButtons(Message message, string clan)
        {
            Logger.LogInformation($"    Adding attendance buttons");
            ActionRowBuilder presentActionRow = new();
            ActionRowBuilder absentActionRow = new();
            for (int i = 0; i < 3; i++)
            {
                presentActionRow.WithButton($"{message.ThursdayDate.DayOfWeek + i}", $"presentButton_{message.ThursdayDate.DayOfWeek + i}", ButtonStyle.Success);
            }
            if (clan == "ROFA")
            {
                presentActionRow.WithButton($"{DayOfWeek.Sunday}", $"presentButton_Sunday", ButtonStyle.Success);
            }
            presentActionRow.WithButton($"All Days", $"presentButton_Week", ButtonStyle.Success);
            for (int i = 0; i < 3; i++)
            {
                absentActionRow.WithButton($"{message.ThursdayDate.DayOfWeek + i}", $"absentButton_{message.ThursdayDate.DayOfWeek + i}", ButtonStyle.Danger);
            }
            if (clan == "ROFA")
            {
                absentActionRow.WithButton($"{DayOfWeek.Sunday}", $"absentButton_Sunday", ButtonStyle.Danger);
            }
            absentActionRow.WithButton($"All Days", $"absentButton_Week", ButtonStyle.Danger);
            Logger.LogInformation($"    Created buttons");
            message.MessageButtons = new ComponentBuilderV2()
                .WithActionRow(presentActionRow)
                .WithActionRow(absentActionRow);
            return message;
        }
    }
}
