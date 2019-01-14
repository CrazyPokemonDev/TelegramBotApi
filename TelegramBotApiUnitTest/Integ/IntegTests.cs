using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            Config = JsonConvert.DeserializeObject<IntegrationConfiguration>(File.ReadAllText("IntegrationConfig.json"));
            Bot = new TelegramBot(Config.BotToken);
        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }
}
