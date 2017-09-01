using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LivetEx {
	public class SystemTypeExtension : MarkupExtension {
		private object parameter;

		public int Int { set { parameter = value; } }
		public double Double { set { parameter = value; } }
		public float Float { set { parameter = value; } }
		public bool Bool { set { parameter = value; } }
		

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return parameter;
		}
	}

	public class IntExtension : MarkupExtension {
		public IntExtension( int value ) {
			this.value = value;
		}

		public int value { get; set; }

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return value;
		}
	}

	public class DoubleExtension : MarkupExtension {
		public DoubleExtension( double value ) {
			this.value = value;
		}

		public double value { get; set; }

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return value;
		}
	}

	public class FloatExtension : MarkupExtension {
		public FloatExtension( float value ) {
			this.value = value;
		}

		public float value { get; set; }

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return value;
		}
	}

	public class BoolExtension : MarkupExtension {
		public BoolExtension( bool value ) {
			this.value = value;
		}

		public bool value { get; set; }

		public override object ProvideValue( IServiceProvider serviceProvider ) {
			return value;
		}
	}


}
