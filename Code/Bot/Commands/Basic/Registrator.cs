using PolyChessTGBot.Logs;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PolyChessTGBot.Bot.Commands.Basic
{
    internal class CommandRegistrator
    {
        public List<Command> Commands;

        public CommandRegistrator()
        {
            Commands = [];
        }

        public void RegisterCommands(object obj, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var method in obj.GetType().GetMethods(flags))
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                    continue;
                CommandDelegate? commandDelegate = null;
                commandDelegate = (CommandDelegate)Delegate.CreateDelegate(typeof(CommandDelegate), obj, method);
                if (commandDelegate != null)
                {
                    var command = new Command([commandAttribute.Name], commandAttribute.Description, commandAttribute.ScopeType, commandAttribute.Visible, commandAttribute.Admin, commandDelegate);

                    var equals = Commands.Where(c => c.Names.Contains(command.Name));
                    if (equals.Any())
                        Commands.Remove(equals.First());
                    Commands.Add(command);
                }
            }
        }

        public async Task<bool> ExecuteCommand(string text, Message message, User user, CancellationToken token)
        {
            string commandText = text.Remove(0, 1);

            int index = -1;
            for (int i = 0; i < commandText.Length; i++)
            {
                if (commandText[i] == ' ')
                {
                    index = i;
                    break;
                }
            }
            string commandName = index < 0 ? commandText.ToLower() : commandText[..index].ToLower();
            List<string> commandArgs = index < 0 ? []: Utils.ParseParameters(commandText[index..]);

            CommandArgs args = new(message, Program.Bot.Telegram, user, commandArgs, token);
            foreach (var command in Commands)
                if (command.Names.Contains(commandName))
                {
                    Program.Logger.Write($"Получена команда: '{message.Text}'. {(args.Parameters.Count > 0 ? $"Аргументы: {string.Join(", ", args.Parameters)}" : "Аргументов нет")}", LogType.Info);

                    if (!command.AdminCommand || Program.MainConfig.Admins.Contains(user.Id))
                    {
                        #if !DEBUG
                        try
                        {
                        #endif
                            await command.Delegate(args);
                        #if !DEBUG
                        }
                        catch (Exception e)
                        {
                            Program.Logger.Write(e.ToString(), LogType.Error);
                            await args.Reply("Произошла ошибка при выполнении команды! Обратитесь к вашему системному администратору");
                        }
                        #endif
                    }
                    else
                        await args.Reply("Эта команда доступна только админам!");
                    return true;
                }
            return false;
        }

        public Dictionary<BotCommandScopeType, List<BotCommand>> GetCommandsInTelegram()
        {
            Dictionary<BotCommandScopeType, List<BotCommand>> commands = [];
            foreach (var command in Commands)
                if (command.Visible)
                {
                    if (commands.TryGetValue(command.ScopeType, out var list))
                        list.Add(command.ToTelegramCommand());
                    else
                        commands.Add(command.ScopeType, [command.ToTelegramCommand()]);
                }
            return commands;
        }
    }
}