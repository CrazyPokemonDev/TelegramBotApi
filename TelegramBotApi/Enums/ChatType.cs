namespace TelegramBotApi.Enums
{
    /*
     * 
     * IF YOU ADD A TYPE HERE, ADD IT IN Chat.cs AS WELL!!!!!!!!
     * ADD IT IN BOTH THE GETTER AND THE SETTER OF Type!
     * 
     */

    /// <summary>
    /// Type of a chat
    /// </summary>
    public enum ChatType
    {
        /// <summary>
        /// A private chat
        /// </summary>
        Private,
        /// <summary>
        /// A group chat
        /// </summary>
        Group,
        /// <summary>
        /// A supergroup chat
        /// </summary>
        Supergroup,
        /// <summary>
        /// A channel
        /// </summary>
        Channel,
        /// <summary>
        /// An unimplemented type of chat
        /// </summary>
        Unknown
    }
}
