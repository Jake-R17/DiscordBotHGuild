using System;

namespace DiscordBotHGuild.Models
{
    public class Warning : Entity
    {
        public string GuildId { get; set; }
        public string MemberId { get; set; }
        public string WarnReason { get; set; }
        public DateTime WarnDate { get; set; }
    }
}
