using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wallace.UWP.Helpers.DataVirtualization {
    public class IncrementalLoadingContext<T> : IncrementalLoadingContextBase where T : class {
        public IncrementalLoadingContext(FetchDataCallbackHandler callback, int typeNumber, uint rollNumber, string targetHost ) {
            FetchCallback = callback;
            number = typeNumber;
            this.rollNumber = rollNumber;
            this.targetHost = targetHost;
            InitType = InitSelector.Default;
        }

        public IncrementalLoadingContext(FetchDataCallbackHandler callback, int typeNumber, uint rollNumber, string targetHost , InitSelector type) {
            FetchCallback = callback;
            number = typeNumber;
            this.rollNumber = rollNumber;
            this.targetHost = targetHost;
            InitType = type;
            if(InitType == InitSelector.Special) { LoadPreview(); }
        }

        public IncrementalLoadingContext(FetchDataCallbackHandler callback, uint rollNumber, string targetHost, InitSelector type) {
            FetchCallback = callback;
            number = 0;
            this.rollNumber = rollNumber;
            this.targetHost = targetHost;
            InitType = type;
            if (InitType == InitSelector.Special) { LoadPreview(); }
        }

        private async void LoadPreview() { await LoadMoreItemsAsync(0); InitType = InitSelector.Default; }

        protected override bool HasMoreItemsOrNot() { return true; }

        protected override async Task<IList<object>> LoadItemsAsync(CancellationToken cancToken, uint count) {
            //await Task.Delay(10);
            wholeCount += rollNumber;
            var coll = await FetchCallback.Invoke( number, targetHost, rollNumber,wholeCount);

            // Is this ok?
            //return (coll as ObservableCollection<object>).ToArray();
            return coll.ToArray();
        }

        uint wholeCount = 0;
        public delegate Task<List<T>> FetchDataCallbackHandler( int ID , string host, uint rollNum, uint nowWholeCount);
        public FetchDataCallbackHandler FetchCallback;
        private int number;
        private uint rollNumber;
        private string targetHost;
        private InitSelector InitType;
    }
}
