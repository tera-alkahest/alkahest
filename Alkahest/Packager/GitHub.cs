using Alkahest.Core.Reflection;
using Octokit;
using System;
using System.Reflection;

namespace Alkahest.Packager
{
    static class GitHub
    {
        public static GitHubClient Client { get; } =
            new GitHubClient(new ProductHeaderValue(nameof(Alkahest),
                Assembly.GetExecutingAssembly().GetInformationalVersion(true)));

        static GitHub()
        {
            Client.SetRequestTimeout(Configuration.AssetTimeout);
        }

        public static object GetObject(Uri uri)
        {
            return Client.Connection.Get<object>(uri, Configuration.AssetTimeout).Result.HttpResponse.Body;
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
