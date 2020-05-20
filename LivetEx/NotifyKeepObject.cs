using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LivetEx {
	/// <summary>
	/// 変更有無を保存、検知する変更通知オブジェクトの基底クラスです。
	/// </summary>
	[Serializable]
	public class NotifyKeepObject : NotificationObject, IIsChanged {

		[field: NonSerialized]
		public event EventHandler<IsChangedChangingEventArgs> IsChangedChanging;

		/// <summary>
		/// 変更があったのかを取得または設定します。
		/// </summary>
		public bool IsChanged {
			get {
				return _IsChanged || ChangedChildren.Where( x => x != null ).Any( x => x.IsChanged );
			}
			set {
				IsChangedChanging?.Invoke( this, new IsChangedChangingEventArgs( _IsChanged, value ) );

				if( _IsChanged != value ) {
					_IsChanged = value;
					base.RaisePropertyChanged();
				}
			}
		}

		[field: NonSerialized]
		protected bool _IsChanged;

		/// <summary>
		/// 保有しているIIsChangedのコレクションを取得します。
		/// </summary>
		public virtual IEnumerable<IIsChanged> ChangedChildren { get { return Enumerable.Empty<IIsChanged>(); } }

		/// <summary>
		/// 変更フラグをリセットします。
		/// </summary>
		public void IsChangedReset() {
			foreach( var child in ChangedChildren ) {
				if( child != null ) {
					child.IsChangedReset();
				}
			}

			IsChanged = false;
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <exception cref="NotSupportedException">() => プロパティ 以外の形式のラムダ式が指定されました。</exception>
		protected virtual void RaisePropertyChanged<T>( Expression<Func<T>> propertyExpression, bool isChanged = true ) {
			base.RaisePropertyChanged( propertyExpression );

			IsChanged |= isChanged;
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = "", bool isChanged = true ) {
			base.RaisePropertyChanged( propertyName );

			IsChanged |= isChanged;
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		protected virtual void RaisePropertyChanged( bool isChanged, [CallerMemberName] string propertyName = "" ) {
			this.RaisePropertyChanged( propertyName, isChanged );
		}


		protected override void RaisePropertyChanged<T>( Expression<Func<T>> propertyExpression ) {
			base.RaisePropertyChanged( propertyExpression );

			IsChanged |= true;
		}

		protected override void RaisePropertyChanged( [CallerMemberName] string propertyName = "" ) {
			base.RaisePropertyChanged( propertyName );

			IsChanged |= true;
		}
	}
}
