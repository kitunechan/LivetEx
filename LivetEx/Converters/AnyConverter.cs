using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace LivetEx.Converters {
	public class AnyConverter : MarkupExtension, IValueConverter {
		public static AnyConverter Instance => _Instance ?? ( _Instance = new AnyConverter() );
		static AnyConverter _Instance;

		public object Convert( object target, Type targetType, object parameter, CultureInfo culture ) {
			if( target is IEnumerable items ) {
				return items.GetEnumerator().MoveNext();
			}
			return false;
		}

		public object ConvertBack( object target, Type targetType, object parameter, CultureInfo culture ) {
			throw new NotImplementedException();
		}


		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return this;
		}
	}
}
