using DSharpPlus;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DiscordEmojiScraper
{
    class Program
    {
        private static DiscordClient discordClient = null;
        static async void Main(string[] args)
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("Need token.txt with your token.");
                Console.ReadKey();
                return;
            }

            await doall();


            Console.ReadKey();

        }

        private static async Task doall()
        {
            int tokenTypeInt = 0;
            TokenType tokenType = (TokenType)tokenTypeInt;

            discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = File.ReadAllText("token.txt").Trim(),
                TokenType = tokenType, // Override stupid obsolete error
            });

            //discordClient.MessageCreated += DiscordClient_MessageCreated;
            /*discordClient.MessageCreated += async (e) =>
            {

                Task.Run(() => {
                    DiscordClient_MessageCreated(e);
                });
            };*/


            Task connectTask = discordClient.ConnectAsync();
            Console.WriteLine("Waiting for connect.");
            await connectTask;
            Console.WriteLine("Connected.");

            foreach(var kvp in discordClient.Guilds)
            {
                ulong guildId = kvp.Key;
                var guild = kvp.Value;
                foreach(var emoji in guild.Emojis)
                {
                    string emojiFilename = MakeValidFileName(emoji.GetDiscordName());
                    string url = emoji.
                }
            }
        }

        // from: https://stackoverflow.com/a/847251
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public static string getUrl(this DSharpPlus.Entities.DiscordEmoji me)
        {
            return me.Id == 0
                ? throw new InvalidOperationException("Cannot get URL of unicode emojis.")
                : me.
                ? $"https://cdn.discordapp.com/emojis/{me.Id.ToString(CultureInfo.InvariantCulture)}.gif"
                : $"https://cdn.discordapp.com/emojis/{me.Id.ToString(CultureInfo.InvariantCulture)}.png";
        }


    }
}
