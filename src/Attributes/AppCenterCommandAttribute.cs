using System;

namespace AppCenterClient.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AppCenterCommandAttribute : Attribute
    {
        public string Group { get; }
        public string Command { get; }
        public string Help { get; set; } = string.Empty;

        public AppCenterCommandAttribute(string group, string command)
        {
            Group = group;
            Command = command;
        }
    }
}