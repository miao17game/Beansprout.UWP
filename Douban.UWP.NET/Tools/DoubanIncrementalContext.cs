using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.DataVirtualization;

namespace Douban.UWP.NET.Tools {
    public class DoubanIncrementalContext<T> : IncrementalLoadingContextBase where T : class {

        public DoubanIncrementalContext(FetchDataCallbackHandler callback, int offset = 0 , InitSelector type = InitSelector.Default) {
            FetchCallback = callback;
            this.offset = offset;
            InitType = type;
            if(InitType == InitSelector.Special) { LoadPreviewAsync(); }
        }

        private async void LoadPreviewAsync() { await LoadMoreItemsAsync(0); InitType = InitSelector.Default; }

        protected override bool HasMoreItemsOrNot() { return true; }

        protected override async Task<IList<object>> LoadItemsAsync(CancellationToken cancToken, uint count) {
            var coll = await FetchCallback.Invoke(offset);
            wholeCount += (uint)coll.Count;
            offset++;
            return coll.ToArray();
        }

        uint wholeCount = 0;
        public delegate Task<ICollection<T>> FetchDataCallbackHandler(int offset);
        public FetchDataCallbackHandler FetchCallback;
        private int offset;
        private InitSelector InitType;
    }
}
