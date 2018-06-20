using Newtonsoft.Json;

namespace TelegramBotApi.Types.Game
{
    /// <summary>
    /// This object represents one row of the high scores table for a game.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GameHighScore
    {
        /// <summary>
        /// Position in high score table for the game
        /// </summary>
        [JsonProperty(PropertyName = "position", Required = Required.Always)]
        public int Position;

        /// <summary>
        /// User
        /// </summary>
        [JsonProperty(PropertyName = "user", Required = Required.Always)]
        public User Userd;

        /// <summary>
        /// Score
        /// </summary>
        [JsonProperty(PropertyName = "score", Required = Required.Always)]
        public int Score;
    }
}
