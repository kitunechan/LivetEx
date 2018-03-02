using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace LivetEx.Converters {
	public class EqualConverter : MarkupExtension, IValueConverter {
		public EqualConverter() {

		}

		public EqualConverter( object obj ) {
			this.obj = obj;
		}

		object obj;


		public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
			this.obj = obj ?? parameter;
			return Equals( value, obj );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
			throw new NotImplementedException();
		}

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return this;
		}
	}
}
