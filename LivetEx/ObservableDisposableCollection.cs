using System.Runtime.CompilerServices;
using System.ComponentModel;
using LivetEx.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using LivetEx;

namespace LivetEx {

	/// <summary>
	/// 内部で公開可能なメソッドを定義
	/// </summary>
	public interface IObservableDisposableCollection {
		/// <summary>
		/// 再構築します。
		/// </summary>
		void Refresh();

		/// <summary>
		/// 基本処理を実行します。
		/// </summary>
		void ActionRun();
	}

	/// <summary>
	/// イベントリスナーの親子関係を紐付けるためのクラスです。
	/// </summary>
	public class ObservableDisposableCollection : IObservableDisposableCollection, IDisposable, IEnumerable<IDisposable> {

		/// <summary>
		/// 
		/// </summary>
		public ObservableDisposableCollection() {

		}

		/// <summary>
		/// 元となるIDisposableを取得する関数を設定して、インスタンスを作成します。
		/// </summary>
		public ObservableDisposableCollection( Func<IObservableDisposableCollection, IDisposable> currentGenerator ) {
			this.Current = currentGenerator( this );
		}

		/// <summary>
		/// 元となるIDisposableインスタンス
		/// </summary>
		#region Current 変更通知プロパティ
		public IDisposable Current {
			get { return _Current; }
			set {
				if( _Current != null ) {
					throw new ArgumentException( "既に登録済みです。" );
				}
				_Current = value;
			}
		}
		private IDisposable _Current;
		#endregion



		/// <summary>
		/// 再構築します。
		/// </summary>
		public void Refresh() {
			_disposableCollection.Dispose();
			_disposableCollection = new DisposableCollection( _ChildGenerateList.Select( x => x( ActionRun ) ).Where( x => x != null ) );
		}

		/// <summary>
		/// 基本処理を設定または取得します。
		/// </summary>
		public Action DefaultAction { get; set; }

		/// <summary>
		/// 基本処理を実行します。
		/// </summary>
		public void ActionRun() {
			DefaultAction?.Invoke();
		}

		private DisposableCollection _disposableCollection = new DisposableCollection();

		List<Func<Action, IDisposable>> _ChildGenerateList = new List<Func<Action, IDisposable>>();

		/// <summary>
		/// 子のIDisposableを取得する関数を追加します。
		/// </summary>
		public void Add( params Func<Action, IDisposable>[] childGenerators ) {
			foreach( var func in childGenerators ) {
				_ChildGenerateList.Add( func );

				var child = func( ActionRun );
				if( child != null ) {
					_disposableCollection.Add( child );
				}
			}
		}

		#region dispose
		private bool _disposed;
		private object _lockObject = new object();

		/// <summary>
		/// このツリーに含まれるすべての要素をDisposeします。
		/// </summary>
		public void Dispose() {
			Dispose( true );

		}

		protected virtual void Dispose( bool disposing ) {
			lock( _lockObject ) {
				if( _disposed ) return;
				_disposed = true;

				if( disposing ) {
					_disposableCollection.Dispose();
					_disposableCollection = null;
					Current?.Dispose();

					GC.SuppressFinalize( this );
				}
			}
		}

		#endregion

		public IEnumerator<IDisposable> GetEnumerator() {
			return ( (IEnumerable<IDisposable>)this._disposableCollection ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ( (IEnumerable<IDisposable>)this._disposableCollection ).GetEnumerator();
		}
	}

	public static class PropertyChangedTreeExtension {

		/// <summary>
		/// 基本処理を設定します。
		/// </summary>
		public static ObservableDisposableCollection SetDefaultAction( this ObservableDisposableCollection tree, Action defaultAction ) {
			tree.DefaultAction = defaultAction;
			return tree;
		}

		/// <summary>
		/// 関数の戻り値により、Currentプロパティを設定します。
		/// </summary>
		public static ObservableDisposableCollection SetCurrentFunc( this ObservableDisposableCollection tree, Func<IObservableDisposableCollection, IDisposable> currentFunc ) {
			tree.Current = currentFunc( tree );

			return tree;
		}


		/// <summary>
		/// 子のIDisposableを取得する関数を追加します。
		/// </summary>
		public static ObservableDisposableCollection AddChildren( this ObservableDisposableCollection tree, params Func<Action, IDisposable>[] funcs ) {
			tree.Add( funcs );
			return tree;
		}

	}
}

