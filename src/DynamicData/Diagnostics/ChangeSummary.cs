﻿// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace DynamicData.Diagnostics
{
    /// <summary>
    /// Accumulates change statics
    /// </summary>
    public class ChangeSummary
    {
        private readonly int _index;

        /// <summary>
        /// An empty instance of change summary
        /// </summary>
        public static readonly ChangeSummary Empty = new ChangeSummary();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ChangeSummary(int index, ChangeStatistics latest, ChangeStatistics overall)
        {
            Latest = latest;
            Overall = overall;
            _index = index;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        private ChangeSummary()
        {
            _index = -1;
            Latest = new ChangeStatistics();
            Overall = new ChangeStatistics();
        }

        /// <summary>
        /// Gets the latest change
        /// </summary>
        /// <value>
        /// The latest.
        /// </value>
        public ChangeStatistics Latest { get; }

        /// <summary>
        /// Gets the overall change count
        /// </summary>
        /// <value>
        /// The overall.
        /// </value>
        public ChangeStatistics Overall { get; }

        #region Equality members

        private bool Equals(ChangeSummary other)
        {
            return _index == other._index && Equals(Latest, other.Latest) && Equals(Overall, other.Overall);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ChangeSummary)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _index;
                hashCode = (hashCode * 397) ^ (Latest?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Overall?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion

        /// <inheritdoc />
        public override string ToString() => $"CurrentIndex: {_index}, Latest Count: {Latest.Count}, Overall Count: {Overall.Count}";
    }
}
