using System;
using LivetEx.Messaging;
using System.Xml.Serialization;

namespace LivetEx {
	/// <summary>
	/// ViewModelの基底クラスです。
	/// </summary>
	[Serializable]
	public abstract class ViewModel : NotificationObject, IDisposable {
		[NonSerialized]
		private bool _disposed;
		[NonSerialized]
		private Messenger _messenger;
		[NonSerialized]
		private DisposableCollection _disposableCollection;

		/// <summary>
		/// このViewModelクラスの基本DisposableCollectionです。
		/// </summary>
		[XmlIgnore]
		public DisposableCollection DisposableCollection {
			get {
				if( _disposableCollection == null ) {
					_disposableCollection = new DisposableCollection();
				}
				return _disposableCollection;
			}
			internal set {
				_disposableCollection = value;
			}
		}

		/// <summary>
		/// このViewModelクラスの基本Messegerインスタンスです。
		/// </summary>
		[XmlIgnore]
		public Messenger Messenger {
			get {
				if( _messenger == null ) {
					_messenger = new Messenger();
				}
				return _messenger;
			}
			internal set {
				_messenger = value;
			}
		}

		/// <summary>
		/// このインスタンスによって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;
			if( disposing ) {
				_disposableCollection?.Dispose();
			}
			_disposed = true;
		}
	}
}