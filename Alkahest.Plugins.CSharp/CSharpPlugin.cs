using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Plugins;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.JScript;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.ComponentModel.DataAnnotations;
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
using System.Transactions;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Xps.Packaging;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

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

        public string Name { get; } = "csharp";

        static readonly Log _log = new Log(typeof(CSharpPlugin));

        readonly List<(string, Type, Log)> _scripts = new List<(string, Type, Log)>();

        public void Start(GameProxy[] proxies)
        {
            var pkg = Configuration.PackageDirectory;

            Directory.CreateDirectory(pkg);

            var compiler = CSharpCompilation.Create(ScriptAssembly, options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary).WithAllowUnsafe(true).WithDeterministic(true));

            // Reference all the important .NET Framework assemblies.
            var types = new[]
            {
                typeof(Assert),                 // Alkahest.Core.dll
                typeof(RuntimeBinderException), // Microsoft.CSharp.dll
                typeof(JSObject),               // Microsoft.JScript.dll
                typeof(TriState),               // Microsoft.VisualBasic.dll
                typeof(object),                 // mscorlib.dll
                typeof(UIElement),              // PresentationCore.dll
                typeof(Window),                 // PresentationFramework.dll
                typeof(XpsDocument),            // ReachFramework.dll
                typeof(Uri),                    // System.dll
                typeof(Configuration),          // System.Configuration.dll
                typeof(Installer),              // System.Configuration.Install.dll
                typeof(ExportAttribute),        // System.ComponentModel.Composition.dll
                typeof(ExportBuilder),          // System.ComponentModel.Composition.Registration.dll
                typeof(Validator),              // System.ComponentModel.DataAnnotations.dll
                typeof(Enumerable),             // System.Core.dll
                typeof(DataSet),                // System.Data.dll
                typeof(DataTableExtensions),    // System.Data.DataSetExtensions.dll
                typeof(EntityConnection),       // System.Data.Entity.dll
                typeof(DataContext),            // System.Data.Linq.dll
                typeof(DataServiceHost),        // System.Data.Services.dll
                typeof(DataServiceContext),     // System.Data.Services.Client.dll
                typeof(Bitmap),                 // System.Drawing.dll
                typeof(ZipArchive),             // System.IO.Compression.dll
                typeof(ZipFile),                // System.IO.Compression.FileSystem.dll
                typeof(ManagementObject),       // System.Management.dll
                typeof(InstrumentationManager), // System.Management.Instrumentation.dll
                typeof(MessageQueue),           // System.Messaging.dll
                typeof(IPEndPointCollection),   // System.Net.dll
                typeof(HttpClient),             // System.Net.Http.dll
                typeof(WebRequestHandler),      // System.Net.Http.WebRequest.dll
                typeof(BigInteger),             // System.Numerics.dll
                typeof(PrintQueue),             // System.Printing.dll
                typeof(MemoryCache),            // System.Runtime.Caching.dll
                typeof(Unsafe),                 // System.Runtime.CompilerServices.Unsafe.dll
                typeof(InstanceHandle),         // System.Runtime.DurableInstancing.dll
                typeof(RemotingService),        // System.Runtime.Remoting.dll
                typeof(DataContractAttribute),  // System.Runtime.Serialization.dll
                typeof(SoapFormatter),          // System.Runtime.Serialization.Formatters.Soap.dll
                typeof(ProtectedData),          // System.Security.dll
                typeof(ChannelFactory),         // System.ServiceModel.dll
                typeof(ServiceHostFactory),     // System.ServiceModel.Activation.dll
                typeof(UdpBinding),             // System.ServiceModel.Channels.dll
                typeof(DiscoveryClient),        // System.ServiceModel.Discovery.dll
                typeof(RoutingService),         // System.ServiceModel.Routing.dll
                typeof(WebHttpBinding),         // System.ServiceModel.Web.dll
                typeof(ServiceBase),            // System.ServiceProcess.dll
                typeof(SpeechSynthesizer),      // System.Speech.dll
                typeof(Transaction),            // System.Transactions.dll
                typeof(RibbonControl),          // System.Windows.Controls.Ribbon
                typeof(Form),                   // System.Windows.Forms.dll
                typeof(DataPoint),              // System.Windows.Forms.DataVisualization.dll
                typeof(XamlType),               // System.Xaml.dll
                typeof(XmlNode),                // System.Xml.dll
                typeof(XNode),                  // System.Xml.Linq.dll
                typeof(Rect),                   // WindowsBase.dll
            };

            foreach (var type in types)
                compiler = compiler.AddReferences(MetadataReference.CreateFromFile(type.Assembly.Location));

            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);
            var typeNames = new List<(string, string)>();

            foreach (var dir in Directory.EnumerateDirectories(pkg))
            {
                var name = Path.GetFileName(dir);

                if (Configuration.DisablePackages.Contains(name))
                    continue;

                foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
                {
                    var tree = SyntaxFactory.ParseSyntaxTree(SourceText.From(
                        File.OpenRead(file), Encoding.UTF8), options, file);

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
                DebugInformationFormat.PortablePdb));

            if (!result.Success)
            {
                _log.Error("Could not compile the final script assembly:");

                foreach (var diag in result.Diagnostics)
                    _log.Error("{0}", diag);

                return;
            }

            var asm = Assembly.Load(stream.ToArray());

            foreach (var (name, typeName) in typeNames)
                _scripts.Add((name, asm.GetType(typeName), new Log(typeof(CSharpPlugin), name)));

            var count = 0;

            foreach (var (name, type, log) in _scripts)
            {
                try
                {
                    type.GetMethod(StartName, MethodFlags).Invoke(null,
                        new object[] { proxies.ToArray(), log });
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
        }

        public void Stop(GameProxy[] proxies)
        {
            var count = 0;

            foreach (var (name, type, log) in _scripts)
            {
                try
                {
                    type.GetMethod(StopName, MethodFlags).Invoke(null,
                        new object[] { proxies.ToArray(), log });
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
