using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LivetEx.Commands {
	/// <summary>
	/// 対象のFrameworkElementにCommandBindingを適用させるクラス
	/// </summary>
	public class CommandBindingService : IEnumerable<BindingUnit> {

		public CommandBindingService( FrameworkElement taregetElement ) {
			this.TaregetElement = taregetElement;
		}

		public FrameworkElement TaregetElement { get; }
		readonly Dictionary<BindingUnit, CommandBinding> Items = new Dictionary<BindingUnit, CommandBinding>();

		/// <summary>
		/// コマンドを有効にします。
		/// </summary>
		public void EnableCommandBindings() {
			foreach( var item in this.Items ) {
				if( !this.TaregetElement.CommandBindings.Contains( item.Value ) ) {
					this.TaregetElement.CommandBindings.Add( item.Value );
				}
			}
		}

		/// <summary>
		/// コマンドを無効にします。
		/// </summary>
		public void DisableCommandBindings() {
			foreach( var item in this.Items ) {
				this.TaregetElement.CommandBindings.Remove( item.Value );
			}
		}

		/// <summary>
		/// コマンドを登録して有効にします。
		/// </summary>
		/// <param name="item"></param>
		public void Add( BindingUnit item ) {
			var commandBinding = new CommandBinding( item.Target,
				( s, ex ) => item.Command.Execute( item.CommandParameter ?? ex ),
				( s, ex ) => {
					ex.CanExecute = item.Command.CanExecute( item.CommandParameter ?? ex );
					ex.Handled = true;
				} );

			this.Items[item] = commandBinding;
			this.TaregetElement.CommandBindings.Add( commandBinding );
		}

		/// <summary>
		/// 複数のコマンドを登録して有効にします。
		/// </summary>
		/// <param name="collection"></param>
		public void AddRange( IEnumerable<BindingUnit> collection ) {
			foreach( var item in collection ) {
				Add( item );
			}
		}

		/// <summary>
		/// 登録したすべてのコマンドを無効にして削除します。
		/// </summary>
		public void Clear() {
			DisableCommandBindings();
			this.Items.Clear();
		}

		/// <summary>
		/// 指定したコマンドを無効にして削除します。
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove( BindingUnit item ) {
			if( this.Items.ContainsKey( item ) ) {
				this.TaregetElement.CommandBindings.Remove( Items[item] );
				return this.Items.Remove( item );
			}

			return false;
		}

		public IEnumerator<BindingUnit> GetEnumerator() {
			return this.Items.Keys.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public int Count => Items.Count;

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
