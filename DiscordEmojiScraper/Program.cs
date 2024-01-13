using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordEmojiScraper
{
    class Program
    {
        private static DiscordClient discordClient = null;
        static void Main(string[] args)
        {
            if (!File.Exists("token.txt"))
            {
                Console.WriteLine("Need token.txt with your token.");
                Console.ReadKey();
                return;
            }

            Task.Run(() => { 
                doall();
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            });


            Console.ReadKey();

        }

        private static async Task doall()
        {
            StringBuilder sb = new StringBuilder();

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

            HashSet<string> emojiNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var user = await discordClient.GetUserAsync(discordClient.CurrentUser.Id);

            ulong[] userGuilds = await discordClient.GetCurrentUserGuildsAsync();

            //var guilds = discordClient.Guilds;

            foreach (var guildId in userGuilds)
            {
                var guild = await discordClient.GetGuildAsync(guildId);
                foreach(var emoji in guild.Emojis)
                {
                    string emojiFilenameBase = MakeValidFileName(emoji.Name);
                    string emojiFilename = emojiFilenameBase;
                    int i = 2;
                    while (emojiNames.Contains(emojiFilename))
                    {
                        emojiFilename = $"{emojiFilenameBase}-{i++}";
                    }
                    emojiNames.Add(emojiFilename);
                    string url = emoji.Url;
                    emojiFilename = Path.Combine("emojis",$"{emojiFilename}{Path.GetExtension(url)}");
                    sb.Append($"wget -c --retry-connrefused --tries=0 --timeout=500 -O \"{emojiFilename}\" \"{url}\"\n");
                }
            }
            File.WriteAllText("getEmojis.sh", sb.ToString());
        }

        // from: https://stackoverflow.com/a/847251
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }


    }
}
