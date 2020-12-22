﻿#region .NET
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
    public class AvoidConfusionCommands:BaseCommandModule
    {
        [
            /*  Command name:ping
             *  Command description:Measures the bot latency.
             * 
             * Note:
             *  "DescriptionAttribute" declares the description of the command.
             *  "CommandAttribute" declares the name for the command.
             */

            /* Komut adı: ping
             * Komut açıklaması: Botun gecikmesini ölçer.
             *  
             */
            Command("ping"),
            Description("Bot gecikmesini ölçer.")
        ]
        //Measures the bot latency.
        //Botun gecikme süresini ölçer.
        public async Task Ping(CommandContext context)
        {
            int result = context.Client.Ping;
            var _embed = new DiscordEmbedBuilder();
            var random = new Random();
            (float r, float g, float b) = (((float)random.NextDouble()) , ((float)random.NextDouble()), ((float)random.NextDouble()));

            try
            {
                _embed.Color = new Optional<DiscordColor>(new DiscordColor(r,g,b));
                _embed.Title = "Botun cevap verme süresi ölçülüyor...";
                _embed.Author = new() { Name = "AvoidConfusion - Karmaşaları çözen Discord botu."};
                _embed.AddField("Bot Gecikme Süresi (milisaniye):", $"{result} ms");
                _embed.Description = $"{result} ms içinde yanıt verildi.";
                _ = await context.RespondAsync(embed: _embed.Build());
            }
            catch(Exception ex)
            {
                _embed.WithTitle("Komut Çakıldı!");
                _embed.WithColor(new(255, 0, 0));
                _embed.WithDescription($"{ex}");
                _ = await context.RespondAsync(embed: _embed.Build());
            }
            
        }

        [
            Command("begin-discussion"),
            Description("Bir konuşma başlatır.")
        ]
        //Command for starting a conversation.
        //Konuşma başlatmak için komut.
        public async Task InitConversation
            (
             CommandContext context,
             [Description("Konuşmanın başlığı.")]
             string conversationTitle,
             [Description("Konuşmanın açıklaması.")]
             string conversationDescription
            )
        {
            DiscordEmbedBuilder embedBuilder = new();
            embedBuilder.Color = new Optional<DiscordColor>(new DiscordColor(255, 0, 127));
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor();
            embedBuilder.Author.Name = "AvoidConfusion - Karmaşaları çözen Discord botu.";
            embedBuilder.Title = "Bu komut yapım aşamasındadır.";
            await context.RespondAsync(embed: embedBuilder.Build());
        }


    }
    public class AvoidConfusionHelper:BaseHelpFormatter
    {
        protected DiscordEmbedBuilder _embed;
        protected StringBuilder _strBuilder;

        public AvoidConfusionHelper(CommandContext context):base(context)
        {
            _embed = new();
            _strBuilder = new();
            _embed.Color = new(new(0,200,255));
        }
        public override BaseHelpFormatter WithCommand(Command command)
        {
            _embed.AddField(command.Name, command.Description);
            _strBuilder.AppendLine($@"{command.Name}\: {command.Description}");

            return (BaseHelpFormatter) this;
        }
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subCommands)
        {
            foreach(var subCommand in subCommands)
            {
                _embed.AddField(subCommand.Name, subCommand.Description);
                _strBuilder.AppendLine($@"{subCommand.Name}\: {subCommand.Description}");
            }
            return this;
        }
        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(content: _strBuilder.ToString(), embed: _embed.Build());
        }
    }
}


