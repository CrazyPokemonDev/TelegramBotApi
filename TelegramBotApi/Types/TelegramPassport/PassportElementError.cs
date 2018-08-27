using TelegramBotApi.Enums;

namespace TelegramBotApi.Types.TelegramPassport
{
    /// <summary>
    /// This object represents an error in the Telegram Passport element which was submitted that should be resolved by the user. It should be one of:
    /// <see cref="PassportElementErrorDataField"/>,
    /// <see cref="PassportElementErrorFrontSide"/>,
    /// <see cref="PassportElementErrorReverseSide"/>,
    /// <see cref="PassportElementErrorSelfie"/>,
    /// <see cref="PassportElementErrorFile"/>,
    /// <see cref="PassportElementErrorFiles"/>,
    /// <see cref="PassportElementErrorTranslationFile"/>,
    /// <see cref="PassportElementErrorTranslationFiles"/>,
    /// <see cref="PassportElementErrorUnspecified"/>
    /// </summary>
    public abstract class PassportElementError
    {
        /// <summary>
        /// Error source
        /// </summary>
        public abstract string Source { get; set; }

        /// <summary>
        /// The section of the user's Telegram Passport which has the error
        /// </summary>
        public abstract EncryptedPassportElementType Type { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public abstract string Message { get; set; }
    }
}
