using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LivetEx.EventListeners
{
	/// <summary>
	/// ICollectionItemNotifyPropertyChanged.CollectionItemNotifyPropertyChangedを受信するためのイベントリスナです。
	/// </summary>
	public sealed class LivetCollectionItemNotifyPropertyChangedEventListener : LivetEventListener<PropertyChangedEventHandler>, IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>> {
		private AnonymousCollectionItemNotifyPropertyChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ICollectionItemNotifyPropertyChangedオブジェクト</param>
		public LivetCollectionItemNotifyPropertyChangedEventListener( ICollectionItemNotifyPropertyChanged source ) {
			_bag = new AnonymousCollectionItemNotifyPropertyChangedEventHandlerBag( source );
			Initialize( h => source.CollectionItemNotifyPropertyChanged += h, h => source.CollectionItemNotifyPropertyChanged -= h, ( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// コンストラクタ。リスナのインスタンスの作成と同時にハンドラを一つ登録します。
		/// </summary>
		/// <param name="source">ICollectionItemNotifyPropertyChangedオブジェクト</param>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public LivetCollectionItemNotifyPropertyChangedEventListener( ICollectionItemNotifyPropertyChanged source, PropertyChangedEventHandler handler ) {
			_bag = new AnonymousCollectionItemNotifyPropertyChangedEventHandlerBag( source, handler );
			Initialize( h => source.CollectionItemNotifyPropertyChanged += h, h => source.CollectionItemNotifyPropertyChanged -= h, ( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// このリスナインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public void RegisterHandler( PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( handler );
		}

		/// <summary>
		/// このリスナインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyName">ハンドラを登録したいPropertyChagedEventArgs.PropertyNameの名前</param>
		/// <param name="handler">propertyNameで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		public void RegisterHandler( string propertyName, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( propertyName, handler );
		}

		/// <summary>
		/// このリスナインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <param name="handler">propertyExpressionで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		public void RegisterHandler<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( propertyExpression, handler );
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return
				( (IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>)_bag )
					.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return ( (IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>)_bag ).GetEnumerator();
		}

		public void Add( PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( handler );
		}

		public void Add( string propertyName, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyName, handler );
		}

		[Obsolete( "記述方法が間違っています。", true )]
		public void Add( string propertyName ) {
			throw new NotSupportedException( "記述方法が間違っています。" );
		}

		public void Add( string propertyName, params PropertyChangedEventHandler[] handlers ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyName, handlers );
		}

		public void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyExpression, handler );
		}

		[Obsolete( "記述方法が間違っています。", true )]
		public void Add<T>( Expression<Func<T>> propertyExpression ) {
			throw new NotSupportedException( "記述方法が間違っています。" );
		}

		public void Add<T>( Expression<Func<T>> propertyExpression, params PropertyChangedEventHandler[] handlers ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyExpression, handlers );
		}

		protected override void Dispose( bool disposing ) {
			base.Dispose( disposing );
		}
	}
}
