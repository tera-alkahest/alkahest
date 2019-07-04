using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Reflection
{
    public static class ReflectionExtensions
    {
        static void CheckAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
        }

        public static string GetCompany(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        }

        public static string GetConfiguration(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration;
        }

        public static string GetCopyright(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        }

        public static string GetCulture(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyCultureAttribute>()?.Culture;
        }

        public static string GetDescription(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        }

        public static string GetFileVersion(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }

        public static string GetInformationalVersion(this Assembly assembly, bool stripExtra = false)
        {
            CheckAssembly(assembly);

            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (stripExtra && version != null)
            {
                var idx = version.IndexOf('+');

                if (idx != -1)
                    version = version.Substring(0, idx);
            }

            return version;
        }

        public static Dictionary<string, string> GetMetadata(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public static string GetProduct(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        }

        public static string GetTitle(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        }

        public static string GetTrademark(this Assembly assembly)
        {
            CheckAssembly(assembly);

            return assembly.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark;
        }
    }
}
