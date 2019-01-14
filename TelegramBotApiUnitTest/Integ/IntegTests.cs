using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelegramBotApi;

namespace TelegramBotApiUnitTest.Integ
{
    [TestClass]
    public class IntegTests
    {
        protected TelegramBot Bot;
        protected IntegrationConfiguration Config;

        [TestInitialize]
        public void Setup()
        {
            Bot = new TelegramBot(Config.BotToken);
        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }
}
