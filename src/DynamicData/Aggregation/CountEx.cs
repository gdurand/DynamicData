﻿// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace DynamicData.Aggregation
{
    /// <summary>
    /// Count extensions
    /// </summary>
    public static class CountEx
    {
        /// <summary>
        /// Counts the total number of items in the underlying data source
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<int> Count<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source)
        {
            return source.ForAggregation().Count();
        }

        /// <summary>
        /// Counts the total number of items in the underlying data source
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<int> Count<TObject>(this IObservable<IChangeSet<TObject>> source)
        {
            return source.ForAggregation().Count();
        }

        /// <summary>
        /// Counts the total number of items in the underlying data source
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<int> Count<TObject>(this IObservable<IAggregateChangeSet<TObject>> source)
        {
            return source.Accumlate(0, t => 1,
                                    (current, increment) => current + increment,
                                    (current, increment) => current - increment);
        }

        /// <summary>
        /// Counts the total number of items in the underlying data source
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IObservable<int> Count<TObject>(this IObservable<IDistinctChangeSet<TObject>> source)
        {
            return source.ForAggregation().Count();
        }
    }
}
