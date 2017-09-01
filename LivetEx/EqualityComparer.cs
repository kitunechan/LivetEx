using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivetEx {


	public class EqualityComparer<T, TKey> : EqualityComparer<T> {

		Func<T, TKey> selector;

		public EqualityComparer( Func<T, TKey> selector ) {
			this.selector = selector;
		}

		public override bool Equals( T x, T y ) {
			return selector( x ).Equals( selector( y ) );
		}

		public override int GetHashCode( T obj ) {
			return selector( obj ).GetHashCode();
		}
	}
}
