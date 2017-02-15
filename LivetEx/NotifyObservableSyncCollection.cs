using LivetEx.EventListeners;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivetEx {
	/// <summary>
	/// 保有しているプロパティの変更通知も行うコレクションです。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NotifyObservableSyncCollection<T>: ObservableSynchronizedCollection<T>, IList, IDisposable {
		public NotifyObservableSyncCollection() {
			AddCollectionChanged( this );
		}

		public NotifyObservableSyncCollection( IEnumerable<T> collection )
			: base( collection ) {

			foreach( INotifyPropertyChanged item in this.Items) {
				if( item != null ) {
					AddPropertyChanged( item );
				}
			}
			AddCollectionChanged( this );
		}

		/// <summary>
		/// コレクション内のプロパティの変更通知を伝えます。
		/// </summary>
		public event PropertyChangedEventHandler CollectionItemNotifyPropertyChanged;

		void AddPropertyChanged( INotifyPropertyChanged item ) {
			var eventListener = new LivetPropertyChangedEventListener( item, item_PropertyChanged );

			if( !removeList.ContainsKey( item ) ) {
				removeList[item] = new List<IDisposable>();
			}
			removeList[item].Add( eventListener );

			this.CompositeDisposable.Add( eventListener );
		}

		void RemovePropertyChanged( INotifyPropertyChanged item ) {
			if( removeList.ContainsKey( item ) ) {
				foreach( var _item in removeList[item] ) {
					_item.Dispose();
				}
			}
		}

		void item_PropertyChanged( object sender, PropertyChangedEventArgs e ) {
			if( this.CollectionItemNotifyPropertyChanged != null ) {
				this.CollectionItemNotifyPropertyChanged( sender, e );
			}
		}

		void AddCollectionChanged( INotifyCollectionChanged collection ) {
			this.CompositeDisposable.Add( new LivetCollectionChangedEventListener( collection, NotifyObservableCollection_CollectionChanged ) );
		}

		void NotifyObservableCollection_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e ) {
			switch( e.Action ) {
				case NotifyCollectionChangedAction.Remove:
					return;
				case NotifyCollectionChangedAction.Reset:
					foreach( INotifyPropertyChanged item in this.Items ) {
						if( item != null ) {
							RemovePropertyChanged( item );
						}
					}
					return;

				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Replace:
					if( e.OldItems != null ) {
						foreach( INotifyPropertyChanged item in e.OldItems ) {
							if( item != null ) {
								RemovePropertyChanged( item );
							}
						}
					}

					if( e.NewItems != null ) {
						foreach( INotifyPropertyChanged item in e.NewItems ) {
							if( item != null ) {
								AddPropertyChanged( item );
							}
						}
					}
					return;

				default:
					break;
			}
		}


		Dictionary<INotifyPropertyChanged, List<IDisposable>> removeList = new Dictionary<INotifyPropertyChanged, List<IDisposable>>();

		#region CompositeDisposable
		private LivetCompositeDisposable _compositeDisposable;
		public LivetCompositeDisposable CompositeDisposable {
			get {
				if( _compositeDisposable == null ) {
					_compositeDisposable = new LivetCompositeDisposable();
				}
				return _compositeDisposable;
			}
			set {
				_compositeDisposable = value;
			}
		}
		#endregion

		#region Dispose
		private bool _disposed;
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;
			if( disposing ) {
				if( _compositeDisposable != null ) {
					removeList = null;
					_compositeDisposable.Dispose();
				}
			}

			// 非管理（unmanaged）リソースの破棄処理をここに記述します。

			_disposed = true;
		}
		#endregion

		#region IList

		int IList.Add( object value ) {
			this.Add( (T)value );
			return this.Count;
		}

		void IList.Clear() {
			this.Clear();
		}

		bool IList.Contains( object value ) {
			return this.Contains( (T)value );
		}

		int IList.IndexOf( object value ) {
			return this.IndexOf( (T)value );
		}

		void IList.Insert( int index, object value ) {
			this.Insert( index, (T)value );
		}

		bool IList.IsFixedSize {
			get {
				IList list = Items as IList;
				if( list != null ) {
					return list.IsFixedSize;
				}
				return Items.IsReadOnly;
			}
		}

		bool IList.IsReadOnly {
			get { return this.IsReadOnly; }
		}

		void IList.Remove( object value ) {
			this.Remove( (T)value );
		}

		void IList.RemoveAt( int index ) {
			this.RemoveAt(index);
		}

		object IList.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = (T)value;
			}
		}

		void ICollection.CopyTo( Array array, int index ) {
			this.CopyTo( array, index );
		}

		int ICollection.Count {
			get { return this.Count; }
		}

		bool ICollection.IsSynchronized {
			get { return this.IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return this.SyncRoot; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
