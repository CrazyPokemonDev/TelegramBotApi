namespace TelegramBotApi.Enums
{
    /*
     * IF YOU ADD SOMETHING, ADD IT TO ChatMember.cs TOO!
     * 
     * */
    /// <summary>
    /// Status of a chat member in a chat
    /// </summary>
    public enum ChatMemberStatus
    {
        /// <summary>
        /// The creator of a chat
        /// </summary>
        Creator,
        /// <summary>
        /// An administrator of a chat
        /// </summary>
        Administrator,
        /// <summary>
        /// A regular member of a chat
        /// </summary>
        Member,
        /// <summary>
        /// A restricted chat member
        /// </summary>
        Restricted,
        /// <summary>
        /// A member that has left
        /// </summary>
        Left,
        /// <summary>
        /// A member that has been kicked and can't return
        /// </summary>
        Kicked,
        /// <summary>
        /// A chat member status that isn't implemented yet
        /// </summary>
        Unknown
    }
}
