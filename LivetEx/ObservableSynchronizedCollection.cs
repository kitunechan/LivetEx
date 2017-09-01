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
	public class ObservableSynchronizedCollection<T> : IList<T>, ICollection, IList, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>, IDisposable {
		protected List<T> Items;

		[NonSerialized]
		private object _syncRoot = new object();

		[NonSerialized]
		private ReaderWriterLockSlimEx _lock = new ReaderWriterLockSlimEx();

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

		public T this[int index] {
			get {
				return _lock.ReadWithLockAction( () => Items[index] );
			}
			set {
				_lock.WriteReadWithLockAction( () => Items[index],
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
			_lock.ReadAndWriteWithLockAction( () => Items.Add( item ),
				() => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, Items.Count - 1 ) );
				} );
		}
		/// <summary>
		/// 指定したインデックスの位置に要素を挿入します。
		/// </summary>
		/// <param name="index">指定するインデックス</param>
		/// <param name="item">挿入するオブジェクト</param>
		public void Insert( int index, T item ) {
			_lock.ReadAndWriteWithLockAction( () => Items.Insert( index, item ),
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
			_lock.WriteReadWithLockAction( () => Items[index],
				removeItem => Items.RemoveAt( index ),
				removeItem => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, removeItem, index ) );
				} );
		}
		/// <summary>
		/// 最初に見つかった特定のオブジェクトを削除します。
		/// </summary>
		/// <param name="item">削除したいオブジェクト</param>
		/// <returns>削除できたかどうか</returns>
		public bool Remove( T item ) {
			bool result = false;

			_lock.WriteReadWithLockAction( () => Items.IndexOf( item ),
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
		/// すべての要素を削除します。
		/// </summary>
		public void Clear() {
			_lock.WriteReadWithLockAction( () => Items.Count,
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
		/// 指定したオブジェクトを検索し、最初に見つかった位置の 0 から始まるインデックスを返します。
		/// </summary>
		/// <param name="item">検索するオブジェクト</param>
		/// <returns>最初に見つかった位置のインデックス</returns>
		public int IndexOf( T item ) {
			return _lock.ReadWithLockAction( () => Items.IndexOf( item ) );
		}

		/// <summary>
		/// ある要素がこのコレクションに含まれているかどうかを判断します。
		/// </summary>
		/// <param name="item">コレクションに含まれているか判断したい要素</param>
		/// <returns>このコレクションに含まれているかどうか</returns>
		public bool Contains( T item ) {
			return _lock.ReadWithLockAction( () => Items.Contains( item ) );
		}
		/// <summary>
		/// 指定されたインデックスの要素を指定されたインデックスに移動します。
		/// </summary>
		/// <param name="oldIndex">移動したい要素のインデックス</param>
		/// <param name="newIndex">移動先のインデックス</param>
		public void Move( int oldIndex, int newIndex ) {
			_lock.WriteReadWithLockAction( () => Items[oldIndex],
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
		/// 全体を互換性のある1次元の配列にコピーします。コピー操作は、コピー先の配列の指定したインデックスから始まります。
		/// </summary>
		/// <param name="array">コピー先の配列</param>
		/// <param name="arrayIndex">コピー先の配列のどこからコピー操作をするかのインデックス</param>
		public void CopyTo( T[] array, int arrayIndex ) {
			_lock.ReadWithLockAction( () => Items.CopyTo( array, arrayIndex ) );
		}

		/// <summary>
		/// 実際に格納されている要素の数を取得します。
		/// </summary>
		public int Count {
			get {
				return _lock.ReadWithLockAction( () => Items.Count );
			}
		}

		/// <summary>
		/// このコレクションが読み取り専用かどうかを取得します。
		/// </summary>
		public bool IsReadOnly {
			get { return ( (ICollection<T>)Items ).IsReadOnly; }
		}
		/// <summary>
		/// 反復処理するためのスナップショットの列挙子を返します。
		/// </summary>
		/// <returns>列挙子</returns>
		public IEnumerator<T> GetEnumerator() {
			return _lock.ReadWithLockAction( () => ( (IEnumerable<T>)Items.ToArray() ).GetEnumerator() );
		}

		/// <summary>
		/// 反復処理するためのスナップショットの列挙子を返します。
		/// </summary>
		/// <returns>列挙子</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return _lock.ReadWithLockAction( () => ( (IEnumerable<T>)Items.ToArray() ).GetEnumerator() );
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

		public bool IsFixedSize => ( (IList)Items ).IsFixedSize;

		object IList.this[int index] {
			get => this[index];
			set => this[index] = (T)value;
		}

		int IList.Add( object item ) {
			return _lock.ReadAndWriteWithLockAction( () => ( (IList)this.Items ).Add( item ),
				x => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, Items.Count - 1 ) );


				} );
		}

		//-----------

		bool IList.Contains( object item ) {
			return _lock.ReadWithLockAction( () => ( (IList)this.Items ).Contains( item ) );
		}

		int IList.IndexOf( object item ) {
			return _lock.ReadWithLockAction( () => ( (IList)this.Items ).IndexOf( item ) );
		}

		void IList.Insert( int index, object item ) {
			_lock.ReadAndWriteWithLockAction( () => ( (IList)this.Items ).Insert( index, item ),
				() => {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item, index ) );
				} );
		}

		void IList.Remove( object item ) {
			_lock.WriteReadWithLockAction( () => ( (IList)this.Items ).IndexOf(item),
				index => {
					if( index != -1 ) {
						( (IList)this.Items ).Remove( item );
					}
				},
				index => {
					if( index != -1 ) {
						OnPropertyChanged( "Count" );
						OnPropertyChanged( "Item[]" );
						OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item, index ) );
					}
				} );
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


		/// <summary>
		/// CollectionChangedイベントを発生させます。
		/// </summary>
		/// <param name="args">NotifyCollectionChangedEventArgs</param>
		protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs args ) {
			Interlocked.CompareExchange( ref CollectionChanged, null, null )?.Invoke( this, args );
		}

		/// <summary>
		/// PropertyChangedイベントを発生させます。
		/// </summary>
		/// <param name="propertyName">変更されたプロパティの名前</param>
		protected virtual void OnPropertyChanged( string propertyName ) {
			Interlocked.CompareExchange( ref PropertyChanged, null, null )?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( propertyName ) );
		}


		#region Dispose
		[NonSerialized]
		private bool _disposed;
		public void Dispose() {
			Dispose( true );
			//GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;
			if( disposing ) {
				_lock.Dispose();


			}

			// 非管理（unmanaged）リソースの破棄処理をここに記述します。

			_disposed = true;
		}

		#endregion


	}
}
