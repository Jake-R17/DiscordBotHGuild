using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotHGuild.Models
{
    public class MutedUser : Entity
    {
        public string MemberId { get; set; }
        public string MutedReason { get; set; }
        public DateTime MutedExpiration { get; set; }
    }
}
