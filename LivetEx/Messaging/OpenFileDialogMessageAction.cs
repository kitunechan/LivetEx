using Microsoft.Win32;
using System.Windows;

using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace LivetEx.Messaging {
	/// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。<see cref="OpenFileDialogMessage"/>に対応します。
	/// </summary>
	public class OpenFileDialogMessageAction : MessageAction<DependencyObject> {

		static Settings setting;
		static Dictionary<string, string> InitialDirectoryGroupList;

		/// <summary>
		/// ファイル ダイアログに表示される初期ディレクトリのグループを取得または設定します。
		/// </summary>
		public string InitialDirectoryGroup { get; set; }

		protected override void InvokeAction( Message message ) {
			if( message is OpenFileDialogMessage windowMessage ) {
				var clone = (OpenFileDialogMessage)windowMessage.Clone();
				if( string.IsNullOrEmpty( clone.InitialDirectoryGroup ) ){
					clone.InitialDirectoryGroup = InitialDirectoryGroup;
				}

				Action( this.AssociatedObject, clone );
				windowMessage.Response = clone.Response;
			}
		}

		public static void Action( DependencyObject element, OpenFileDialogMessage message ) {
			var window = Window.GetWindow( element );

			if( InitialDirectoryGroupList == null ) {
				setting = new Settings( "OpenFileDialogSettings" );
				if( setting.IsUpgrade != true ) {
					setting.Upgrade();
				}

				setting.IsUpgrade = true;

				if( setting.Group == null ) {
					setting.Group = new Dictionary<string, string>();
				}
				InitialDirectoryGroupList = setting.Group;
			}

			var initialDirectory = message.InitialDirectory;
			var group = message.InitialDirectoryGroup;

			if( group != null && InitialDirectoryGroupList.ContainsKey( group ) ) {
				initialDirectory = InitialDirectoryGroupList[group];
			}

			var dialog = new OpenFileDialog() {
				FileName = message.FileName,
				InitialDirectory = !string.IsNullOrEmpty( initialDirectory ) ? Path.GetFullPath( initialDirectory ) : initialDirectory,
				AddExtension = message.AddExtension,
				Filter = message.Filter,
				Title = message.Title,
				Multiselect = message.MultiSelect,
				FilterIndex = message.FilterIndex,
				DefaultExt = message.DefaultExt,
				CheckPathExists = message.CheckPathExists,
				CheckFileExists = message.CheckFileExists,
			};

			if( dialog.ShowDialog(window) == true ) {
				message.Response = dialog.FileNames;

				if( !string.IsNullOrEmpty( group ) ) {
					InitialDirectoryGroupList[group] = Path.GetDirectoryName( dialog.FileName );
					setting.Save();
				}
			} else {
				message.Response = null;
			}
		}

		public class Settings : ApplicationSettingsBase {
			public Settings( string settingsKey ) : base( settingsKey ) { }

			[UserScopedSetting]
			public Dictionary<string, string> Group {
				get { return this["Group"] != null ? (Dictionary<string, string>)this["Group"] : null; }
				set { this["Group"] = value; }
			}

			[UserScopedSetting]
			public bool? IsUpgrade {
				get { return this["IsUpgrade"] != null ? (bool?)this["IsUpgrade"] : null; }
				set { this["IsUpgrade"] = value; }
			}
		}
	}
}
