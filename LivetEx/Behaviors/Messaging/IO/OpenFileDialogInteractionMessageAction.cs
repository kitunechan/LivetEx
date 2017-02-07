using LivetEx.Messaging.IO;
using Microsoft.Win32;
using System.Windows;

using LivetEx.Messaging;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace LivetEx.Behaviors.Messaging.IO
{
   /// <summary>
	/// 「ファイルを開く」ダイアログを表示するアクションです。<see cref="OpeningFileSelectionMessage"/>に対応します。
	/// </summary>
	public class OpenFileDialogInteractionMessageAction: InteractionMessageAction<DependencyObject> {

		static Settings setting;
		static Dictionary<string, string> InitialDirectoryGroupList;

		public string InitialDirectoryGroup { get; set; }

		protected override void InvokeAction( InteractionMessage message ) {

			if( InitialDirectoryGroupList == null ) {
				var window = Window.GetWindow( this.AssociatedObject );

				setting = new Settings( window );
				if( setting.IsUpgrade != true ) {
					setting.Upgrade();
				}

				setting.IsUpgrade = true;

				if( setting.Group == null ) {
					setting.Group = new Dictionary<string, string>();
				}
				InitialDirectoryGroupList = setting.Group;
			}

			var openFileMessage = message as OpeningFileSelectionMessage;
			var group = this.InitialDirectoryGroup;

			var InitialDirectory = openFileMessage.InitialDirectory;

			if( string.IsNullOrWhiteSpace( InitialDirectory ) ) {
				if( InitialDirectoryGroupList.ContainsKey( group ) ) {
					InitialDirectory = InitialDirectoryGroupList[group];
				}
			}

			if( openFileMessage != null ) {
				var dialog = new OpenFileDialog {
					FileName = openFileMessage.FileName,
					InitialDirectory = InitialDirectory,
					AddExtension = openFileMessage.AddExtension,
					Filter = openFileMessage.Filter,
					Title = openFileMessage.Title,
					Multiselect = openFileMessage.MultiSelect,
				};

				if( dialog.ShowDialog() == true ) {
					openFileMessage.Response = dialog.FileNames;

					if( !string.IsNullOrWhiteSpace( group ) ) {

						InitialDirectoryGroupList[group] = Path.GetDirectoryName( dialog.FileName );

						setting.Save();
					}
				} else {
					openFileMessage.Response = null;
				}


			}
		}


		public class Settings: ApplicationSettingsBase {
			public Settings( Window window ) : base( window.GetType().FullName ) { }

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
