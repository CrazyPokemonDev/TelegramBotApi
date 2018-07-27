using System;
using TelegramBotApi;
using TelegramBotApi.Types.Upload;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace TelegramBotApiTests
{
    [TestClass]
    public class TelegramBotTests
    {
        public static TelegramBot Bot;

        [ClassInitialize()]
        public static void TestInitializeBot(TestContext testContext)
        {
            Bot = new TelegramBot(Constants.ApiToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInitializeBotInvalidToken()
        {
            var invalidBot = new TelegramBot("InvalidToken");
        }

        [TestMethod]
        public async Task TestGetMeAsync()
        {
            var me = await Bot.GetMeAsync();
            Assert.AreEqual(me.Id, int.Parse(Constants.ApiToken.Split(':')[0]));
            Assert.AreEqual(me.Username, Constants.BotUsername);
            Assert.AreEqual(me.IsBot, true);
        }

        [TestMethod]
        public void TestGetMe()
        {
            var me = Bot.GetMe();
            Assert.AreEqual(me.Id, int.Parse(Constants.ApiToken.Split(':')[0]));
            Assert.AreEqual(me.Username, Constants.BotUsername);
            Assert.AreEqual(me.IsBot, true);
        }

        [TestMethod]
        public async Task TestSendTextMessageAsync()
        {
            var text = "Test Text";
            var msg = await Bot.SendTextMessageAsync(Constants.ChatId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.ChatId);

            msg = await Bot.SendTextMessageAsync(Constants.GroupId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.GroupId);

            msg = await Bot.SendTextMessageAsync(Constants.SupergroupId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.SupergroupId);

            msg = await Bot.SendTextMessageAsync(Constants.ChannelId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Username, Constants.ChannelId.Trim('@'));
        }

        [TestMethod]
        public void TestSendTextMessage()
        {
            var text = "Test Text";
            var msg = Bot.SendTextMessage(Constants.ChatId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.ChatId);

            msg = Bot.SendTextMessage(Constants.GroupId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.GroupId);

            msg = Bot.SendTextMessage(Constants.SupergroupId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Id, Constants.SupergroupId);

            msg = Bot.SendTextMessage(Constants.ChannelId, text);
            Assert.AreEqual(msg.Text, text);
            Assert.AreEqual(msg.Chat.Username, Constants.ChannelId.Trim('@'));
        }

        [TestMethod]
        public async Task TestForwardMessageAsync()
        {
            var text = "To be forwarded";
            var msg = await Bot.SendTextMessageAsync(Constants.ChatId, text);

            var forwarded = await Bot.ForwardMessageAsync(Constants.GroupId, Constants.ChatId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = forwarded;
            forwarded = await Bot.ForwardMessageAsync(Constants.SupergroupId, Constants.GroupId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = forwarded;
            forwarded = await Bot.ForwardMessageAsync(Constants.ChannelId, Constants.SupergroupId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = await Bot.SendTextMessageAsync(Constants.ChannelId, text);
            forwarded = await Bot.ForwardMessageAsync(Constants.GroupId, Constants.ChannelId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.Chat.Username, forwarded.ForwardFromChat.Username);
        }

        [TestMethod]
        public void TestForwardMessage()
        {
            var text = "To be forwarded";
            var msg = Bot.SendTextMessage(Constants.ChatId, text);

            var forwarded = Bot.ForwardMessage(Constants.GroupId, Constants.ChatId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = forwarded;
            forwarded = Bot.ForwardMessage(Constants.SupergroupId, Constants.GroupId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = forwarded;
            forwarded = Bot.ForwardMessage(Constants.ChannelId, Constants.SupergroupId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.From.Id, forwarded.ForwardFrom.Id);

            msg = Bot.SendTextMessage(Constants.ChannelId, text);
            forwarded = Bot.ForwardMessage(Constants.GroupId, Constants.ChannelId, msg.MessageId);
            Assert.AreEqual(msg.Text, forwarded.Text);
            Assert.AreEqual(msg.Chat.Username, forwarded.ForwardFromChat.Username);
        }
    }
}
