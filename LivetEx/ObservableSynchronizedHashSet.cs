using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivetEx {
	[Serializable]
	public class ObservableSynchronizedHashSet<T> : ISet<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable {
		protected readonly HashSet<T> Items;

		[NonSerialized]
		private ReaderWriterLockSlimEx _lock = new ReaderWriterLockSlimEx();

		[field: NonSerialized]
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;


		public ObservableSynchronizedHashSet() {
			this.Items = new HashSet<T>();
		}

		public ObservableSynchronizedHashSet( IEqualityComparer<T> comparer ) {
			this.Items = new HashSet<T>( comparer );
		}

		public ObservableSynchronizedHashSet( IEnumerable<T> items ) {
			this.Items = new HashSet<T>( items );
		}

		public ObservableSynchronizedHashSet( IEnumerable<T> items, IEqualityComparer<T> comparer ) {
			this.Items = new HashSet<T>( items, comparer );
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return _lock.ReadWithLockAction( () => Items.GetEnumerator() );
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _lock.ReadWithLockAction( () => Items.GetEnumerator() );
		}

		public void UnionWith( IEnumerable<T> other ) {
			_lock.ReadAndWriteWithLockAction( () => {
				var addedItems = other.Where( x => !Items.Contains( x ) ).ToArray();
				Items.UnionWith( addedItems );
				return addedItems;
			}, x => {
				if( 0 < x.Length ) {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, x ) );
				}
			} );
		}

		public void IntersectWith( IEnumerable<T> other ) {
			_lock.ReadAndWriteWithLockAction( () => {
				var removedItems = Items.Where( x => !other.Contains( x ) ).ToArray();
				Items.ExceptWith( removedItems );

				return removedItems;
			}, x => {
				if( 0 < x.Length ) {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, x ) );
				}
			} );
		}

		public virtual void ExceptWith( IEnumerable<T> other ) {
			_lock.ReadAndWriteWithLockAction( () => {
				var removedItems = other.Where( x => Items.Contains( x ) ).ToArray();
				Items.ExceptWith( removedItems );

				return removedItems;
			}, x => {
				if( 0 < x.Length ) {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, x ) );
				}
			} );
		}

		public virtual void SymmetricExceptWith( IEnumerable<T> other ) {
			var addedItems = new List<T>();
			var removedItems = new List<T>();

			_lock.ReadAndWriteWithLockAction( () => {
				foreach( T item in other.Distinct( Items.Comparer ) ) {
					if( Items.Contains( item ) ) {
						removedItems.Add( item );
					} else {
						addedItems.Add( item );
					}
				}

				Items.UnionWith( addedItems );
				Items.ExceptWith( removedItems );
			}, () => {
				if( addedItems.Count > 0 || removedItems.Count > 0 ) {
					OnPropertyChanged( "Count" );
					OnPropertyChanged( "Item[]" );
				}
				if( addedItems.Count > 0 ) {
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, addedItems ) );
				}
				if( removedItems.Count > 0 ) {
					OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, removedItems ) );
				}
			} );
		}

		public bool IsSubsetOf( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.IsSubsetOf( other ) );
		}

		public bool IsSupersetOf( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.IsSupersetOf( other ) );
		}

		public bool IsProperSupersetOf( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.IsProperSupersetOf( other ) );
		}

		public bool IsProperSubsetOf( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.IsProperSubsetOf( other ) );
		}

		public bool Overlaps( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.Overlaps( other ) );
		}

		public bool SetEquals( IEnumerable<T> other ) {
			return _lock.ReadWithLockAction( () => Items.SetEquals( other ) );
		}

		public bool Add( T item ) {
			return _lock.ReadAndWriteWithLockAction( () => Items.Add( item ),
				x => {
					if( x ) {
						OnPropertyChanged( "Count" );
						OnPropertyChanged( "Item[]" );
						OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, item ) );
					}
				} );
		}

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

		public bool Contains( T item ) {
			return _lock.ReadWithLockAction( () => Items.Contains( item ) );
		}

		public void CopyTo( T[] array, int arrayIndex ) {
			_lock.ReadWithLockAction( () => Items.CopyTo( array, arrayIndex ) );
		}

		public bool Remove( T item ) {
			return _lock.ReadAndWriteWithLockAction( () => Items.Remove( item ),
				x => {
					if( x ) {
						OnPropertyChanged( "Count" );
						OnPropertyChanged( "Item[]" );
						OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, item ) );
					}
				} );
		}

		public int Count => _lock.ReadWithLockAction( () => Items.Count );
		bool ICollection<T>.IsReadOnly => ( (ICollection<T>)this.Items ).IsReadOnly;

		void ICollection<T>.Add( T item ) {
			this.Add( item );
		}

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
		private bool disposed = false;
		protected virtual void Dispose( bool disposing ) {
			//lock(_lockObject){
			if( !disposed ) {
				disposed = true;

				// アンマネージドリソースの解放


				if( disposing ) {
					// マネージドリソースの解放（通常はここ）
					_lock.Dispose();
				}
			}
			//}
		}

		//private object _lockObject = new object();

		//アンマネージドリソースがある場合コメントアウトする
		// ファイナライザ
		//~() {
		//	Dispose( false );
		//}

		public void Dispose() {
			Dispose( true );

			//ファイナライザがある場合コメントアウトする
			//GC.SuppressFinalize( this );
		}

		#endregion
	}
}
