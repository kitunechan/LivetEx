using System;
using System.Collections.Generic;

namespace LivetEx {

	/// <summary>
	/// 変更有無の保存、検知を行うインターフェース
	/// </summary>
	public interface IIsChanged {
		/// <summary>
		/// 変更があったのかどうかを取得または設定します。
		/// </summary>
		bool IsChanged { get; }

		/// <summary>
		/// IsChangedが変更される前に発生するイベントです。
		/// </summary>
		event EventHandler<IsChangedChangingEventArgs> IsChangedChanging;

		/// <summary>
		/// 変更フラグを持っている子クラスを取得します。
		/// </summary>
		IEnumerable<IIsChanged> ChangedChildren { get; }

		/// <summary>
		/// 変更フラグをリセットします。
		/// </summary>
		void IsChangedReset();
	}
}
