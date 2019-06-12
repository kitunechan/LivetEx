using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LivetEx.Commands {
	/// <summary>
	/// 対象のElementにコマンドを適用させるクラス
	/// コマンド追加後にApplyCommandBindingsで適用してください。
	/// </summary>
	public class CommandBindingService : IEnumerable<BindingUnit> {

		public CommandBindingService( FrameworkElement taregetElement ) {
			this.TaregetElement = taregetElement;
		}

		public FrameworkElement TaregetElement { get; private set; }
		readonly Dictionary<BindingUnit, CommandBinding> Items = new Dictionary<BindingUnit, CommandBinding>();

		/// <summary>
		/// コマンドの適用
		/// </summary>
		public void ApplyCommandBindings() {
			if( this.TaregetElement == null ) {
				throw new ArgumentException( "Elementを指定して下さい。" );
			}

			foreach( var item in Items ) {
				if( !this.TaregetElement.CommandBindings.Contains( item.Value ) ) {
					this.TaregetElement.CommandBindings.Add( item.Value );
				}
			}
		}

		/// <summary>
		/// コマンドの削除
		/// </summary>
		public void ClearCommandBindings() {
			if( this.TaregetElement == null ) {
				throw new ArgumentException( "Elementを指定して下さい。" );
			}

			foreach( var item in Items ) {
				this.TaregetElement.CommandBindings.Remove( item.Value );
			}
		}

		public void Add( BindingUnit item ) {
			this.Items[item] = new CommandBinding( item.Target,
				( s, ex ) => item.Command.Execute( item.CommandParameter ?? ex ),
				( s, ex ) => {
					ex.CanExecute = item.Command.CanExecute( item.CommandParameter ?? ex );
					ex.Handled = true;
				} );
		}

		public void AddRange( IEnumerable<BindingUnit> collection ) {
			foreach( var item in collection ) {
				Add( item );
			}
		}

		public void Clear() {
			this.Items.Clear();
			ClearCommandBindings();
		}

		public bool Remove( BindingUnit item ) {
			if( Items.ContainsKey( item ) ) {
				this.TaregetElement.CommandBindings.Remove( Items[item] );
				return this.Items.Remove( item );
			}

			return false;
		}

		public IEnumerator<BindingUnit> GetEnumerator() {
			return Items.Keys.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public int Count {
			get { return Items.Count; }
		}
		
	}

	public class BindingUnit {
		public BindingUnit( RoutedUICommand target ) {
			this.Target = target;
		}

		public BindingUnit( RoutedUICommand target, Action action ) {
			this.Target = target;

			this.Command = new DelegateCommand( action );
		}

		public BindingUnit( RoutedUICommand target, Action action, Func<bool> canExecute ) {
			this.Target = target;

			this.Command = new DelegateCommand( action, canExecute );
		}

		public BindingUnit( RoutedUICommand target, ICommand command ) {
			this.Target = target;
			this.Command = command;
		}

		public BindingUnit( RoutedUICommand target, ICommand command, object commandParameter ) {
			this.Target = target;
			this.Command = command;
			this.CommandParameter = commandParameter;
		}

		public RoutedUICommand Target { get; private set; }
		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }

	}


}
