using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using System;

namespace FOFA_Bot.Attendance
{
    internal class MessageGenerator
    {
        private static EmbedBuilder? EmbedMessage;
        private static Message? AttendanceMessage;
        internal static Message? CreateAttendanceMessage()
        {
            Logger.LogInformation($"    Creating attendance message from template");
            AttendanceMessage = new();
            DateTime eventDateTime;
            // TODO event hours
            //eventDateTime = GetEventDateTime(BotData.GetTournamentHour());
            //eventDateTime = GetEventDateTime(BotData.GetBaseCaptureHour());

            EmbedMessage = GenerateMessageFromData();
            AttendanceMessage.Reminder = true;
            if (EmbedMessage == null)
            {
                return null;
            }
            AttendanceMessage.EmbedMessage = EmbedMessage;
            AttendanceMessage = AddMessageButtons(AttendanceMessage);
            return AttendanceMessage;
        }

        internal static EmbedBuilder? GenerateMessageFromData()
        {
            Logger.LogInformation($"[message] Creating weekly attendance message");
            EmbedMessage = null;
            if (AttendanceMessage == null)
            {
                Logger.LogError($"    Cannot find Attendance Message in GenerateMessageFromData, exiting...");
                return null;
            }
            EmbedBuilder embedMessage = CreateBaseMessage();
            MemberHandler.CreateMembersList();
            embedMessage = AddMessageFields(embedMessage);
            embedMessage = AddFooterMessage(embedMessage);
            Logger.LogInformation($"    Embed message created");
            return embedMessage;
        }

        private static EmbedBuilder CreateBaseMessage()
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
            AttendanceMessage.ThursdayDate = nextThursday;
            string descriptionString = "";
            for (int i = 0; i<4; i++)
            {
                long eventUnix = GetUnixFromDateTime(nextThursday.AddDays(i));
                descriptionString += $"{nextThursday.DayOfWeek}: <t:{eventUnix}:D><t:{eventUnix}:t> - <t:{eventUnix}:R>\n";
            }
            EmbedBuilder embedMessage = new()
            {
                Color = Color.DarkGreen,
                Title = $"{nextThursday:dd.MM} - {nextThursday.AddDays(3):dd.MM} Clan Wars",
                Description = descriptionString +
                $"Lineup: https://discord.com/channels/710884253457711134/1279534231256694845\n"
            };
            return embedMessage;
        }
        internal static EmbedBuilder AddMessageFields(EmbedBuilder embedMessage)
        {
            embedMessage.Fields = [];
            Logger.LogInformation($"    Creating message fields");
            List<IEmote> squadEmotes = GetSquadEmotes();
            List<Member> members = MemberHandler.GetMembers();
            List<Member> handledMembers = [];
            Logger.LogInformation($"    {members.Count} members found");
            for (int squadCount = 1; squadCount <= 9; squadCount++)
            {
                string? squadMembers = "";
                foreach (Member member in members) if (!handledMembers.Contains(member) && squadCount == member.squad)
                    {
                        string? newMember = null;
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
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Squad {squadCount}", squadMembers, true);
                }
                else if (squadCount == 7 && squadMembers != "")
                {
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Reserve", squadMembers, true);
                }
                else if (squadCount == 8 && squadMembers != "")
                {
                    embedMessage.AddField($"{squadEmotes[squadCount - 1]} Unassigned", squadMembers, true);
                }
            }
            if (members.Count > handledMembers.Count)
                Logger.LogError($"    Unable to handle {members.Count - handledMembers.Count} members");
            else
                Logger.LogInformation($"    Handled all members");
            return embedMessage;
        }
        internal static EmbedBuilder AddFooterMessage(EmbedBuilder embedMessage)
        {
            Logger.LogInformation($"    Creating footer");
            string footerString = ($"____________________________________________________________________________________________________");
            List<Member> members = MemberHandler.GetMembers();
            for (int i = 0; i < 4; i++)
            {
                int present = members.Where(m => m.status[i] == true).Count();
                int absent = members.Where(m => m.status[i] == false).Count();
                int usigned = members.Where(m => m.status == null).Count();
                DayOfWeek weekday = DayOfWeek.Thursday + i;
                footerString += $"\n *{weekday}* : {present} Present, {absent} Absent, {usigned} Unsigned";
            }
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
                if (dayStatus == null) memberRow += $"{new Emoji("⚫")} | ";
                else if (dayStatus == true) memberRow += $"{new Emoji("🟢")} | ";
                else if (dayStatus == false) memberRow += $"{new Emoji("🔴")} | ";
            }
            if (memberRow.Length > 0) return $"{memberRow} {displayName} \n";
            return null;
        }
        internal static Message AddMessageButtons(Message message)
        {
            Logger.LogInformation($"    Adding attendance buttons");
            ComponentBuilder buttons = new();
            for (int i = 0; i < 4; i++)
            {
                buttons
                    .WithButton($"{message.ThursdayDate.DayOfWeek + i}", $"presentButton_{message.ThursdayDate.DayOfWeek + i}", ButtonStyle.Success)
                    .WithButton($"{message.ThursdayDate.DayOfWeek + i}", $"absentButton_{message.ThursdayDate.DayOfWeek + i}", ButtonStyle.Danger);
            }
            Logger.LogInformation($"    Created buttons");
            message.MessageButtons = buttons;
            return message;
        }
    }
}
