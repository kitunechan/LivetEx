using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Threading;

using LivetEx.EventListeners;

namespace LivetEx {
	/// <summary>
	/// DispatcherCollectionの読み取り専用ラッパーです。<br/>
	/// ReadOnlyObservableCollectionなどと異なり、ソースコレクションの変更によってコレクションが変更された場合、<br/>
	/// 変更通知を行います。
	/// </summary>
	/// <typeparam name="T">コレクションアイテムの型</typeparam>
	public class ReadOnlyDispatcherCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable {
		private DispatcherCollection<T> _list;
		private DisposableCollection _listeners = new DisposableCollection();
		private bool _disposed;

		public ReadOnlyDispatcherCollection( DispatcherCollection<T> collection ) : base( collection ) {
			if( collection == null ) throw new ArgumentNullException( "collection" );

			_list = collection;

			_listeners.Add( new LivetPropertyChangedEventListener( _list, ( sender, e ) => OnPropertyChanged( e ) ) );
			_listeners.Add( new LivetCollectionChangedEventListener( _list, ( sender, e ) => OnCollectionChanged( e ) ) );
		}

		/// <summary>
		/// このコレクションが変更通知を行うDispatcherを取得します。
		/// </summary>
		public Dispatcher Dispatcher {
			get {
				ThrowExceptionIfDisposed();
				return _list.Dispatcher;
			}
		}

		/// <summary>
		/// この読み取り専用コレクションのソースDispatcherCollectionを取得します。
		/// </summary>
		public DispatcherCollection<T> SourceCollection {
			get {
				ThrowExceptionIfDisposed();
				return _list;
			}
		}

		/// <summary>
		/// この読み取り専用コレクションが保持するイベントリスナのコレクションを取得します。
		/// </summary>
		public DisposableCollection EventListeners {
			get {
				ThrowExceptionIfDisposed();
				return _listeners;
			}
		}

		/// <summary>
		/// コレクションが変更された時に発生します。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// プロパティが変更された時に発生します。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnCollectionChanged( NotifyCollectionChangedEventArgs args ) {
			ThrowExceptionIfDisposed();
			Interlocked.CompareExchange( ref CollectionChanged, null, null )?.Invoke( this, args );
		}

		protected void OnPropertyChanged( PropertyChangedEventArgs args ) {
			ThrowExceptionIfDisposed();
			Interlocked.CompareExchange( ref PropertyChanged, null, null )?.Invoke( this, args );
		}

		/// <summary>
		/// ソースコレクションとの連動を解除します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				_listeners.Dispose();

				if( typeof( IDisposable ).IsAssignableFrom( typeof( T ) ) ) {
					foreach( IDisposable i in _list ) {
						i.Dispose();
					}
				}
			}
			_disposed = true;
		}

		protected void ThrowExceptionIfDisposed() {
			if( _disposed ) {
				throw new ObjectDisposedException( "ReadOnlyDispatcherCollection" );
			}
		}
	}
}
