#region Using Statements - Using Deyimleri
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
using AvoidConfusion.Entities;
#endregion
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
            else if (conversationDescription is "")
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
                    
                    //Init a conversation channel.
                    //Bir konuşma kanalı açalım.
                    var convoChannel = await context.Guild.CreateChannelAsync
                        (
                            name:$"convo{((DateTime.Now.Ticks << 3) | (DateTime.Now.Ticks >> 61)):X8}",

                            ChannelType.Text,
                            topic: new Optional<string>(conversationDescription)
                        );
                    //Create a role for the conversation.
                    //Konuşma için bir rol yaratalım.
                    var conversatingRole = await context.Guild.CreateRoleAsync(name:"Conversator",mentionable:(bool?) false,permissions:Permissions.None);
                    
                    await convoChannel.AddOverwriteAsync(conversatingRole,allow:
                    (Permissions.SendMessages|Permissions.SendTtsMessages|Permissions.AttachFiles|Permissions.AccessChannels));

                    await convoChannel.AddOverwriteAsync(context.Guild.EveryoneRole, deny:(Permissions.SendMessages|Permissions.SendTtsMessages|Permissions.AttachFiles|Permissions.AccessChannels));

                    await ((DiscordMember)context.Message.Author).GrantRoleAsync(conversatingRole);
                    

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
