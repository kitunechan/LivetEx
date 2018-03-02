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
	[Serializable]
	public class NotifyObservableSyncCollection<T> : ObservableSynchronizedCollection<T>, ICollectionItemNotifyPropertyChanged {
		public NotifyObservableSyncCollection() {

		}

		public NotifyObservableSyncCollection( IEnumerable<T> collection )
			: base( collection ) {

			foreach( INotifyPropertyChanged item in this.Items ) {
				if( item != null ) {
					AddPropertyChanged( item );
				}
			}
		}


		/// <summary>
		/// コレクション内のプロパティの変更通知を伝えます。
		/// </summary>
		[field: NonSerialized]
		public event PropertyChangedEventHandler CollectionItemNotifyPropertyChanged;

		void AddPropertyChanged( INotifyPropertyChanged item ) {
			var eventListener = new LivetPropertyChangedEventListener( item, OnCollectionItemNotifyPropertyChanged );

			if( !CompositeDisposableTable.ContainsKey( item ) ) {
				CompositeDisposableTable[item] = new List<IDisposable>();
			}
			CompositeDisposableTable[item].Add( eventListener );
		}

		void RemovePropertyChanged( INotifyPropertyChanged item ) {
			if( CompositeDisposableTable.ContainsKey( item ) ) {
				foreach( var _item in CompositeDisposableTable[item] ) {
					_item.Dispose();
				}

				CompositeDisposableTable.Remove( item );
			}
		}

		void ClearPropertyChanged() {
			foreach( var item in CompositeDisposableTable.SelectMany(x=>x.Value) ) {
				item.Dispose();
			}
			CompositeDisposableTable.Clear();
		}

		/// <summary>
		/// Collection内のPropertyChangedイベントを発生させます。
		/// </summary>
		protected virtual void OnCollectionItemNotifyPropertyChanged( object sender, PropertyChangedEventArgs e ) {
			if( !IsSuspend ) {
				this.CollectionItemNotifyPropertyChanged?.Invoke( sender, e );
			}
		}

		/// <summary>
		/// 変更通知イベントの発生を抑制します。
		/// </summary>
		public void SuspendEvent() {
			IsSuspend = true;
		}

		/// <summary>
		/// 変更通知イベントの発生を再開します。
		/// </summary>
		public void ResumeEvent() {
			IsSuspend = false;
		}

		public bool IsSuspend { get; protected set; }

		public void Insert( int index, IEnumerable<T> items ) {
			SuspendEvent();

			foreach( var item in items ) {
				this.Insert( ++index, item );
			}

			ResumeEvent();

			Lock.ReadWithLockAction( () => {
				OnPropertyChanged( "Count" );
				OnPropertyChanged( "Item[]" );
				OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, items ) );
			} );
		}

		public void AddRange( IEnumerable<T> items ) {
			SuspendEvent();

			foreach( var item in items ) {
				this.Add( item );
			}

			ResumeEvent();

			Lock.ReadWithLockAction( () => {
				OnPropertyChanged( "Count" );
				OnPropertyChanged( "Item[]" );
				OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, items ) );
			} );
		}

		/// <summary>
		/// CollectionChangedイベントを発生させます。
		/// </summary>
		/// <param name="args">NotifyCollectionChangedEventArgs</param>
		protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs args ) {
			if( !IsSuspend ) {
				base.OnCollectionChanged( args );
			}

			switch( args.Action ) {
				case NotifyCollectionChangedAction.Remove:
				if( args.OldItems != null ) {
					foreach( var item in args.OldItems.OfType<INotifyPropertyChanged>() ) {
						RemovePropertyChanged( item );
					}
				}
				return;

				case NotifyCollectionChangedAction.Reset:
				ClearPropertyChanged();
				
				return;

				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Replace:
				if( args.OldItems != args.NewItems ) {
					if( args.OldItems != null ) {
						foreach( var item in args.OldItems.OfType<INotifyPropertyChanged>() ) {
							RemovePropertyChanged( item );
						}
					}

					if( args.NewItems != null ) {
						foreach( var item in args.NewItems.OfType<INotifyPropertyChanged>() ) {
							AddPropertyChanged( item );
						}

						//foreach( var item in args.NewItems ) {
						//	if( item is INotifyPropertyChanged notify ) {
						//		AddPropertyChanged( notify );

						//	} else if( item is IEnumerable items ) {
						//		foreach( var item2 in items.OfType<INotifyPropertyChanged>() ) {
						//			AddPropertyChanged( item2 );
						//		}
						//	}
						//}
					}
				}
				return;

				case NotifyCollectionChangedAction.Move:
				default:
				break;
			}
		}

		#region CompositeDisposableTable
		[NonSerialized]
		private Dictionary<INotifyPropertyChanged, List<IDisposable>> _compositeDisposableTable;
		public Dictionary<INotifyPropertyChanged, List<IDisposable>> CompositeDisposableTable {
			get {
				if( _compositeDisposableTable == null ) {
					_compositeDisposableTable = new Dictionary<INotifyPropertyChanged, List<IDisposable>>();
				}
				return _compositeDisposableTable;
			}
			set {
				_compositeDisposableTable = value;
			}
		}
		#endregion

		#region Dispose
		[NonSerialized]
		private bool _disposed;


		protected override void Dispose( bool disposing ) {

			if( _disposed ) return;
			if( disposing ) {
				ClearPropertyChanged();
			}

			// 非管理（unmanaged）リソースの破棄処理をここに記述します。

			_disposed = true;
		}

		#endregion
	}
}
