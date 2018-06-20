using Newtonsoft.Json;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Types
{
    /// <summary>
    /// This object describes the position on faces where a mask should be placed by default.
    /// </summary>
    public class MaskPosition
    {
        [JsonProperty(PropertyName = "point", Required = Required.Always)]
        private string _point;

        /// <summary>
        /// The part of the face relative to which the mask should be placed. 
        /// One of “forehead”, “eyes”, “mouth”, or “chin”.
        /// </summary>
        public MaskPositionPointType Point
        {
            get
            {
                switch (_point)
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
            set
            {
                switch (value)
                {
                    case MaskPositionPointType.Forehead:
                        _point = "forehead";
                        break;
                    case MaskPositionPointType.Eyes:
                        _point = "eyes";
                        break;
                    case MaskPositionPointType.Mouth:
                        _point = "mouth";
                        break;
                    case MaskPositionPointType.Chin:
                        _point = "chin";
                        break;
                    default:
                        _point = "unknown";
                        break;
                }
            }
        }

        /// <summary>
        /// Shift by X-axis measured in widths of the mask scaled to the face size, from left to right. 
        /// For example, choosing -1.0 will place mask just to the left of the default mask position.
        /// </summary>
        [JsonProperty(PropertyName = "x_shift", Required = Required.Always)]
        public float XShift;

        /// <summary>
        /// Shift by Y-axis measured in heights of the mask scaled to the face size, from top to bottom. 
        /// For example, 1.0 will place the mask just below the default mask position.
        /// </summary>
        [JsonProperty(PropertyName = "y_shift", Required = Required.Always)]
        public float YShift;

        /// <summary>
        /// Mask scaling coefficient. For example, 2.0 means double size.
        /// </summary>
        [JsonProperty(PropertyName = "scale", Required = Required.Always)]
        public float Scale;
    }
}
