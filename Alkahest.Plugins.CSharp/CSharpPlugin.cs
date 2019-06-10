using Alkahest.Core;
using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Packets;
using Alkahest.Core.Plugins;
using Alkahest.Core.Reflection;
using EasyHook;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.DiaSymReader;
using Microsoft.JScript;
using Microsoft.VisualBasic;
using Mono.Linq.Expressions;
using Mono.Options;
using Newtonsoft.Json.Linq;
using Octokit;
using SharpDisasm;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Configuration.Install;
using System.Data;
using System.Data.EntityClient;
using System.Data.Linq;
using System.Data.Services;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Management.Instrumentation;
using System.Messaging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Numerics;
using System.Printing;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Runtime.DurableInstancing;
using System.Runtime.Remoting.Services;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Discovery;
using System.ServiceModel.Routing;
using System.ServiceProcess;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Xps.Packaging;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;
using Theraot;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace Alkahest.Plugins.CSharp
{
    public sealed class CSharpPlugin : IPlugin
    {
        const string PackageFile = "Main.cs";

        const string ScriptAssembly = nameof(Alkahest) + "Script";

        const string StartName = "__Start__";

        const string StopName = "__Stop__";

        const BindingFlags MethodFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Static |
            BindingFlags.Public;

        public string Name => "csharp";

        static readonly Color _prefixColor = Color.FromArgb(0, Color.Chartreuse);

        static readonly Log _log = new Log(typeof(CSharpPlugin));

        readonly PluginContext _context;

        readonly List<(string, Type, CSharpScriptContext)> _scripts =
            new List<(string, Type, CSharpScriptContext)>();

        readonly List<(CSharpScriptContext, string, Action<GameClient, string[]>)> _commands =
            new List<(CSharpScriptContext, string, Action<GameClient, string[]>)>();

#pragma warning disable IDE0051 // Remove unused private members
        CSharpPlugin(PluginContext context)
#pragma warning restore IDE0051 // Remove unused private members
        {
            _context = context;
        }

        internal void Message(GameClient client, string from, string format, params object[] args)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var msg = string.Format(
                "[<FONT COLOR=\"#{0:X}\">{1}</FONT>]{2} {3}",
                _prefixColor.ToArgb(), nameof(Alkahest), from == null ? string.Empty :
                string.Format("[{0}]", from), string.Format(format, args));

            client.SendToClient(new SChatPacket
            {
                Message = msg,
                Channel = ChatChannel.System,
            });
        }

        internal bool AddCommand(CSharpScriptContext context, string name,
            Action<GameClient, string[]> handler)
        {
            lock (_commands)
            {
                foreach (var (ctx, n, _) in _commands)
                    if (n == name)
                        return false;

                _commands.Add((context, name, handler));
            }

            return true;
        }

        internal bool RemoveCommand(CSharpScriptContext context, string name)
        {
            lock (_commands)
            {
                foreach (var (i, (ctx, n, _)) in _commands.WithIndex().ToArray())
                {
                    if (ctx == context && n == name)
                    {
                        _commands.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        bool HandleLoadClientUserSetting(GameClient client, Direction direction,
            SLoadClientUserSettingPacket packet)
        {
            Message(client, null, "Proxy version <FONT COLOR=\"#{0:X}\">{1}</FONT> connected",
                Color.FromArgb(0, Color.Aqua).ToArgb(),
                Assembly.GetExecutingAssembly().GetInformationalVersion());

            return true;
        }

        void HandleCommand(GameClient client, string command)
        {
            var args = new List<string>();
            var arg = new StringBuilder();

            void Push()
            {
                var str = arg.ToString().Trim();

                if (str != string.Empty)
                    args.Add(str);

                arg.Clear();
            }

            var quote = false;

            foreach (var c in command)
            {
                if (c == '"')
                {
                    if (quote)
                    {
                        Push();

                        quote = false;
                    }
                    else
                        quote = true;
                }
                else if (char.IsWhiteSpace(c) && !quote)
                    Push();
                else
                    arg.Append(c);
            }

            Push();

            if (args.Count < 1)
                return;

            var cmd = args[0];

            lock (_commands)
            {
                var handler = _commands.FirstOrDefault(x => x.Item2 == cmd).Item3;

                if (handler != null)
                {
                    try
                    {
                        handler(client, args.Skip(1).ToArray());
                    }
                    catch (Exception e) when (!Debugger.IsAttached)
                    {
                        _log.Error("Unhandled exception in command {0}:", cmd);
                        _log.Error("{0}", e);
                    }
                }
                else
                    Message(client, null, "Unknown command name: {0}", cmd);
            }
        }

        bool HandleOpCommand(GameClient client, Direction direction, COpCommandPacket packet)
        {
            // The client strips all HTML from this packet.
            HandleCommand(client, packet.Command);
            return false;
        }

        bool HandleAdmin(GameClient client, Direction direction, CAdminPacket packet)
        {
            // The client strips all HTML from this packet.
            HandleCommand(client, packet.Command);
            return false;
        }

        public void Start()
        {
            var pkg = Configuration.PackageDirectory;

            Directory.CreateDirectory(pkg);

            var compiler = CSharpCompilation.Create(ScriptAssembly, options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary).WithAllowUnsafe(true).WithDeterministic(true));

            // Reference all the important .NET Framework assemblies.
            var types = new[]
            {
                typeof(Assert),                  // Alkahest.Core.dll
                typeof(CSharpPlugin),            // alkahest-csharp
                typeof(RemoteHooking),           // EasyHook.dll
                typeof(SyntaxTree),              // Microsoft.CodeAnalysis.dll
                typeof(CSharpSyntaxTree),        // Microsoft.CodeAnalysis.CSharp.dll
                typeof(CSharpScript),            // Microsoft.CodeAnalysis.CSharp.Scripting.dll
                typeof(CSharpFormattingOptions), // Microsoft.CodeAnalysis.CSharp.Workspaces.dll
                typeof(TextTags),                // Microsoft.CodeAnalysis.Features.dll
                typeof(Script),                  // Microsoft.CodeAnalysis.Scripting.dll
                typeof(VisualBasicSyntaxTree),   // Microsoft.CodeAnalysis.VisualBasic.dll
                typeof(Solution),                // Microsoft.CodeAnalysis.Workspaces.dll
                typeof(RuntimeBinderException),  // Microsoft.CSharp.dll
                typeof(IMetadataImportProvider), // Microsoft.DiaSymReader.dll
                typeof(JSObject),                // Microsoft.JScript.dll
                typeof(TriState),                // Microsoft.VisualBasic.dll
                typeof(CustomExpression),        // Mono.Linq.Expressions.dll
                typeof(OptionSet),               // Mono.Options.dll
                typeof(JObject),                 // Newtonsoft.Json.dll
                typeof(object),                  // mscorlib.dll
                typeof(GitHubClient),            // Octokit.dll
                typeof(UIElement),               // PresentationCore.dll
                typeof(Window),                  // PresentationFramework.dll
                typeof(XpsDocument),             // ReachFramework.dll
                typeof(Disassembler),            // SharpDisasm.dll
                typeof(Uri),                     // System.dll
                typeof(Configuration),           // System.Configuration.dll
                typeof(Installer),               // System.Configuration.Install.dll
                typeof(ExportAttribute),         // System.ComponentModel.Composition.dll
                typeof(ExportBuilder),           // System.ComponentModel.Composition.Registration.dll
                typeof(Validator),               // System.ComponentModel.DataAnnotations.dll
                typeof(AttributedModelProvider), // System.Composition.AttributedModel.dll
                typeof(ConventionBuilder),       // System.Composition.Convention.dll
                typeof(CompositionHost),         // System.Composition.Hosting.dll
                typeof(CompositionContract),     // System.Composition.Runtime.dll
                typeof(ContainerConfiguration),  // System.Composition.TypedParts.dll
                typeof(Enumerable),              // System.Core.dll
                typeof(DataSet),                 // System.Data.dll
                typeof(DataTableExtensions),     // System.Data.DataSetExtensions.dll
                typeof(EntityConnection),        // System.Data.Entity.dll
                typeof(DataContext),             // System.Data.Linq.dll
                typeof(DataServiceHost),         // System.Data.Services.dll
                typeof(DataServiceContext),      // System.Data.Services.Client.dll
                typeof(Bitmap),                  // System.Drawing.dll
                typeof(ZipArchive),              // System.IO.Compression.dll
                typeof(ZipFile),                 // System.IO.Compression.FileSystem.dll
                typeof(ManagementObject),        // System.Management.dll
                typeof(InstrumentationManager),  // System.Management.Instrumentation.dll
                typeof(IPinnable),               // System.Memory.dll
                typeof(MessageQueue),            // System.Messaging.dll
                typeof(IPEndPointCollection),    // System.Net.dll
                typeof(HttpClient),              // System.Net.Http.dll
                typeof(JsonContractResolver),    // System.Net.Http.Formatting.dll
                typeof(WebRequestHandler),       // System.Net.Http.WebRequest.dll
                typeof(BigInteger),              // System.Numerics.dll
                typeof(Vector<>),                // System.Numerics.Vectors.dll
                typeof(PrintQueue),              // System.Printing.dll
                typeof(MemoryCache),             // System.Runtime.Caching.dll
                typeof(Unsafe),                  // System.Runtime.CompilerServices.Unsafe.dll
                typeof(InstanceHandle),          // System.Runtime.DurableInstancing.dll
                typeof(RemotingService),         // System.Runtime.Remoting.dll
                typeof(DataContractAttribute),   // System.Runtime.Serialization.dll
                typeof(SoapFormatter),           // System.Runtime.Serialization.Formatters.Soap.dll
                typeof(ProtectedData),           // System.Security.dll
                typeof(ChannelFactory),          // System.ServiceModel.dll
                typeof(ServiceHostFactory),      // System.ServiceModel.Activation.dll
                typeof(UdpBinding),              // System.ServiceModel.Channels.dll
                typeof(DiscoveryClient),         // System.ServiceModel.Discovery.dll
                typeof(RoutingService),          // System.ServiceModel.Routing.dll
                typeof(WebHttpBinding),          // System.ServiceModel.Web.dll
                typeof(ServiceBase),             // System.ServiceProcess.dll
                typeof(SpeechSynthesizer),       // System.Speech.dll
                typeof(ValueTask<>),             // System.Threading.Tasks.Extensions.dll
                typeof(Transaction),             // System.Transactions.dll
                typeof(HttpServer),              // System.Web.Http.dll
                typeof(HttpSelfHostServer),      // System.Web.Http.SelfHost.dll
                typeof(RibbonControl),           // System.Windows.Controls.Ribbon
                typeof(Form),                    // System.Windows.Forms.dll
                typeof(DataPoint),               // System.Windows.Forms.DataVisualization.dll
                typeof(XamlType),                // System.Xaml.dll
                typeof(XmlNode),                 // System.Xml.dll
                typeof(XNode),                   // System.Xml.Linq.dll
                typeof(No),                      // Theraot.Core.dll
                typeof(PinnedObject),            // Vanara.Core.dll
                typeof(ComCtl32),                // Vanara.PInvoke.ComCtl32.dll
                typeof(BCrypt),                  // Vanara.PInvoke.Cryptography.dll
                typeof(Gdi32),                   // Vanara.PInvoke.Gdi32.dll
                typeof(Kernel32),                // Vanara.PInvoke.Kernel32.dll
                typeof(Mpr),                     // Vanara.PInvoke.Mpr.dll
                typeof(NtDll),                   // Vanara.PInvoke.NtDll.dll
                typeof(AdvApi32),                // Vanara.PInvoke.Security.dll
                typeof(Lib),                     // Vanara.PInvoke.Shared.dll
                typeof(Shell32),                 // Vanara.PInvoke.Shell32.dll
                typeof(ShlwApi),                 // Vanara.PInvoke.ShlwApi.dll
                typeof(User32),                  // Vanara.PInvoke.User32.dll
                typeof(User32_Gdi),              // Vanara.PInvoke.User32.Gdi.dll
                typeof(WinINet),                 // Vanara.PInvoke.WinINet.dll
                typeof(Rect),                    // WindowsBase.dll
            };

            foreach (var type in types)
                compiler = compiler.AddReferences(MetadataReference.CreateFromFile(type.Assembly.Location));

            var options = CSharpParseOptions.Default.WithLanguageVersion(
                Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8);
            var typeNames = new List<(string, string)>();

            foreach (var dir in Directory.EnumerateDirectories(pkg))
            {
                var name = Path.GetFileName(dir);

                if (Configuration.DisablePackages.Contains(name))
                    continue;

                foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
                {
                    using var src = File.OpenRead(file);
                    var tree = Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(
                        SourceText.From(src, Encoding.UTF8), options, file);

                    compiler = compiler.AddSyntaxTrees(tree);

                    if (tree.FilePath == Path.Combine(dir, PackageFile))
                    {
                        var typeDecl = (from type in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                                        from method in type.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                        let methodName = method.Identifier.ValueText
                                        where methodName == StartName || methodName == StopName
                                        select type).FirstOrDefault();

                        if (typeDecl != null)
                        {
                            var typeSym = compiler.GetSemanticModel(tree, false).GetDeclaredSymbol(typeDecl);

                            typeNames.Add((name, $"{typeSym.ContainingNamespace}.{typeSym.Name}"));
                        }
                    }
                }
            }

            using var stream = new MemoryStream();

            var result = compiler.Emit(stream, options: new EmitOptions().WithDebugInformationFormat(
                DebugInformationFormat.Embedded));

            if (!result.Success)
            {
                _log.Error("Could not compile the final script assembly:");

                foreach (var diag in result.Diagnostics)
                    _log.Error("{0}", diag);

                return;
            }

            var asm = Assembly.Load(stream.ToArray());

            foreach (var (name, typeName) in typeNames)
                _scripts.Add((name, asm.GetType(typeName), new CSharpScriptContext(this, name,
                    Path.GetFullPath(Path.Combine(pkg, name)), new Log(typeof(CSharpPlugin), name),
                    _context.Data, _context.Proxies)));

            var count = 0;

            foreach (var (name, type, ctx) in _scripts)
            {
                try
                {
                    type.GetMethod(StartName, MethodFlags).Invoke(null, new object[] { ctx });
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    _log.Error("Failed to start package {0}:", name);
                    _log.Error("{0}", e);

                    continue;
                }

                _log.Info("Started package {0}", name);

                count++;
            }

            _log.Basic("Started {0} packages", count);

            foreach (var proc in _context.Proxies.Select(x => x.Processor))
            {
                proc.AddHandler<SLoadClientUserSettingPacket>(HandleLoadClientUserSetting);
                proc.AddHandler<CAdminPacket>(HandleAdmin);
                proc.AddHandler<COpCommandPacket>(HandleOpCommand);
            }
        }

        public void Stop()
        {
            foreach (var proc in _context.Proxies.Select(x => x.Processor))
            {
                proc.RemoveHandler<SLoadClientUserSettingPacket>(HandleLoadClientUserSetting);
                proc.RemoveHandler<CAdminPacket>(HandleAdmin);
                proc.RemoveHandler<COpCommandPacket>(HandleOpCommand);
            }

            var count = 0;

            foreach (var (name, type, ctx) in _scripts)
            {
                try
                {
                    type.GetMethod(StopName, MethodFlags).Invoke(null, new object[] { ctx });
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    _log.Error("Failed to stop package {0}:", name);
                    _log.Error("{0}", e);

                    continue;
                }

                _log.Info("Stopped package {0}", name);

                count++;
            }

            _log.Basic("Stopped {0} packages", count);
        }
    }
}
