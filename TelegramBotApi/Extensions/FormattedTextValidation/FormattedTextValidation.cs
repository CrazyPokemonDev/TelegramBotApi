using System.Linq;
using System.Xml.Linq;
using TelegramBotApi.Enums;

namespace TelegramBotApi.Extensions.FormattedTextValidation
{
    /// <summary>
    /// Custom extension class, implements methods to validate HTML- or Markdown-formatted text before sending it
    /// </summary>
    public static class FormattedTextValidation
    {
        #region HTML
        /// <summary>
        /// Validates whether the given text is correctly HTML-formatted and can be sent with <see cref="ParseMode.Html"/>
        /// </summary>
        /// <param name="str">The string for which the HTML-formatting is validated</param>
        /// <returns>Whether this string is correctly HTML-formatted and can be sent with <see cref="ParseMode.Html"/></returns>
        public static bool ValidateHTML(this string str)
        {
            try
            {
                return XDocument.Parse("<parse>" + str + "</parse>")
                    .Descendants().Skip(1).FirstOrDefault(x =>
                        (!new[] { "a", "b", "strong", "i", "em", "code", "pre" }.Contains(x.Name.LocalName)) ||
                        (x.Name.LocalName == "a" && x.Attributes().Single().Name.LocalName != "href") ||
                        (x.Name.LocalName != "a" && x.Attributes().Any()) ||
                        (x.Descendants().Any())) 
                    == null;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        // TO DO: ValidateMarkdown(this string str)
    }
}
