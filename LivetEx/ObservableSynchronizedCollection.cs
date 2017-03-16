using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Collections;

namespace LivetEx {
	/// <summary>
	/// スレッドセーフな変更通知コレクションです。
	/// </summary>
	/// <typeparam name="T">コレクションアイテムの型</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable" )]
	[Serializable]
	public class ObservableSynchronizedCollection<T>: IList<T>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T> {
		protected IList<T> Items;
		[NonSerialized]
		private object _syncRoot = new object();
		[NonSerialized]
		private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ObservableSynchronizedCollection() {
			Items = new List<T>();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">初期値となるソース</param>
		public ObservableSynchronizedCollection( IEnumerable<T> source ) {
			if( source == null ) throw new ArgumentNullException( "source" );
			Items = new List<T>( source );
		}

		/// <summary>
		/// 指定したオブジェクトを検索し、最初に見つかった位置の 0 から始まるインデックスを返します。
		/// </summary>
		/// <param name="item">検索するオブジェクト</param>
		/// <returns>最初に見つかった位置のインデックス</returns>
		public int IndexOf( T item ) {
			return ReadWithLockAction( () => Items.IndexOf( item ) );
		}

		/// <summary>
		/// 指定したインデックスの位置に要素を挿入します。
		/// </summary>
		/// <param name="index">指定するインデックス</param>
		/// <param name="item">挿入するオブジェクト</param>
		public void Insert( int index, T item ) {
			ReadAndWriteWithLockAction( () => Items.Insert( index, item ),
				() => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
				} );
		}

