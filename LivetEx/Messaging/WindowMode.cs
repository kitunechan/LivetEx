namespace LivetEx.Messaging {
	public enum WindowMode {
		/// <summary>
		/// 指定されていません。Action, MessageともにUnKnownの場合はModalが設定されます。
		/// </summary>
		UnKnown,
		/// <summary>
		/// 新しいWindowをモーダルウインドウとして開きます。
		/// </summary>
		Modal,
		/// <summary>
		/// 新しいWindowをモーダレスウインドウとして開きます。
		/// </summary>
		Modeless,
		/// <summary>
		/// すでに同じ型のWindowが開かれている場合はそのWindowをアクティブにします。<br/>
		/// 同じ型のWindowが開かれていなかった場合、新しくWindowを開きます。
		/// </summary>
		NewOrActive,
	}
}
