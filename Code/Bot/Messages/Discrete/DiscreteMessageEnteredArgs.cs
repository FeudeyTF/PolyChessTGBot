﻿using PolyChessTGBot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PolyChessTGBot.Bot.Messages.Discrete
{
    public class DiscreteMessageEnteredArgs
    {
        public Message[] Responses;

        public TelegramBotClient Bot;

        public ChatId ChatID;

        public User User;

        public List<object> Data;

        public CancellationToken Token;

        public DiscreteMessageEnteredArgs(Message[] responses, TelegramBotClient bot, ChatId chat, User user, CancellationToken token, List<object> data)
        {
            Responses = responses;
            Bot = bot;
            ChatID = chat;
            User = user;
            Data = data;
            Token = token;
        }

        public async Task Reply(TelegramMessageBuilder message)
        {
            await Bot.SendMessage(message.WithToken(Token), ChatID);
        }
    }
}
