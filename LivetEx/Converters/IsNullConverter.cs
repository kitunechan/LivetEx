using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace LivetEx.Converters {

	public class IsNullConverter : MarkupExtension, IValueConverter {
		public static IsNullConverter Instance {
			get {
				if( _Instance == null ) {
					_Instance = new IsNullConverter();
				}
				return _Instance;
			}
		}
		static IsNullConverter _Instance;

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
			return value == null;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
			throw new NotImplementedException();
		}


		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return this;
		}
	}

}
