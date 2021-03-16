namespace DiscordBotHGuild.Models
{
    public class Warnings : Entity
    {
        public string GuildId { get; set; }
        public string MemberId { get; set; }
        public string WarnReason { get; set; }
        public int WarnAmount { get; set; }
    }
}
