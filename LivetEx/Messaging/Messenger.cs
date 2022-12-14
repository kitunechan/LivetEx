using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Animation;

namespace LivetEx.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class Messenger {
		/// <summary>
		/// 指定された相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public void Raise( Message message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			if( this.Raised != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
			}

			if( this.RaisedLater != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
			}
		}


		/// <summary>
		/// 戻り値のあるメッセージを送信します。
		/// </summary>
		public object GetResponse( IResponsiveMessage responsiveMessage ) {
			if( responsiveMessage == null ) {
				throw new ArgumentException( $"{nameof( responsiveMessage )}はnullにできません", nameof( responsiveMessage ) );
			}
			if( responsiveMessage is Message message ) {
				var hasEvent = false;
				if( this.Raised != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				if( this.RaisedLater != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				return hasEvent ? responsiveMessage.Response : default( object );

			} else {
				throw new ArgumentException( $"{nameof( responsiveMessage )}は{ typeof( Message ) }を継承している必要があります。", nameof( responsiveMessage ) );
			}
		}

		/// <summary>
		/// 戻り値のあるメッセージを送信します。
		/// </summary>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="message">メッセージ</param>
		/// <returns>呼び出したメッセージの戻り値</returns>
		public TResult GetResponse<TResult>( ResponsiveMessage<TResult> message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			var hasEvent = false;
			if( this.Raised != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
				hasEvent = true;
			}

			if( this.RaisedLater != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
				hasEvent = true;
			}

			return hasEvent ? message.Response : default( TResult );
		}


		/// <summary>
		/// １つの引数を持った、戻り値のあるメソッドを呼び出すメッセージを送信します。
		/// </summary>
		/// <typeparam name="TArgument">引数の型</typeparam>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="callResultMethodMessage">メッセージ</param>
		/// <returns>呼び出したメソッドの戻り値</returns>
		public object GetResponse( ICallFuncMessage callResultMethodMessage ) {
			if( callResultMethodMessage == null ) {
				throw new ArgumentException( $"{nameof( callResultMethodMessage )}はnullにできません", nameof( callResultMethodMessage ) );
			}

			if( callResultMethodMessage is Message message ) {
				var hasEvent = false;
				if( this.Raised != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				if( this.RaisedLater != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				return hasEvent ? callResultMethodMessage.Result : default( object );

			} else {
				throw new ArgumentException( $"{nameof( callResultMethodMessage )}は{ typeof( Message ) }を継承している必要があります。", nameof( callResultMethodMessage ) );

			}
		}

		/// <summary>
		/// 戻り値のあるメソッドを呼び出すメッセージを送信します。
		/// </summary>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="callResultMethodMessage">メッセージ</param>
		/// <returns>呼び出したメソッドの戻り値</returns>
		public TResult GetResponse<TResult>( CallFuncMessage<TResult> callResultMethodMessage ) {
			if( callResultMethodMessage == null ) {
				throw new ArgumentException( $"{nameof( callResultMethodMessage )}はnullにできません", nameof( callResultMethodMessage ) );
			}

			if( callResultMethodMessage is Message message ) {
				var hasEvent = false;
				if( this.Raised != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				if( this.RaisedLater != null ) {
					if( !message.IsFrozen ) {
						message.Freeze();
					}
					this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
					hasEvent = true;
				}

				return hasEvent ? callResultMethodMessage.Result : default( TResult );


			} else {
				throw new ArgumentException( $"{nameof( callResultMethodMessage )}は{ typeof( Message ) }を継承している必要があります。", nameof( callResultMethodMessage ) );
			}
		}




		/// <summary>
		/// メッセージを送信し、そのメッセージを返します。
		/// </summary>
		/// <typeparam name="T">メッセージの型</typeparam>
		/// <param name="message">メッセージ</param>
		/// <returns>送信したメッセージ</returns>
		public T GetResponseMessage<T>( T message ) where T : Message, IResponsiveMessage {
			if( message == null ) {
				throw new ArgumentException( $"{nameof( message )}はnullにできません", nameof( message ) );
			}

			var hasEvent = false;
			if( this.Raised != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.Raised?.Invoke( this, new MessageRaisedEventArgs( message ) );
				hasEvent = true;
			}

			if( this.RaisedLater != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}
				this.RaisedLater?.Invoke( this, new MessageRaisedEventArgs( message ) );
				hasEvent = true;
			}

			return hasEvent ? message : default( T );
		}

		/// <summary>
		/// 相互作用メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<MessageRaisedEventArgs> Raised;


		public event EventHandler<MessageRaisedEventArgs> RaisedLater;


		/// <summary>
		/// 指定された相互作用メッセージを非同期で送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public async Task RaiseAsync( Message message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			if( !message.IsFrozen ) {
				message.Freeze();
			}

			await Task.Run( () => Raise( message ) );
		}

		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを非同期で送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		public async Task<T> GetResponseAsync<T>( ResponsiveMessage<T> message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			if( !message.IsFrozen ) {
				message.Freeze();
			}

			return await Task.Run( () => GetResponse( message ) );
		}

	}


	/// <summary>
	/// 相互作用メッセージ送信時イベント用のイベント引数です。
	/// </summary>
	public class MessageRaisedEventArgs : EventArgs {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">Message</param>
		public MessageRaisedEventArgs( Message message ) {
			Message = message;
		}

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public Message Message { get; set; }
	}
}