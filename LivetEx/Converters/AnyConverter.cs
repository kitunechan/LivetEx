using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace LivetEx.Converters {
	public class AnyConverter : MarkupExtension, IValueConverter {
		public static AnyConverter Instance {
			get {
				if( _Instance == null ) {
					_Instance = new AnyConverter();
				}
				return _Instance;
			}
		}
		static AnyConverter _Instance;

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
			if( value is IEnumerable<object> items ) {
				return items.Any();
			}

			return false;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
			throw new NotImplementedException();
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return this;
		}
	}
}
