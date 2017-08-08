using System.Runtime.CompilerServices;
using System.ComponentModel;
using LivetEx.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace LivetEx {

	public delegate IDisposable ReAttachGenerator( Action ReAttach, Action Execute );
	public delegate IDisposable ChildGenerator( Action Execute );

	public class PropertyChangedTree : IDisposable, IEnumerable<IDisposable> {

		public PropertyChangedTree() {

		}

		public PropertyChangedTree( ReAttachGenerator currentFunc ) {
			this.Current = currentFunc( ReAttach, Execute );
		}


		public IDisposable Current { get; set; }

		public void ReAttach() {
			_disposableCollection.Dispose();
			_disposableCollection = new DisposableCollection();

			_disposableCollection.AddRange( _ChildGenerateList.Select( x => x( Execute ) ).Where( x => x != null ) );
		}

		public Action Execute { get; set; }

		private DisposableCollection _disposableCollection = new DisposableCollection();

		List<ChildGenerator> _ChildGenerateList = new List<ChildGenerator>();


		public void Add( params ChildGenerator[] funcs ) {

			foreach( var func in funcs ) {
				_ChildGenerateList.Add( func );

				var child = func( Execute );
				if( child != null ) {
					_disposableCollection.Add( child );
				}
			}

		}

		private bool _disposed;
		private object _lockObject = new object();

		/// <summary>
		/// このツリーに含まれるすべての要素をDisposeします。
		/// </summary>
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				lock( _lockObject ) {
					_disposableCollection.Dispose();
				}
			}
			_disposed = true;
		}


		public IEnumerator<IDisposable> GetEnumerator() {
			return ( (IEnumerable<IDisposable>)this._disposableCollection ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ( (IEnumerable<IDisposable>)this._disposableCollection ).GetEnumerator();
		}
	}

	public static class PropertyChangedTreeExtension {

		public static PropertyChangedTree AddExecute( this PropertyChangedTree tree, Action execute ) {
			tree.Execute = execute;
			return tree;
		}

		public static PropertyChangedTree SetReAttachAction( this PropertyChangedTree tree, ReAttachGenerator currentFunction ) {
			tree.Current = currentFunction( tree.ReAttach, tree.Execute );

			return tree;
		}

		public static PropertyChangedTree AddChildren( this PropertyChangedTree tree, params ChildGenerator[] funcs ) {
			tree.Add( funcs );
			return tree;
		}

	}
}
