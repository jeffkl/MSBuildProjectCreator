﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Build.Utilities.ProjectCreation.NuGet {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Build.Utilities.ProjectCreation.NuGet.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &quot;{0}&quot; has already been added to the package..
        /// </summary>
        internal static string ErrorFileAlreadyCreated {
            get {
                return ResourceManager.GetString("ErrorFileAlreadyCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified file path &quot;{0}&quot; must have a parent directory..
        /// </summary>
        internal static string ErrorFilePathMustBeInADirectory {
            get {
                return ResourceManager.GetString("ErrorFilePathMustBeInADirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified file path &quot;{0}&quot; must be a relative path..
        /// </summary>
        internal static string ErrorFilePathMustBeRelative {
            get {
                return ResourceManager.GetString("ErrorFilePathMustBeRelative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &quot;{0}&quot; version &quot;{1}&quot; has already been created..
        /// </summary>
        internal static string ErrorPackageAlreadyCreated {
            get {
                return ResourceManager.GetString("ErrorPackageAlreadyCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must add a package before adding build logic..
        /// </summary>
        internal static string ErrorWhenAddingBuildLogicRequiresPackage {
            get {
                return ResourceManager.GetString("ErrorWhenAddingBuildLogicRequiresPackage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must add a package before adding a library..
        /// </summary>
        internal static string ErrorWhenAddingLibraryRequiresPackage {
            get {
                return ResourceManager.GetString("ErrorWhenAddingLibraryRequiresPackage", resourceCulture);
            }
        }
    }
}