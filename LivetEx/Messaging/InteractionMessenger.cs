﻿using System;
using System.Threading.Tasks;
using System.Threading;

namespace LivetEx.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class InteractionMessenger {
		/// <summary>
		/// 指定された相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public void Raise( InteractionMessage message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}

				threadSafeHandler.Invoke( this, new InteractionMessageRaisedEventArgs( message ) );
			}
		}

		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		/// <returns>アクション実行後に、戻り情報を含んだ相互作用メッセージ</returns>
		public T GetResponse<T>( ResponsiveInteractionMessage<T> message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}

				threadSafeHandler( this, new InteractionMessageRaisedEventArgs( message ) );
				return message.Response;
			}

			return default( T );
		}


		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="V">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		/// <returns>アクション実行後に、戻り情報を含んだ相互作用メッセージ</returns>
		public V GetResponse<T, V>( ResponsiveInteractionMessage<T, V> message ) {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}

				threadSafeHandler( this, new InteractionMessageRaisedEventArgs( message ) );
				return message.Response;
			}

			return default( V );
		}

		/// <summary>
		/// 指定された、戻り値情報のある相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <typeparam name="T">戻り値情報のある相互作用メッセージの型</typeparam>
		/// <param name="message">戻り値情報のある相互作用メッセージ</param>
		/// <returns>アクション実行後に、戻り情報を含んだ相互作用メッセージ</returns>
		public T GetResponseVM<T>( T message ) where T : InteractionMessage, IResponsiveInteractionMessage {
			if( message == null ) {
				throw new ArgumentException( "messageはnullにできません" );
			}

			var threadSafeHandler = Interlocked.CompareExchange( ref Raised, null, null );
			if( threadSafeHandler != null ) {
				if( !message.IsFrozen ) {
					message.Freeze();
				}

				threadSafeHandler( this, new InteractionMessageRaisedEventArgs( message ) );
				return message;
			}

			return default( T );
		}

		/// <summary>
		/// 相互作用メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<InteractionMessageRaisedEventArgs> Raised;

		/// <summary>
		/// 指定された相互作用メッセージを非同期で送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public async Task RaiseAsync( InteractionMessage message ) {
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
		public async Task<T> GetResponseAsync<T>( ResponsiveInteractionMessage<T> message ) {
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
	public class InteractionMessageRaisedEventArgs : EventArgs {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">InteractionMessage</param>
		public InteractionMessageRaisedEventArgs( InteractionMessage message ) {
			Message = message;
		}

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public InteractionMessage Message { get; set; }
	}
}