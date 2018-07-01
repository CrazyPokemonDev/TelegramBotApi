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

        public static string GetString(UpdateType ut)
        {
            switch (ut)
            {
                case UpdateType.CallbackQuery:
                    return "callback_query";
                case UpdateType.ChannelPost:
                    return "channel_post";
                case UpdateType.ChosenInlineResult:
                    return "chosen_inline_result";
                case UpdateType.EditedChannelPost:
                    return "edited_channel_post";
                case UpdateType.EditedMessage:
                    return "edited_message";
                case UpdateType.InlineQuery:
                    return "inline_query";
                case UpdateType.Message:
                    return "message";
                case UpdateType.PreCheckoutQuery:
                    return "pre_checkout_query";
                case UpdateType.ShippingQuery:
                    return "shipping_query";
                default:
                    return "unknown";
            }
        }

        public static UpdateType GetUpdateType(string str)
        {
            switch (str)
            {
                case "callback_query":
                    return UpdateType.CallbackQuery;
                case "channel_post":
                    return UpdateType.ChannelPost;
                case "chosen_inline_result":
                    return UpdateType.ChosenInlineResult;
                case "edited_channel_post":
                    return UpdateType.EditedChannelPost;
                case "edited_message":
                    return UpdateType.EditedMessage;
                case "inline_query":
                    return UpdateType.InlineQuery;
                case "message":
                    return UpdateType.Message;
                case "pre_checkout_query":
                    return UpdateType.PreCheckoutQuery;
                case "shipping_query":
                    return UpdateType.ShippingQuery;
                default:
                    return UpdateType.Unknown;
            }
        }

        public static string GetString(ChatAction ca)
        {
            switch (ca)
            {
                case ChatAction.FindLocation:
                    return "find_location";
                case ChatAction.RecordAudio:
                    return "record_audio";
                case ChatAction.RecordVideo:
                    return "record_video";
                case ChatAction.RecordVideoNote:
                    return "record_video_note";
                case ChatAction.Typing:
                    return "typing";
                case ChatAction.UploadAudio:
                    return "upload_audio";
                case ChatAction.UploadDocument:
                    return "upload_document";
                case ChatAction.UploadPhoto:
                    return "upload_photo";
                case ChatAction.UploadVideo:
                    return "upload_video";
                case ChatAction.UploadVideoNote:
                    return "upload_video_note";
                default:
                    return null;
            }
        }

        public static ChatAction GetChatAction(string str)
        {
            switch (str)
            {
                case "upload_photo":
                    return ChatAction.UploadPhoto;
                case "record_video":
                    return ChatAction.RecordVideo;
                case "upload_video":
                    return ChatAction.UploadVideo;
                case "record_audio":
                    return ChatAction.RecordAudio;
                case "upload_audio":
                    return ChatAction.UploadAudio;
                case "upload_document":
                    return ChatAction.UploadDocument;
                case "find_location":
                    return ChatAction.FindLocation;
                case "record_video_note":
                    return ChatAction.RecordVideoNote;
                case "upload_video_note":
                    return ChatAction.UploadVideoNote;
                case "typing":
                default:
                    return ChatAction.Typing;
            }
        }
    }
}
