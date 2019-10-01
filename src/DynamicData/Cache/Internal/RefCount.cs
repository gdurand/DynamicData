﻿// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Annotations;

namespace DynamicData.Cache.Internal
{
    internal class RefCount<TObject, TKey>
    {
        private readonly IObservable<IChangeSet<TObject, TKey>> _source;
        private readonly object _locker = new object();
        private int _refCount;
        private IObservableCache<TObject, TKey> _cache;

        public RefCount([NotNull] IObservable<IChangeSet<TObject, TKey>> source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IObservable<IChangeSet<TObject, TKey>> Run()
        {
            return Observable.Create<IChangeSet<TObject, TKey>>(observer =>
            {
                lock (_locker)
                {
                    if (++_refCount == 1)
                    {
                        _cache = _source.AsObservableCache();
                    }
                }

                var subscriber = _cache.Connect().SubscribeSafe(observer);

                return Disposable.Create(() =>
                {
                    subscriber.Dispose();
                    IDisposable cacheToDispose = null;
                    lock (_locker)
                    {
                        if (--_refCount == 0)
                        {
                            cacheToDispose = _cache;
                            _cache = null;
                        }
                    }

                    cacheToDispose?.Dispose();
                });
            });
        }
    }
}
