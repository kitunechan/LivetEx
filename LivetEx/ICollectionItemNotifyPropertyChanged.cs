using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivetEx {
	public interface ICollectionItemNotifyPropertyChanged {
		event PropertyChangedEventHandler CollectionItemNotifyPropertyChanged;
	}
}
