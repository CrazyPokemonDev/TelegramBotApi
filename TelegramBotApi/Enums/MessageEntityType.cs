namespace TelegramBotApi.Enums
{
    /*
     * 
     * IF YOU ADD A TYPE HERE, ADD IT IN MessageEntity.cs AS WELL!!!!!!!!
     * ADD IT IN BOTH THE GETTER AND THE SETTER OF Type!
     * 
     */

    /// <summary>
    /// Type of a special text entity
    /// </summary>
    public enum MessageEntityType
    {
        /// <summary>
        /// @username
        /// </summary>
        Mention,
        /// <summary>
        /// #hashtag
        /// </summary>
        Hashtag,
        /// <summary>
        /// /bot_command
        /// </summary>
        BotCommand,
        /// <summary>
        /// www.url.com
        /// </summary>
        Url,
        /// <summary>
        /// email@adress.com
        /// </summary>
        Email,
        /// <summary>
        /// Bold text
        /// </summary>
        Bold,
        /// <summary>
        /// Italic text
        /// </summary>
        Italic,
        /// <summary>
        /// Monowidth string
        /// </summary>
        Code,
        /// <summary>
        /// Monowidth block
        /// </summary>
        Pre,
        /// <summary>
        /// Clickable text URLs
        /// </summary>
        TextLink,
        /// <summary>
        /// Mentioning a user without username
        /// </summary>
        TextMention,
        /// <summary>
        /// A phone number
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// A cashtag
        /// </summary>
        Cashtag,
        /// <summary>
        /// A message entity type that isn't implemented yet
        /// </summary>
        Unknown
    }
}
