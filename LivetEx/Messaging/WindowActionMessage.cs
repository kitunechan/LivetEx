using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// Windowを最大化・最小化・閉じる・通常化・ダイアログの結果の相互作用メッセージです。
	/// </summary>
	public class WindowActionMessage : Message {

		#region StaticMessage

		/// <summary>
		/// メッセージキーの無い WindowAction.Close のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage Close => new WindowActionMessage( WindowAction.Close );

		/// <summary>
		/// メッセージキーの無い WindowAction.Minimize のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage Minimize => new WindowActionMessage( WindowAction.Minimize );

		/// <summary>
		/// メッセージキーの無い WindowAction.Maximize のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage Maximize => new WindowActionMessage( WindowAction.Maximize );

		/// <summary>
		/// メッセージキーの無い WindowAction.Normal のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage Normal => new WindowActionMessage( WindowAction.Normal );

		/// <summary>
		/// メッセージキーの無い WindowAction.Active のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage Active => new WindowActionMessage( WindowAction.Active );

		/// <summary>
		/// メッセージキーの無い WindowAction.ResultOK のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage ResultOK => new WindowActionMessage( WindowAction.ResultOK );

		/// <summary>
		/// メッセージキーの無い WindowAction.ResultCancel のメッセージを取得します。
		/// </summary>
		public static WindowActionMessage ResultCancel => new WindowActionMessage( WindowAction.ResultCancel );

		#endregion

		public WindowActionMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// メッセージキーとWindowが遷移すべき状態を定義して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="action">Windowが遷移すべき状態を表すWindowAction列挙体</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowActionMessage( string messageKey, WindowAction action ) : this( messageKey ) {
			Action = action;
		}

		/// <summary>
		/// Windowが遷移すべき状態を定義して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="action">Windowが遷移すべき状態を表すWindowAction列挙体</param>
		public WindowActionMessage( WindowAction action ) : this( null, action ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowActionMessage();
		}

		/// <summary>
		/// Windowが遷移すべき状態を表すWindowAction列挙体を指定、または取得します。
		/// </summary>
		public WindowAction Action {
			get { return (WindowAction)GetValue( ActionProperty ); }
			set { SetValue( ActionProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Action.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ActionProperty =
			DependencyProperty.Register( "Action", typeof( WindowAction ), typeof( WindowActionMessage ), new PropertyMetadata( WindowAction.None ) );


	}
}