		/// <summary>
		/// 指定したインデックスにある要素を削除します。
		/// </summary>
		/// <param name="index">指定するインデックス</param>
		public void RemoveAt( int index ) {
			ReadAndWriteWithLockAction( () => Items[index],
				removeItem => Items.RemoveAt( index ),
				removeItem => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, removeItem, index ) );
				} );
		}

		public T this[int index] {
			get {
				return ReadWithLockAction( () => Items[index] );
			}
			set {
				ReadAndWriteWithLockAction( () => Items[index],
					oldItem => {
						Items[index] = value;
					},
					oldItem => {
						OnPropertyChanged( "Item[]" );
						OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Replace, Items[index], oldItem, index ) );
					} );
			}
		}

		/// <summary>
		/// 末尾にオブジェクトを追加します。
		/// </summary>
		/// <param name="item">追加するオブジェクト</param>
		public void Add( T item ) {
			ReadAndWriteWithLockAction( () => Items.Add( item ),
				() => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, Items.Count - 1 ) );
				} );
		}

		/// <summary>
		/// すべての要素を削除します。
		/// </summary>
		public void Clear() {
			ReadAndWriteWithLockAction( () => Items.Count,
			count => {
				if( count != 0 ) {
					Items.Clear();
				}
			},
			count => {
				if( count != 0 ) {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
				}
			} );
		}

		/// <summary>
		/// ある要素がこのコレクションに含まれているかどうかを判断します。
		/// </summary>
		/// <param name="item">コレクションに含まれているか判断したい要素</param>
		/// <returns>このコレクションに含まれているかどうか</returns>
		public bool Contains( T item ) {
			return ReadWithLockAction( () => Items.Contains( item ) );
		}

		/// <summary>
		/// 全体を互換性のある1次元の配列にコピーします。コピー操作は、コピー先の配列の指定したインデックスから始まります。
		/// </summary>
		/// <param name="array">コピー先の配列</param>
		/// <param name="arrayIndex">コピー先の配列のどこからコピー操作をするかのインデックス</param>
		public void CopyTo( T[] array, int arrayIndex ) {
			ReadWithLockAction( () => Items.CopyTo( array, arrayIndex ) );
		}

		/// <summary>
		/// 実際に格納されている要素の数を取得します。
		/// </summary>
		public int Count {
			get {
				return ReadWithLockAction( () => Items.Count );
			}
		}

		/// <summary>
		/// このコレクションが読み取り専用かどうかを取得します。
		/// </summary>
		public bool IsReadOnly {
			get { return Items.IsReadOnly; }
		}

		/// <summary>
		/// 最初に見つかった特定のオブジェクトを削除します。
		/// </summary>
		/// <param name="item">削除したいオブジェクト</param>
		/// <returns>削除できたかどうか</returns>
		public bool Remove( T item ) {
			bool result = false;

			ReadAndWriteWithLockAction( () => Items.IndexOf( item ),
				index => {
					result = Items.Remove( item );
				},
				index => {
					if( result ) {
						OnPropertyChanged( "Count" );
						OnPropertyChanged( "Item[]" );
						OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
					}
				} );

			return result;
		}

		/// <summary>
		/// 指定されたインデックスの要素を指定されたインデックスに移動します。
		/// </summary>
		/// <param name="oldIndex">移動したい要素のインデックス</param>
		/// <param name="newIndex">移動先のインデックス</param>
		public void Move( int oldIndex, int newIndex ) {
			ReadAndWriteWithLockAction( () => Items[oldIndex],
				item => {
					Items.RemoveAt( oldIndex );
					Items.Insert( newIndex, item );
				},
				item => {
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Move, item, newIndex, oldIndex ) );
				} );
		}

		/// <summary>
		/// 反復処理するためのスナップショットの列挙子を返します。
		/// </summary>
		/// <returns>列挙子</returns>
		public IEnumerator<T> GetEnumerator() {
			return ReadWithLockAction( () => ( (IEnumerable<T>)Items.ToArray() ).GetEnumerator() );
		}

		/// <summary>
		/// 反復処理するためのスナップショットの列挙子を返します。
		/// </summary>
		/// <returns>列挙子</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return ReadWithLockAction( () => ( (IEnumerable<T>)Items.ToArray() ).GetEnumerator() );
		}

		/// <summary>
		/// 全体を互換性のある1次元の配列にコピーします。コピー操作は、コピー先の配列の指定したインデックスから始まります。
		/// </summary>
		/// <param name="array">コピー先の配列</param>
		/// <param name="index">コピー先の配列のどこからコピー操作をするかのインデックス</param>
		public void CopyTo( Array array, int index ) {
			CopyTo( array.Cast<T>().ToArray(), index );
		}

		/// <summary>
		/// このコレクションがスレッドセーフであるかどうかを取得します。(常にtrueを返します)
		/// </summary>
		public bool IsSynchronized {
			get { return true; }
		}

		/// <summary>
		/// このコレクションへのスレッドセーフなアクセスに使用できる同期オブジェクトを返します。
		/// </summary>
		public object SyncRoot {
			get { return _syncRoot; }
		}

		/// <summary>
		/// CollectionChangedイベントを発生させます。
		/// </summary>
		/// <param name="args">NotifyCollectionChangedEventArgs</param>
		protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs args ) {
			var threadSafeHandler = Interlocked.CompareExchange( ref CollectionChanged, null, null );

			if( threadSafeHandler != null ) {
				threadSafeHandler( this, args );
			}
		}

		/// <summary>
		/// PropertyChangedイベントを発生させます。
		/// </summary>
		/// <param name="propertyName">変更されたプロパティの名前</param>
		protected virtual void OnPropertyChanged( string propertyName ) {
			var threadSafeHandler = Interlocked.CompareExchange( ref PropertyChanged, null, null );

			if( threadSafeHandler != null ) {
				threadSafeHandler( this, EventArgsFactory.GetPropertyChangedEventArgs( propertyName ) );
			}
		}


		protected void ReadWithLockAction( Action readAction ) {
			if( !_lock.IsReadLockHeld ) {
				_lock.EnterReadLock();
				try {
					readAction();
				} finally {
					_lock.ExitReadLock();
				}
			} else {
				readAction();
			}
		}

		protected TResult ReadWithLockAction<TResult>( Func<TResult> readAction ) {
			if( !_lock.IsReadLockHeld ) {
				_lock.EnterReadLock();
				try {
					return readAction();
				} finally {
					_lock.ExitReadLock();
				}
			}

			return readAction();
		}

		protected void ReadAndWriteWithLockAction( Action writeAction, Action readAfterWriteAction ) {
			_lock.EnterUpgradeableReadLock();
			try {
				_lock.EnterWriteLock();
				try {
					writeAction();
				} finally {
					_lock.ExitWriteLock();
				}

				_lock.EnterReadLock();

				try {
					readAfterWriteAction();
				} finally {
					_lock.ExitReadLock();
				}
			} finally {
				_lock.ExitUpgradeableReadLock();
			}
		}

		protected void ReadAndWriteWithLockAction<TResult>( Func<TResult> readBeforeWriteAction, Action<TResult> writeAction, Action<TResult> readAfterWriteAction ) {
			_lock.EnterUpgradeableReadLock();
			try {
				TResult readActionResult = readBeforeWriteAction();

				_lock.EnterWriteLock();

				try {
					writeAction( readActionResult );
				} finally {
					_lock.ExitWriteLock();
				}

				_lock.EnterReadLock();

				try {
					readAfterWriteAction( readActionResult );
				} finally {
					_lock.ExitReadLock();
				}
			} finally {
				_lock.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// コレクションが変更された際に発生するイベントです。
		/// </summary>
		[field: NonSerialized]
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// プロパティが変更された際に発生するイベントです。
		/// </summary>
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
