// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DynamicData
{
    /// <summary>
    /// A readonly observable list, providing  observable methods
    /// as well as data access methods
    /// </summary>
    public interface IObservableList<T> : IDisposable
    {
        /// <summary>
        /// Connect to the observable list and observe any changes
        /// starting with the list's initial items. 
        /// </summary>
        /// <param name="predicate">The result will be filtered on the specified predicate.</param>
        IObservable<IChangeSet<T>> Connect(Func<T, bool> predicate = null);

        /// <summary>
        /// Connect to the observable list and observe any changes before they are applied to the list.
        /// Unlike Connect(), the returned observable is not prepended with the lists initial items.
        /// </summary>
        /// <param name="predicate">The result will be filtered on the specified predicate.</param>
        IObservable<IChangeSet<T>> Preview(Func<T, bool> predicate = null);

        /// <summary>
        /// Observe the count changes, starting with the inital items count
        /// </summary>
        IObservable<int> CountChanged { get; }

        /// <summary>
        /// Items enumerable
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        int Count { get; }
    }
}
