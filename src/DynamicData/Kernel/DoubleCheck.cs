// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace DynamicData.Kernel
{
    /// <summary>
    /// Very simple, primitive yet light weight lazy loader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DoubleCheck<T>
        where T : class
    {
        private readonly Func<T> _factory;
        private readonly object _locker = new object();
        private volatile T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleCheck{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <exception cref="System.ArgumentNullException">factory</exception>
        public DoubleCheck(Func<T> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        /// Gets the value. Factory is execute when first called
        /// </summary>
        public T Value
        {
            get
            {
                if (_value != null)
                {
                    return _value;
                }

                lock (_locker)
                {
                    if (_value == null)
                    {
                        _value = _factory();
                    }
                }

                return _value;
            }
        }
    }
}
