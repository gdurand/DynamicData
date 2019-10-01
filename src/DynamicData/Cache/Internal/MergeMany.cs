// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;

namespace DynamicData.Cache.Internal
{
    internal class MergeMany<TObject, TKey, TDestination>
    {
        private readonly IObservable<IChangeSet<TObject, TKey>> _source;
        private readonly Func<TObject, TKey, IObservable<TDestination>> _observableSelector;

        public MergeMany(IObservable<IChangeSet<TObject, TKey>> source, Func<TObject, TKey, IObservable<TDestination>> observableSelector)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _observableSelector = observableSelector ?? throw new ArgumentNullException(nameof(observableSelector));
        }

        public MergeMany(IObservable<IChangeSet<TObject, TKey>> source, Func<TObject, IObservable<TDestination>> observableSelector)
        {
            if (observableSelector == null)
            {
                throw new ArgumentNullException(nameof(observableSelector));
            }

            _source = source ?? throw new ArgumentNullException(nameof(source));
            _observableSelector = (t, key) => observableSelector(t);
        }

        public IObservable<TDestination> Run()
        {
            return Observable.Create<TDestination>
            (
                observer => _source.SubscribeMany(
                        (t, key) => _observableSelector(t, key)
                            .Subscribe(
                                observer.OnNext,
                                ex => {},
                                ( ) => {}
                                )
                        )
                    .Subscribe(t => { }, observer.OnError));
        }
    }
}