using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LivetEx.Commands {
	public class CommandBindingService : IEnumerable<BindingUnit> {
		public CommandBindingService() {

		}

		public CommandBindingService( FrameworkElement element ) {
			this.Element = element;
		}

		FrameworkElement Element;
		Dictionary<BindingUnit, CommandBinding> Items = new Dictionary<BindingUnit, CommandBinding>();

		/// <summary>
		/// コマンドの適用
		/// </summary>
		public void ApplyCommandBindings() {
			if( this.Element == null ) {
				throw new ArgumentException( "Elementを指定して下さい。" );
			}

			foreach( var item in Items ) {
				if( !this.Element.CommandBindings.Contains( item.Value ) ) {
					this.Element.CommandBindings.Add( item.Value );
				}
			}
		}

		/// <summary>
		/// コマンドの適用
		/// </summary>
		public void ApplyCommandBindings( FrameworkElement element ) {
			foreach( var item in Items ) {
				if( !element.CommandBindings.Contains( item.Value ) ) {
					element.CommandBindings.Add( item.Value );
				}
			}
		}

		/// <summary>
		/// コマンドの削除
		/// </summary>
		public void ClearCommandBindings() {
			if( this.Element == null ) {
				throw new ArgumentException( "Elementを指定して下さい。" );
			}

			foreach( var item in Items ) {
				this.Element.CommandBindings.Remove( item.Value );
			}
		}

		/// <summary>
		/// コマンドの削除
		/// </summary>
		public void ClearCommandBindings( FrameworkElement element ) {
			foreach( var item in Items ) {
				element.CommandBindings.Remove( item.Value );
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
				this.Element.CommandBindings.Remove( Items[item] );
				return this.Items.Remove( item );
			}

			return false;
		}

		public IEnumerator<BindingUnit> GetEnumerator() {
			return Items.Select( x => x.Key ).GetEnumerator();
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

		public RoutedUICommand Target { get; set; }
		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }

	}


}
