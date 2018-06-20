namespace TelegramBotApi.Enums
{
    /*
     * For the improbable case that there will be a parsemode added, 
     * don't forget to add it at the other occurrences of ParseMode switches as well
     * 
     * */
    /// <summary>
    /// The parse mode of a message
    /// </summary>
    public enum ParseMode
    {
        /// <summary>
        /// Markdown Style
        /// </summary>
        Markdown,

        /// <summary>
        /// HTML Style
        /// </summary>
        Html,

        /// <summary>
        /// No Markdown
        /// </summary>
        None,
    }
}
