using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApi.Enums
{
    internal static class Enum
    {

        public static string GetString(ParseMode pm)
        {
            switch (pm)
            {
                case ParseMode.Markdown:
                    return "Markdown";
                case ParseMode.Html:
                    return "Html";
                default:
                    return null;
            }
        }

        public static ParseMode GetParseMode(string str)
        {
            switch (str.ToLower())
            {
                case "markdown":
                    return ParseMode.Markdown;
                case "html":
                    return ParseMode.Html;
                default:
                    return ParseMode.None;
            }
        }

        public static string GetString(ChatMemberStatus cms)
        {
            switch (cms)
            {
                case ChatMemberStatus.Administrator:
                    return "administrator";
                case ChatMemberStatus.Creator:
                    return "creator";
                case ChatMemberStatus.Kicked:
                    return "kicked";
                case ChatMemberStatus.Left:
                    return "left";
                case ChatMemberStatus.Member:
                    return "member";
                case ChatMemberStatus.Restricted:
                    return "restricted";
                default:
                    return "unknown";
            }
        }

        public static ChatMemberStatus GetChatMemberStatus(string str)
        {
            switch (str)
            {
                case "creator":
                    return ChatMemberStatus.Creator;
                case "administrator":
                    return ChatMemberStatus.Administrator;
                case "member":
                    return ChatMemberStatus.Member;
                case "restricted":
                    return ChatMemberStatus.Restricted;
                case "left":
                    return ChatMemberStatus.Left;
                case "kicked":
                    return ChatMemberStatus.Kicked;
                default:
                    return ChatMemberStatus.Unknown;
            }
        }

        public static string GetString(MaskPositionPointType mppt)
        {
            switch (mppt)
            {
                case MaskPositionPointType.Forehead:
                    return "forehead";
                case MaskPositionPointType.Eyes:
                    return "eyes";
                case MaskPositionPointType.Mouth:
                    return "mouth";
                case MaskPositionPointType.Chin:
                    return "chin";
                default:
                    return "unknown";
            }
        }

        public static MaskPositionPointType GetMaskPositionPointType(string str)
        {
            switch (str)
            {
                case "forehead":
                    return MaskPositionPointType.Forehead;
                case "eyes":
                    return MaskPositionPointType.Eyes;
                case "mouth":
                    return MaskPositionPointType.Mouth;
                case "chin":
                    return MaskPositionPointType.Chin;
                default:
                    return MaskPositionPointType.Unknown;
            }
        }

        public static string GetString(MessageEntityType met)
        {
            switch (met)
            {
                case MessageEntityType.Bold:
                    return "bold";
                case MessageEntityType.BotCommand:
                    return "bot_command";
                case MessageEntityType.Code:
                    return "code";
                case MessageEntityType.Email:
                    return "email";
                case MessageEntityType.Hashtag:
                    return "hashtag";
                case MessageEntityType.Italic:
                    return "italic";
                case MessageEntityType.Mention:
                    return "mention";
                case MessageEntityType.PhoneNumber:
                    return "phone_number";
                case MessageEntityType.Pre:
                    return "pre";
                case MessageEntityType.TextLink:
                    return "text_link";
                case MessageEntityType.TextMention:
                    return "text_mention";
                case MessageEntityType.Url:
                    return "url";
                default:
                    return "unknown";
            }
        }

        public static MessageEntityType GetMessageEntityType(string str)
        {
            switch (str)
            {
                case "mention":
                    return MessageEntityType.Mention;
                case "hashtag":
                    return MessageEntityType.Hashtag;
                case "bot_command":
                    return MessageEntityType.BotCommand;
                case "url":
                    return MessageEntityType.Url;
                case "email":
                    return MessageEntityType.Email;
                case "bold":
                    return MessageEntityType.Bold;
                case "italic":
                    return MessageEntityType.Italic;
                case "code":
                    return MessageEntityType.Code;
                case "pre":
                    return MessageEntityType.Pre;
                case "text_link":
                    return MessageEntityType.TextLink;
                case "text_mention":
                    return MessageEntityType.TextMention;
                case "phone_number":
                    return MessageEntityType.PhoneNumber;
                default:
                    return MessageEntityType.Unknown;
            }
        }

        public static string GetString(ChatType ct)
        {
            switch (ct)
            {
                case ChatType.Channel:
                    return "channel";
                case ChatType.Group:
                    return "group";
                case ChatType.Private:
                    return "private";
                case ChatType.Supergroup:
                    return "supergroup";
                default:
                    return "unknown";
            }
        }

        public static ChatType GetChatType(string str)
        {
            switch (str)
            {
                case "private":
                    return ChatType.Private;
                case "group":
                    return ChatType.Group;
                case "supergroup":
                    return ChatType.Supergroup;
                case "channel":
                    return ChatType.Channel;
                default:
                    return ChatType.Unknown;
            }
        }
    }
}
