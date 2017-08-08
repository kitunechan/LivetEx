namespace LivetEx.Messaging {
	/// <summary>
	/// WindowActionMessageで使用する、Windowが遷移すべき状態を表します。
	/// </summary>
	public enum WindowAction {
		/// <summary>
		/// 何もしません
		/// </summary>
		None,

		/// <summary>
		/// Windowを閉じます。
		/// </summary>
		Close,
		/// <summary>
		/// Windowを最大化します。
		/// </summary>
		Maximize,
		/// <summary>
		/// Windowを最小化します。
		/// </summary>
		Minimize,
		/// <summary>
		/// Windowを通常状態にします。
		/// </summary>
		Normal,
		/// <summary>
		/// Windowをアクティブにします。
		/// </summary>
		Active,

		/// <summary>
		/// WindowのDialogResultをTrueにします。
		/// </summary>
		ResultOK,

		/// <summary>
		/// WindowのDialogResultをFalseにします。
		/// </summary>
		ResultCancel,
	}
}
