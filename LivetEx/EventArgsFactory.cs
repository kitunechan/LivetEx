﻿using System.ComponentModel;
using System.Collections.Concurrent;

namespace LivetEx
{
    internal static class EventArgsFactory
    {
        private static ConcurrentDictionary<string, PropertyChangedEventArgs> _propertyChangedEventArgsDictionary = new ConcurrentDictionary<string, PropertyChangedEventArgs>();

        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            return _propertyChangedEventArgsDictionary.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));
        }
    }
}
