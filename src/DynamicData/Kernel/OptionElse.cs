﻿// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace DynamicData.Kernel
{
    /// <summary>
    /// Continuation container used for the else optator on an option object.
    /// </summary>
    public sealed class OptionElse
    {
        internal static readonly OptionElse NoAction = new OptionElse(false);

        private readonly bool _shouldRunAction;

        internal OptionElse(bool shouldRunAction = true)
        {
            _shouldRunAction = shouldRunAction;
        }

        /// <summary>
        /// Invokes the specified action when an option has no value.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public void Else(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (_shouldRunAction)
            {
                action();
            }
        }
    }
}
