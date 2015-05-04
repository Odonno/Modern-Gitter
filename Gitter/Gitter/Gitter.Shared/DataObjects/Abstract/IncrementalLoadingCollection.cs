using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.DataObjects.Abstract
{
    public abstract class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public bool HasMoreItems { get { return true; } }
        public int Page { get; protected set; }
        public int ItemsPerPage { get; protected set; }
        public bool IsBusy { get; protected set; }
        public bool Ascendant { get; protected set; }


        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsBusy)
                throw new InvalidOperationException("Only one operation in flight at a time");
            IsBusy = true;

            try
            {
                CoreDispatcher dispatcher = Window.Current.Dispatcher;

                return Task.Run(
                    async () =>
                    {
                        var items = await LoadMoreItemsAsync();

                        dispatcher.RunAsync(
                            CoreDispatcherPriority.High,
                            () =>
                            {
                                if (Ascendant)
                                {
                                    int i = (Page - 1) * ItemsPerPage;
                                    foreach (var item in items)
                                        Insert(i, item);
                                }
                                else
                                {
                                    foreach (var item in items)
                                        Add(item);
                                }
                            });

                        return new LoadMoreItemsResult { Count = (uint)items.Count() };
                    }).AsAsyncOperation();
            }
            finally
            {
                IsBusy = false;
            }
        }
        protected abstract Task<IEnumerable<T>> LoadMoreItemsAsync();
        public void Reset()
        {
            Clear();
            Page = 0;
        }
    }
}
