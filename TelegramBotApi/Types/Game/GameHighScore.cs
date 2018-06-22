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
        public int Position { get; set; }

        /// <summary>
        /// User
        /// </summary>
        [JsonProperty(PropertyName = "user", Required = Required.Always)]
        public User User { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        [JsonProperty(PropertyName = "score", Required = Required.Always)]
        public int Score { get; set; }
    }
}
