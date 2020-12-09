using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCenterClient.Attributes;
using CommandLine;

namespace AppCenterClient.Commands
{
    public class CommandProcessor
    {
        private readonly ImmutableSortedDictionary<string, (AppCenterCommandAttribute, Type)> _commands;

        public CommandProcessor()
        {
            _commands = GetCommands();
        }

        public Task Process(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: AppCenterClient <command> [options]");
                Console.WriteLine(ToString());
                return Task.CompletedTask;
            }

            return ProcessInternal(args);
        }

        private Task ProcessInternal(string[] args)
        {
            var cmdGroup = args[0];
            var cmdCommand = args[1];
            var cmdFullName = $"{cmdGroup}:{cmdCommand}";
            if (_commands.TryGetValue(cmdFullName, out var cmdData))
            {
                args = args.Skip(2).ToArray();
                var cmdMeta = cmdData.Item1;
                var cmdType = cmdData.Item2;
                Console.WriteLine($"Run command: {cmdMeta.Group} {cmdMeta.Command}");
                return Parser.Default.ParseArguments(() => Activator.CreateInstance(cmdType)!, args)
                    .WithParsedAsync(o =>
                    {
                        if (o is AppCenterCommand command)
                        {
                            return command.Run();
                        }
                        return Task.FromException(new ArgumentException($"Wrong command type: {o.GetType()}"));
                    });
            }
            
            return Task.FromException(new ArgumentException($"Invalid command: {cmdGroup} {cmdCommand}"));
        }

        public override string ToString()
        {
            var stringifyCommands = new Dictionary<string, Dictionary<string, string>>();
            foreach (var (_, (appCenterCommandAttribute, _)) in _commands)
            {
                var group = appCenterCommandAttribute.Group;
                var command = appCenterCommandAttribute.Command;
                var help = appCenterCommandAttribute.Help;
                if (!stringifyCommands.ContainsKey(group))
                {
                    stringifyCommands[group] = new Dictionary<string, string> {{command, help}};
                }
                else
                {
                    stringifyCommands[group].Add(command, help);
                }
            }

            var sb = new StringBuilder("Commands: (Run <command> --help for specific command options)").AppendLine();
            foreach (var (group, data) in stringifyCommands)
            {
                sb.AppendLine($"- {group}:");
                foreach (var (command, help) in data)
                {
                    sb.AppendLine($"\t{command} - {help}");
                }
            }

            return sb.ToString();
        }

        private static ImmutableSortedDictionary<string, (AppCenterCommandAttribute, Type)> GetCommands() => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(AppCenterCommand)))
            .Where(t => t.GetCustomAttributes(typeof(AppCenterCommandAttribute), false).Length == 1)
            .ToImmutableSortedDictionary(t =>
            {
                var attribute = (AppCenterCommandAttribute) t.GetCustomAttributes(typeof(AppCenterCommandAttribute), false)[0]!;
                return $"{attribute.Group}:{attribute.Command}";
            }, t =>
            {
                var attribute = (AppCenterCommandAttribute) t.GetCustomAttributes(typeof(AppCenterCommandAttribute), false)[0]!;
                return (attribute, t);
            });
    }
}