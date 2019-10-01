// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace DynamicData.Aggregation
{
    /// <summary>
    /// A changeset which has been shaped for rapid online aggregations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAggregateChangeSet<T> : IEnumerable<AggregateItem<T>>
    {
    }
}
