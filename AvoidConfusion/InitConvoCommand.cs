#region .NET
using System;
using System.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
#endregion

#region D#+
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using Newtonsoft.Json;
using DSharpPlus.CommandsNext.Entities;
#endregion

namespace AvoidConfusion
{
    public partial class AvoidConfusionCommands
    {

        //Command for starting a conversation.
        //Konuşma başlatmak için komut.
        [
             Command("begin-discussion"),
             Description("Bir konuşma başlatır.")
        ]
        public async Task InitConversation
            (
                CommandContext context,
                [Description("Konuşma başlığı.")]
                string conversationTitle,
                [Description("Konuşma açıklaması.")]
                string conversationDescription
            )
        {
            //The startup variables.
            //Başlangıç değişkenleri.
            DiscordEmbedBuilder resultEmbed = new();
            resultEmbed.Author = new() { Name = "AvoidConfusion - Karmaşaları çözen Discord botu." };
            if ((conversationTitle is "") || (conversationTitle.Length is < 1 or > 250))
            {
                //We need valid values.
                //Geçerli değerlere ihtiyacımız var.
                resultEmbed.Title = "Hata";
                resultEmbed.Color = new(new DiscordColor(255, 0, 0));
                resultEmbed.Description = "Konuşma başlığı boş, 250 karakterden uzun veya 1 karakterden kısa.";
                await context.RespondAsync(embed: resultEmbed.Build());
            }
            else if ((conversationDescription is ""))
            {
                //An invalid value is given.
                //Yine geçersiz bir değer girildi.
                resultEmbed.Title = "Hata";
                resultEmbed.Color = new(new DiscordColor(255, 0, 0));
                resultEmbed.Description = "Konuşma açıklaması boş";
                await context.RespondAsync(embed: resultEmbed.Build());
            }
            else
            {
#nullable enable
                try
                {
                    //Connect to the database.
                    //Veritabanına bağlanalım.
                    var connectionString = "Data Source=MasherCoder;Initial Catalog=AvoidConfusion; Username=Bot";
                    using SqlConnection connection = new(connectionString);
                    await connection.OpenAsync();
                    //Get the snowflake of the guild where the command was sent.
                    //Komutun gönderildiği sunucunun kar tanesini alalım.
                    var snowflakeCommandSenderGuild = new SqlBinary((byte[]?)(BitConverter.GetBytes(context.Guild.Id)));
                    while (connection.State != ConnectionState.Open)
                    {
                        //Do nothing while the connection gets opened.
                        //Bağlantı açılmadıkça bir şey yapma.
                    }

                    //Do something when the connection is open.
                    //Bağlantı açılınca bir şeyler yap.
                    var getSnowflakeCommand = connection.CreateCommand();
                    //Get guild snowflakes from the database.
                    //Veritabanından lonca kar tanelerini al.
                    getSnowflakeCommand.CommandText = "SELECT [Snowflake] FROM [dbo].[Guilds]";
                    getSnowflakeCommand.CommandTimeout = 10;
                    getSnowflakeCommand.CommandType = CommandType.Text;
                    var dbDataReader = await getSnowflakeCommand.ExecuteReaderAsync();
                    //If the snowflake of this guild is not in the database, 
                    //register the guild to the database (but not set the conversation supervisior role.).
                    //Eğer bu sunucunun kar tanesi veritabanında yoksa, 
                    //bu sunucuyu veritabanına kaydedelim (ama konuşma gözlemcisi rolünü ayarlamayalım.).
                    bool guildRecorded = false;
                    for (; dbDataReader.Read();)
                    {
                        if ((SqlBinary)(dbDataReader["[Snowflake]"]) == snowflakeCommandSenderGuild)
                        {
                            //The guild is recorded to our database...
                            //Sunucu veritabanımıza kayıtlıymış...
                            guildRecorded = true;
                        }
                    }

                    if (!guildRecorded)
                    {
                        //Record the guild to the database.
                        //Sunucuyu veritabanına kaydet.
                        var recordGuildCommand = connection.CreateCommand();
                        var everyoneRole = new SqlBinary(BitConverter.GetBytes(context.Guild.EveryoneRole.Id));
                        recordGuildCommand.CommandType = CommandType.Text;
                        recordGuildCommand.CommandText = $"INSERT INTO [dbo].[Guilds] ([Snowflake], [ConversationSupervisiorRoleSnowflake]) VALUES (@GuildSnowflake,@ConvoSnowflake)";
                        recordGuildCommand.Parameters.AddWithValue("@GuildSnowflake", snowflakeCommandSenderGuild);
                        recordGuildCommand.Parameters.AddWithValue("@ConvoSnowflake", everyoneRole);
                        await recordGuildCommand.ExecuteNonQueryAsync();

                    }
                    //Init a conversation channel.
                    //Bir konuşma kanalı açalım.
                    var convoChannel = await context.Guild.CreateChannelAsync
                        (
                            $"convo{((DateTime.Now.Ticks << 3) | (DateTime.Now.Ticks >> 61)):X10}",
                            ChannelType.Text,
                            topic: new Optional<string>(conversationDescription)
                        );
                    //Create a role for the conversation.
                    //Konuşma için bir rol yaratalım.


                    //Record the conversation channel to the channel.
                    //Sunucuya konuşma kanalını kaydedelim.
                    var getGuildIDCommand = connection.CreateCommand();
                    getGuildIDCommand.CommandType = CommandType.Text;
                    getGuildIDCommand.CommandText = "SELECT [Id] FROM [dbo].[Guilds] WHERE [Snowflake] = @GuildSnowflake";
                    getGuildIDCommand.Parameters.AddWithValue("@GuildSnowflake", snowflakeCommandSenderGuild);
                    await getGuildIDCommand.ExecuteScalarAsync();

                    var recordChannelCommand = connection.CreateCommand();
                    recordChannelCommand.CommandType = CommandType.Text;
                    recordChannelCommand.CommandText = "INSERT INTO [dbo].[Conversations] ([GuildID], [ChannelSnowflake], [ConversatingUserRoleSnowflake],) VALUES ()";

#nullable disable
                }
                catch (Exception excp)
                {
                    //Bir hata oluştu.
                    resultEmbed.Title = "Beklenmeyen Özel Durum";
                    resultEmbed.Color = new(new DiscordColor(255, 10, 10));
                    resultEmbed.Description = $"{excp.GetType().FullName} türünden istisna";
                    resultEmbed.AddField("Hata Mesajı", excp.Message);
                    resultEmbed.AddField("Hata Kaynağı", excp.Source ?? "Bilinmiyor");
                    resultEmbed.AddField("Tam Hata Kaynağı", excp.StackTrace ?? "Bilinmiyor");
                    resultEmbed.AddField("Yardım linki", excp.HelpLink ?? "(Bulunamadı)");

                    await context.RespondAsync(isTTS: true, embed: resultEmbed.Build());
                }

            }
        }
    }
}
