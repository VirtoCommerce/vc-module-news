using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.News.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "news:access";
            public const string Create = "news:create";
            public const string Read = "news:read";
            public const string Update = "news:update";
            public const string Delete = "news:delete";
            public const string Publish = "news:publish";

            public static string[] AllPermissions { get; } =
            [
                Access,
                Create,
                Read,
                Update,
                Delete,
                Publish,
            ];
        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor NewsEnabled { get; } = new()
            {
                Name = "News.Enabled",
                GroupName = "News|News",
                ValueType = SettingValueType.Boolean,
                DefaultValue = true,
                IsPublic = true,
            };

            public static SettingDescriptor UseNewsPrefixInLinks { get; } = new()
            {
                Name = "News.UseNewsPrefixInLinks",
                GroupName = "News|News",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
                IsPublic = true,
            };

            public static SettingDescriptor UseStoreDefaultLanguage { get; } = new()
            {
                Name = "News.UseStoreDefaultLanguage",
                GroupName = "News|News",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
                IsPublic = true,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return NewsEnabled;
                    yield return UseNewsPrefixInLinks;
                    yield return UseStoreDefaultLanguage;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }
}
