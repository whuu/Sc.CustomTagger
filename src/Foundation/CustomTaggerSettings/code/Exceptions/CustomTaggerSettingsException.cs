using System;

namespace Sc.CustomTagger.Settings.Exceptions
{
    public class CustomTaggerSettingsException : Exception
    {
        public CustomTaggerSettingsException()
        {
        }

        public CustomTaggerSettingsException(string message) : base(message)
        {
        }

        public CustomTaggerSettingsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}