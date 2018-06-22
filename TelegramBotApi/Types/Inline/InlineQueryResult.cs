namespace TelegramBotApi.Types.Inline
{
    /// <summary>
    /// This object represents one result of an inline query. Telegram clients currently support results of the following 20 types:
    /// InlineQueryResultCachedAudio, 
    /// InlineQueryResultCachedDocument, 
    /// InlineQueryResultCachedGif, 
    /// InlineQueryResultCachedMpeg4Gif, 
    /// InlineQueryResultCachedPhoto, 
    /// InlineQueryResultCachedSticker, 
    /// InlineQueryResultCachedVideo, 
    /// InlineQueryResultCachedVoice, 
    /// InlineQueryResultArticle, 
    /// InlineQueryResultAudio, 
    /// InlineQueryResultContact, 
    /// InlineQueryResultGame, 
    /// InlineQueryResultDocument, 
    /// InlineQueryResultGif, 
    /// InlineQueryResultLocation,
    /// InlineQueryResultMpeg4Gif,
    /// InlineQueryResultPhoto, 
    /// InlineQueryResultVenue, 
    /// InlineQueryResultVideo, 
    /// InlineQueryResultVoice, 
    /// </summary>
    public abstract class InlineQueryResult
    {
        /// <summary>
        /// Type of the inline query result
        /// </summary>
        public abstract string Type { get; set; }
        /// <summary>
        /// Unique ID of this one result to be given back to the bot if this is selected
        /// </summary>
        public abstract string Id { get; set; }
    }
}
