using System.Windows.Input;
using System;
using System.ComponentModel;
using System.Threading;

namespace LivetEx.Commands {
	/// <summary>
	/// 汎用的コマンドを表します。
	/// </summary>
	public sealed class DelegateCommand : Command, ICommand, INotifyPropertyChanged {
		Action _execute;
		Func<bool> _canExecute;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		public DelegateCommand( Action execute ) : this( execute, null ) { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		/// <param name="canExecute">コマンドが実行可能かどうかをあらわすFunc&lt;bool&gt;</param>
		public DelegateCommand( Action execute, Func<bool> canExecute ) {
			_execute = execute ?? throw new ArgumentNullException( "execute" );
			_canExecute = canExecute;
		}

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CanExecute {
			get { return _canExecute?.Invoke() ?? true; }
		}

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		public void Execute() {
			_execute();
		}

		/// <summary>
		/// コマンドを試行します。
		/// </summary>
		public void TryExecute() {
			if( CanExecute ) {
				_execute();
			}
		}

		void ICommand.Execute( object parameter ) {
			Execute();
		}

		bool ICommand.CanExecute( object parameter ) {
			return CanExecute;
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged() {
			PropertyChanged?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( nameof( CanExecute ) ) );
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化したことを通知します。
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate" )]
		public void RaiseCanExecuteChanged() {
			OnPropertyChanged();
			OnCanExecuteChanged();
		}
	}
}
