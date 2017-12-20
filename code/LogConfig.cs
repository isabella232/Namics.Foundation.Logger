// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfig.cs" company="Namics AG">
//   (c) Namics AG
//
//  Want to work for one of Europe's leading Sitecore Agencies? 
//  Check out http://www.namics.com/jobs/
// </copyright>
// <summary>
//   Defines the LogConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Namics.Foundation.Logger
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using Sitecore.Configuration;
    using Sitecore.Data.Items;

    /// <summary>
    /// Handles runtime and settings file configuration.
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// Defines the config file entry name for the path to the config item
        /// </summary>
        private const string CONFIG_ITEM_SETTING_KEY = "Namics.Debug.ConfigItemPath";

        /// <summary>
        /// Defines the config file entry name for log level
        /// </summary>
        private const string LOG_LEVEL_SETTING_KEY = "Namics.Debug.LogLevel";

        /// <summary>
        /// Defines the config file entry name for log processors
        /// </summary>
        private const string LOG_PROCESSORS_SETTING_KEY = "Namics.Debug.LogProcessors";

        /// <summary>
        /// Defines the config file entry name for for allowed IPs
        /// </summary>
        private const string CONSOLE_ALLOWED_IP_SETTING_KEY = "Namics.Debug.Console.AllowedIp";

        /// <summary>
        /// Defines the item field name name for log level
        /// </summary>
        private const string LOG_LEVEL_FIELD_KEY = "LogLevel";

        /// <summary>
        /// Defines the item field name name for log processors
        /// </summary>
        private const string LOG_PROCESSORS_FIELD_KEY = "LogProcessors";

        /// <summary>
        /// Defines the item field name name for allowed IPs
        /// </summary>
        private const string CONSOLE_ALLOWED_IP_FIELD_KEY = "Console AllowedIp";

        /// <summary>
        /// Gets the log level from the configuration item or config file.
        /// </summary>
        public static LogLevel LogLevel 
        {
            get
            {
                try
                {
                    var logSetting = GetConfigFieldValue(LOG_LEVEL_FIELD_KEY, LOG_LEVEL_SETTING_KEY);

                    LogLevel logLevel;
                    if (Enum.TryParse(logSetting, true, out logLevel))
                    {
                        return logLevel;
                    }

                    return LogLevel.NOTSET;
                }
                catch (Exception)
                {
                    return LogLevel.NOTSET;
                }
            }
        }

        /// <summary>
        /// Gets the log processors assembly names.
        /// </summary>
        public static List<string> LogProcessors
        {
            get
            {
                var processors = new List<string>();
                foreach (var processor in LogProcessorsWithDebugLevel)
                {
                    processors.Add(processor.Split(':')[0]);
                }

                return processors;
            }
        }

        /// <summary>
        /// Gets the log processors assembly names.
        /// </summary>
        public static List<string> LogProcessorsWithDebugLevel
        {
            get
            {
                var processorsRaw = GetConfigFieldValue(LOG_PROCESSORS_FIELD_KEY, LOG_PROCESSORS_SETTING_KEY);

                processorsRaw = processorsRaw.Replace("\n", string.Empty);

                var processors = new List<string>();

                foreach (var processor in processorsRaw.Split('|'))
                {
                    processors.Add(processor.Trim());
                }

                return processors;
            }
        }

        /// <summary>
        /// Gets the console allowed ip.
        /// </summary>
        public static List<string> ConsoleAllowedIp
        {
            get
            {
                var valuesRaw = GetConfigFieldValue(CONSOLE_ALLOWED_IP_FIELD_KEY, CONSOLE_ALLOWED_IP_SETTING_KEY);

                valuesRaw = valuesRaw.Replace("\n", string.Empty);

                var ips = new List<string>();

                foreach (var processor in valuesRaw.Split('|'))
                {
                    ips.Add(processor.Trim());
                }

                return ips;
            }
        }

        /// <summary>
        /// Gets the config item.
        /// </summary>
        private static Item ConfigItem
        {
            get
            {
                var configItemPath = Settings.GetSetting(CONFIG_ITEM_SETTING_KEY, string.Empty).Trim();
                Item configItem = null;

                if (!string.IsNullOrEmpty(configItemPath))
                {
                    configItem = Sitecore.Context.Database.GetItem(configItemPath);
                }

                return configItem;
            }
        }

        /// <summary>
        /// The get log level.
        /// </summary>
        /// <param name="pProcessorName">
        /// The p processor name.
        /// </param>
        /// <returns>
        /// The <see cref="LogLevel"/>.
        /// </returns>
        public static LogLevel GetLogLevel(string pProcessorName)
        {
            foreach (var logProcessor in LogProcessorsWithDebugLevel.Where(pItem => !string.IsNullOrEmpty(pItem) && pItem.Split(':')[0].Equals(pProcessorName)))
            {
                var splittedlogProcessor = logProcessor.Split(':');

                if (splittedlogProcessor.Length > 1)
                {
                    if (!string.IsNullOrEmpty(splittedlogProcessor[1]))
                    {
                        LogLevel logLevel;
                        if (Enum.TryParse(splittedlogProcessor[1], true, out logLevel))
                        {
                            return logLevel;
                        }

                        return LogLevel.NOTSET;
                    }
                }
            }

            return LogLevel;
        }

        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="fieldKey">
        /// The field key of the Settings item.
        /// </param>
        /// <param name="configFileKey">
        /// The config setting key for fallback-
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        private static string GetConfigFieldValue(string fieldKey, string configFileKey)
        {
            var configItem = ConfigItem;

            var setting = string.Empty;
            if (configItem != null)
            {
                setting = configItem[fieldKey];
            }

            // Fallback to config file
            if (string.IsNullOrEmpty(setting))
            {
                setting = Settings.GetSetting(configFileKey, string.Empty).Trim();
            }

            return setting.Trim();
        }
    }
}
