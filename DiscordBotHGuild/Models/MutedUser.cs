﻿using System;

namespace DiscordBotHGuild.Models
{
    public class MutedUser : Entity
    {
        public string MemberId { get; set; }
        public string MutedReason { get; set; }
        public DateTime MutedExpiration { get; set; }
    }
}
