﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotApiUnitTest.Integ
{
    [JsonObject]
    public  class IntegrationConfiguration
    {
        [JsonProperty(PropertyName = "bot_token")]
        public string BotToken { get; set; }

        [JsonProperty(PropertyName = "admin_user_id")]
        public int AdminUserId { get; set; }

        [JsonProperty(PropertyName = "non_admin_user_id")]
        public int NonAdminUserId { get; set; }

        [JsonProperty(PropertyName = "testing_group_id")]
        public long TestingGroupId { get; set; }
    }
}
