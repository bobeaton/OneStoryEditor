﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AiChorus.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastProjectFolder {
            get {
                return ((string)(this["LastProjectFolder"]));
            }
            set {
                this["LastProjectFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UpgradeSettings {
            get {
                return ((bool)(this["UpgradeSettings"]));
            }
            set {
                this["UpgradeSettings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("k6/KWKfduEWPXvdayves4DBdLgdR9w09GICKWVMO4PeOzf6YGbGAGCPwgwIgoP4tuzE4LOvL4jLSUuQ5v" +
            "W5nyEJsj/0DjMV3e5lMq+pEm0U=")]
        public string GoogleSheetsCredentialsClientId {
            get {
                return ((string)(this["GoogleSheetsCredentialsClientId"]));
            }
            set {
                this["GoogleSheetsCredentialsClientId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("bQyMqDyAzkDFPoAGRydbKCDx3QfoS2X9UNB9IqeqMss=")]
        public string GoogleSheetsCredentialsClientSecret {
            get {
                return ((string)(this["GoogleSheetsCredentialsClientSecret"]));
            }
            set {
                this["GoogleSheetsCredentialsClientSecret"] = value;
            }
        }
    }
}
