﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandleBarsRegex
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            handlebarsContext.RegisterHelper("Regex.Match", (writer, context, arguments) =>
            {
                (string stringToProcess, string regexPattern, object defaultValue) = ParseArguments(arguments);

                Match match = Regex.Match(stringToProcess, regexPattern);

                if (match.Success)
                {
                    writer.WriteSafeString(match.Value);
                }
                else if (defaultValue != null)
                {
                    writer.WriteSafeString(defaultValue);
                }
            });

            handlebarsContext.RegisterHelper("Regex.Match", (writer, options, context, arguments) =>
            {
                (string stringToProcess, string regexPattern, object defaultValue) = ParseArguments(arguments);

                var regex = new Regex(regexPattern);
                var namedGroups = RegexUtils.GetNamedGroups(regex, stringToProcess);
                if (namedGroups.Any())
                {
                    options.Template(writer, namedGroups);
                }
                else if (defaultValue != null)
                {
                    options.Template(writer, defaultValue);
                }
            });
        }

        private static (string stringToProcess, string regexPattern, object defaultValue) ParseArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 2 || args.Length == 3, nameof(arguments));

            string ParseAsString(object arg)
            {
                if (arg is string argAsString)
                {
                    return argAsString;
                }

                throw new NotSupportedException($"The value '{arg}' with type '{arg?.GetType()}' cannot be used in Handlebars Regex.");
            }

            return (ParseAsString(arguments[0]), ParseAsString(arguments[1]), arguments.Length == 3 ? arguments[2] : null);
        }
    }
}