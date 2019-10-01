// Copyright (c) 2011-2019 Roland Pheasant. All rights reserved.
// Roland Pheasant licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using DynamicData.Kernel;

namespace DynamicData.Cache.Internal
{
    internal sealed class ReaderWriter<TObject, TKey>
    {
        private readonly Func<TObject, TKey> _keySelector;
        private Dictionary<TKey,TObject> _data = new Dictionary<TKey, TObject>(); //could do with priming this on first time load
        private CacheUpdater<TObject, TKey> _activeUpdater;

        private readonly object _locker = new object();

        public ReaderWriter(Func<TObject, TKey> keySelector = null) => _keySelector = keySelector;

        #region Writers

        public ChangeSet<TObject, TKey> Write(IChangeSet<TObject, TKey> changes, Action<ChangeSet<TObject, TKey>> previewHandler, bool collectChanges)
        {
            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            return DoUpdate(updater => updater.Clone(changes), previewHandler, collectChanges);
        }

        public ChangeSet<TObject, TKey> Write(Action<ICacheUpdater<TObject, TKey>> updateAction, Action<ChangeSet<TObject, TKey>> previewHandler, bool collectChanges)
        {
            if (updateAction == null)
            {
                throw new ArgumentNullException(nameof(updateAction));
            }

            return DoUpdate(updateAction, previewHandler, collectChanges);
        }

        public ChangeSet<TObject, TKey> Write(Action<ISourceUpdater<TObject, TKey>> updateAction, Action<ChangeSet<TObject, TKey>> previewHandler, bool collectChanges)
        {
            if (updateAction == null)
            {
                throw new ArgumentNullException(nameof(updateAction));
            }

            return DoUpdate(updateAction, previewHandler, collectChanges);
        }

        private ChangeSet<TObject, TKey> DoUpdate(Action<CacheUpdater<TObject, TKey>> updateAction, Action<ChangeSet<TObject, TKey>> previewHandler, bool collectChanges)
        {
            lock (_locker)
            {
                if (previewHandler != null)
                {
                    var copy = new Dictionary<TKey, TObject>(_data);
                    var changeAwareCache = new ChangeAwareCache<TObject, TKey>(_data);

                    _activeUpdater = new CacheUpdater<TObject, TKey>(changeAwareCache, _keySelector);
                    updateAction(_activeUpdater);
                    _activeUpdater = null;

                    var changes = changeAwareCache.CaptureChanges();

                    InternalEx.Swap(ref copy, ref _data);
                    previewHandler(changes);
                    InternalEx.Swap(ref copy, ref _data);

                    return changes;
                }

                if (collectChanges)
                {
                    var changeAwareCache = new ChangeAwareCache<TObject, TKey>(_data);

                    _activeUpdater = new CacheUpdater<TObject, TKey>(changeAwareCache, _keySelector);
                    updateAction(_activeUpdater);
                    _activeUpdater = null;

                    return changeAwareCache.CaptureChanges();
                }

                _activeUpdater = new CacheUpdater<TObject, TKey>(_data, _keySelector);
                updateAction(_activeUpdater);
                _activeUpdater = null;

                return ChangeSet<TObject, TKey>.Empty;
            }
        }

        internal void WriteNested(Action<ISourceUpdater<TObject, TKey>> updateAction)
        {
            lock (_locker)
            {
                if (_activeUpdater == null)
                {
                    throw new InvalidOperationException("WriteNested can only be used if another write is already in progress.");
                }

                updateAction(_activeUpdater);
            }
        }

        #endregion

        #region Accessors

        public ChangeSet<TObject, TKey> GetInitialUpdates( Func<TObject, bool> filter = null)
        {
            lock (_locker)
            {
                var dictionary = _data;

                if (dictionary.Count == 0)
                {
                    return ChangeSet<TObject, TKey>.Empty;
                }

                var changes = filter == null
                    ? new ChangeSet<TObject, TKey>(dictionary.Count)
                    : new ChangeSet<TObject, TKey>();

                foreach (var kvp in dictionary)
                {
                    if (filter == null || filter(kvp.Value))
                    {
                        changes.Add(new Change<TObject, TKey>(ChangeReason.Add, kvp.Key, kvp.Value));
                    }
                }

                return changes;
            }
        }

        public TKey[] Keys
        {
            get
            {
                lock (_locker)
                {
                    TKey[] result = new TKey[_data.Count];
                    _data.Keys.CopyTo(result, 0);
                    return result;
                }
            }
        }

        public KeyValuePair<TKey, TObject>[] KeyValues
        {
            get
            {
                lock (_locker)
                {
                    KeyValuePair<TKey, TObject>[] result = new KeyValuePair<TKey, TObject>[_data.Count];
                    int i = 0;
                    foreach (var kvp in _data)
                    {
                        result[i] = kvp;
                        i++;
                    }

                    return result;
                }
            }
        }

        public TObject[] Items
        {
            get
            {
                lock (_locker)
                {
                    TObject[] result = new TObject[_data.Count];
                    _data.Values.CopyTo(result, 0);
                    return result;
                }
            }
        }

        public Optional<TObject> Lookup(TKey key)
        {
            lock (_locker)
            {
                return _data.Lookup(key);
            }
        }

        public int Count
        {
            get
            {
                lock (_locker)
                {
                    return _data.Count;
                }
            }
        }

        #endregion
    }
}
