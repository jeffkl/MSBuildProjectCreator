using Microsoft.Build;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class ShadeAssemblies : Microsoft.Build.Utilities.Task
{
    private static readonly string[] AssemblyExtensions = new[] { ".dll", ".exe" };

    private static byte[] _keyPairCache;

    private string[] _assemblySearchPaths;

    private Dictionary<string, ITaskItem> _assembliesToRename;

    private Dictionary<string, string> _friendAssembliesToRename;

    private DefaultAssemblyResolver _assemblyResolver;

    private StrongNameKeyPair _strongNameKeyPair;

    private HashSet<string> _shadedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    [Required]
    public string[] AssemblySearchPaths
    {
        get
        {
            return _assemblySearchPaths;
        }
        set
        {
            _assemblySearchPaths = value;

            System.AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                AssemblyName requestedAssemblyName = new AssemblyName(args.Name);

                foreach (FileInfo candidateAssemblyFile in _assemblySearchPaths.SelectMany(searchPath => AssemblyExtensions.Select(extension => new FileInfo(Path.Combine(searchPath, $"{requestedAssemblyName.Name}{extension}")))))
                {
                    if (!candidateAssemblyFile.Exists)
                    {
                        continue;
                    }

                    AssemblyName candidateAssemblyName = AssemblyName.GetAssemblyName(candidateAssemblyFile.FullName);

                    if (requestedAssemblyName.ProcessorArchitecture != System.Reflection.ProcessorArchitecture.None && requestedAssemblyName.ProcessorArchitecture != candidateAssemblyName.ProcessorArchitecture)
                    {
                        continue;
                    }

                    if (requestedAssemblyName.Flags.HasFlag(AssemblyNameFlags.PublicKey) && !requestedAssemblyName.GetPublicKeyToken().SequenceEqual(candidateAssemblyName.GetPublicKeyToken()))
                    {
                        continue;
                    }

                    return Assembly.LoadFrom(candidateAssemblyFile.FullName);
                }

                return null;
            };
        }
    }

    [Required]
    public ITaskItem[] AssembliesToShade { get; set; }

    public override bool Execute()
    {
        _assembliesToRename = AssembliesToShade.ToDictionary(i => i.GetMetadata("AssemblyFullName"), i => i);

        _friendAssembliesToRename = AssembliesToShade.ToDictionary(i => i.GetMetadata("OldAssemblyName"), i => i.GetMetadata("NewAssemblyName"));

        _strongNameKeyPair = GetStrongNameKeyPair();

        _assemblyResolver = new DefaultAssemblyResolver();

        try
        {
            foreach (var item in _assemblySearchPaths)
            {
                _assemblyResolver.AddSearchDirectory(item);
            }

            foreach (var item in _assembliesToRename.Values)
            {
                _assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(item.ItemSpec));
                _assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(item.GetMetadata("OutputPath")));
            }

            foreach (KeyValuePair<string, ITaskItem> assemblyToRename in _assembliesToRename)
            {
                ShadeAssembly(assemblyToRename.Key, assemblyToRename.Value);
            }
        }
        finally
        {
            _assemblyResolver.Dispose();
        }

        return !Log.HasLoggedErrors;
    }

    private FileInfo ShadeAssembly(string assemblyName, ITaskItem item)
    {
        AssemblyName oldAssemblyName = new AssemblyName(assemblyName);

        FileInfo shadedPath = new FileInfo(item.GetMetadata("OutputPath"));

        if (!_shadedAssemblies.Add(shadedPath.FullName))
        {
            return shadedPath;
        }
        
        shadedPath.Directory.Create();

        Log.LogMessageFromText($"Shading assembly {item.ItemSpec} -> {shadedPath.FullName}", MessageImportance.High);

        string newAssemblyFileName = Path.GetFileNameWithoutExtension(shadedPath.Name);
        
        using (AssemblyDefinition assembly = ReadAssembly(item.ItemSpec))
        {
            assembly.Name.Name = newAssemblyFileName;
            assembly.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;
            assembly.Name.HasPublicKey = false;
            assembly.Name.PublicKey = new byte[0];

            assembly.Write(
                shadedPath.FullName,
                new WriterParameters
                {
                    StrongNameKeyPair = _strongNameKeyPair,
                });
            
            bool dirty = false;

            string fullPublicKey = BitConverter.ToString(assembly.Name.PublicKey).Replace("-", "").ToLowerInvariant();

            foreach (AssemblyNameReference assemblyReference in assembly.MainModule.AssemblyReferences)
            {
                if (_assembliesToRename.TryGetValue(assemblyReference.FullName, out ITaskItem renamedAssembly))
                {
                    AssemblyName renamedAssemblyName = new AssemblyName(assemblyReference.FullName);
                    
                    ShadeAssembly(assemblyReference.FullName, renamedAssembly);

                    assemblyReference.Name = $"{renamedAssemblyName.Name}.{renamedAssemblyName.Version}";
                    assemblyReference.PublicKey = assembly.Name.PublicKey;
                    assemblyReference.PublicKeyToken = assembly.Name.PublicKeyToken;
                    assemblyReference.HasPublicKey = true;

                    dirty = true;
                }
            }

            foreach (CustomAttribute customAttribute in assembly.CustomAttributes.Where(i => i.AttributeType.Name == "InternalsVisibleToAttribute"))
            {
                string value = customAttribute.ConstructorArguments[0].Value.ToString();

                string oldFriendAssemblyName = value.Substring(0, value.IndexOf(','));

                if (_friendAssembliesToRename.TryGetValue(oldFriendAssemblyName, out string newFriendAssemblyName))
                {
                    customAttribute.ConstructorArguments[0] = new CustomAttributeArgument(customAttribute.ConstructorArguments[0].Type, $"{newFriendAssemblyName}, PublicKey={fullPublicKey}");

                    dirty = true;
                }
            }

            if (dirty)
            {
                assembly.Write(
                    shadedPath.FullName,
                    new WriterParameters
                    {
                        StrongNameKeyPair = _strongNameKeyPair,
                    });
            }
            
            AssemblyName _ = AssemblyName.GetAssemblyName(shadedPath.FullName);
        }

        return shadedPath;
    }

    private StrongNameKeyPair GetStrongNameKeyPair(string keyPath = null, string keyFilePassword = null)
    {
        if (!string.IsNullOrEmpty(keyPath))
        {
            if (!string.IsNullOrEmpty(keyFilePassword))
            {
                X509Certificate2 certificate = new X509Certificate2(keyPath, keyFilePassword, X509KeyStorageFlags.Exportable);

                if (certificate.PrivateKey is not RSACryptoServiceProvider provider)
                {
                    throw new InvalidOperationException("The key file is not password protected or the incorrect password was provided.");
                }

                return new StrongNameKeyPair(provider.ExportCspBlob(true));
            }

            return new StrongNameKeyPair(File.ReadAllBytes(keyPath));
        }

        if (_keyPairCache != null)
        {
            return new StrongNameKeyPair(_keyPairCache);
        }

        using (var provider = new RSACryptoServiceProvider(1024, new CspParameters() { KeyNumber = 2 }))
        {
            _keyPairCache = provider.ExportCspBlob(!provider.PublicOnly);
        }

        return new StrongNameKeyPair(_keyPairCache);
    }

    private AssemblyDefinition ReadAssembly(string fullPath)
    {
        return AssemblyDefinition.ReadAssembly(fullPath, new ReaderParameters
        {
            AssemblyResolver = _assemblyResolver,
            ReadSymbols = File.Exists(Path.ChangeExtension(fullPath, ".pdb"))
        });
    }
}