using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wallace.UWP.Helpers.DataVirtualization;

namespace Douban.UWP.NET.Tools {
    public class DoubanLazyLoadContext<T> : LazyLoadingContextBase where T : class {

        public DoubanLazyLoadContext(FetchDataCallbackHandler callback) {
            FetchCallback = callback;
        }

        protected override async Task<IList<object>> LoadItemsAsync(CancellationToken cancToken) {
            has = false;
            var coll = await FetchCallback.Invoke();
            wholeCount += (uint)coll.Count;
            return coll.ToArray();
        }

        public override void HasMoreItemsOrNot(bool has) {
            this.has = has;
        }

        protected override bool CheckIfHasMore() {
            return has;
        }

        uint wholeCount = 0;
        bool has = true;
        public delegate Task<IList<T>> FetchDataCallbackHandler();
        public FetchDataCallbackHandler FetchCallback;
    }
}
