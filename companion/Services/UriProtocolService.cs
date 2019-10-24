using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace companion.Services
{
    public class UriProtocolService : IUriProtocolService
    {
        public bool RegisterUriScheme()
        {
            string applicationLocation = typeof(App).Assembly.Location;
            var currRegister = Properties.Settings.Default.UriProtocol;

            if (string.IsNullOrEmpty(currRegister) || string.Compare(currRegister, GetCommandValue(applicationLocation)) != 0)
            {
                AddOrUpdateRegistryEntry(applicationLocation);
                return true;
            }
            return false;
        }

        public string GetUriParams(string[] args)
        {
            var input = args[0];
            input = input.Remove(0, Properties.Settings.Default.UriScheme.Length + 3);

            return input;
        }

        public string GetCurrentRegistryCommand()
        {
            var res = "null";
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\" + Properties.Settings.Default.UriScheme);
            if (key != null)
            {
                var command = key.OpenSubKey(@"shell\open\command");
                res = command.GetValue("").ToString();
            }
            return res;
        }

        private static void AddOrUpdateRegistryEntry(string applicationLocation)
        {
            var uriScheme = Properties.Settings.Default.UriScheme;
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\" + uriScheme);
            if (key != null)
            {
                var command = key.OpenSubKey(@"shell\open\command", true);
                command.SetValue("", GetCommandValue(applicationLocation));
            }
            else
            {

                using (key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + uriScheme))
                {
                    key.SetValue("", "URL:" + Properties.Settings.Default.UriFriendlyName);
                    key.SetValue("URL Protocol", "");

                    using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                    {
                        defaultIcon.SetValue("", applicationLocation + ",1");
                    }
                    using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                    {
                        commandKey.SetValue("", GetCommandValue(applicationLocation));
                    }
                }

            }
            Properties.Settings.Default.UriProtocol = GetCommandValue(applicationLocation);
            Properties.Settings.Default.Save();
        }

        private static string GetCommandValue(string applicationLocation) => $"\"{applicationLocation}\" \"%1\"";
    }
}