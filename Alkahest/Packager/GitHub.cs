using Octokit;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Alkahest.Packager
{
    static class GitHub
    {
        public static GitHubClient Client { get; } =
            new GitHubClient(new ProductHeaderValue(nameof(Alkahest), Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion));

        public static object GetObject(Uri uri)
        {
            return Client.Connection.Get<object>(uri, new Dictionary<string, string>(), null).Result.HttpResponse.Body;
        }

        public static string GetString(Uri uri)
        {
            return (string)GetObject(uri);
        }

        public static byte[] GetBytes(Uri uri)
        {
            return (byte[])GetObject(uri);
        }
    }
}
