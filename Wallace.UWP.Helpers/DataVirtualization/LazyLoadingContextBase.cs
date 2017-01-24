using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Wallace.UWP.Helpers.DataVirtualization {
    public abstract class LazyLoadingContextBase : IList , ISupportIncrementalLoading, INotifyCollectionChanged {

        #region IList

        public object this[int index] {get { return sourceList[index]; } set { sourceList[index] = value; } }

        public int Count { get { return sourceList.Count; } }

        public bool IsFixedSize { get { return false; } }

        public bool IsReadOnly { get { return false; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot {get { throw new NotImplementedException(); } }

        public int Add(object value) { throw new NotImplementedException();}

        public void Clear() { sourceList.Clear(); }

        public bool Contains(object value) { return sourceList.Contains(value); }

        public void CopyTo(Array array, int index) { ((IList)sourceList).CopyTo(array, index); }

        public IEnumerator GetEnumerator() { return sourceList.GetEnumerator(); }

        public int IndexOf(object value) { return sourceList.IndexOf(value); }

        public void Insert(int index, object value) { throw new NotImplementedException(); }

        public void Remove(object value) { throw new NotImplementedException(); }

        public void RemoveAt(int index) { throw new NotImplementedException(); }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) {
            if (IsOnAsyncWorkOrNot)
                return AsyncInfo.Run((cancToken) => LoadNoItemsAsync(cancToken));
            IsOnAsyncWorkOrNot = true;
            return AsyncInfo.Run((cancToken) => LoadMoreItemsAsync(cancToken));
        }

        public bool HasMoreItems { get { return CheckIfHasMore(); } }

        #endregion

        #region Methods
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken cancToken) {
            try {
                var items = await LoadItemsAsync(cancToken);
                var baseIndex = sourceList.Count;
                (sourceList as List<object>).AddRange(items);
                NotifyWhenItemsInserted(baseIndex, items.Count);
                return new LoadMoreItemsResult { Count = (uint)items.Count };
            } finally { IsOnAsyncWorkOrNot = false; }
        }

        async Task<LoadMoreItemsResult> LoadNoItemsAsync(CancellationToken cancToken) {
            await Task.Delay(10);
            return new LoadMoreItemsResult { Count = 0 };
        }

        void NotifyWhenItemsInserted(int baseIndex, int count) {
            if (CollectionChanged == null) { return; }
            for (int i = 0; i < count; i++) {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, sourceList[i + baseIndex], i + baseIndex);
                CollectionChanged(this, args);
            }
        }
        #endregion

        #region Override Methods
        protected abstract Task<IList<object>> LoadItemsAsync(CancellationToken cancToken);
        protected abstract bool CheckIfHasMore();
        public abstract void HasMoreItemsOrNot(bool has);
        #endregion

        #region State
        IList<object> sourceList = new List<object>();
        bool IsOnAsyncWorkOrNot = false;
        #endregion

    }
}
