using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Gitter.DataObjects.Abstract
{
    public abstract class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        #region Properties

        public bool HasMoreItems { get; protected set; }
        public int Page { get; protected set; }
        public int ItemsPerPage { get; protected set; }
        public bool IsBusy { get; protected set; }
        public bool Ascendant { get; protected set; }

        #endregion


        #region Constructor

        protected IncrementalLoadingCollection()
        {
            HasMoreItems = true;
        }

        #endregion


        #region Methods

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (IsBusy)
                throw new InvalidOperationException("Only one operation in flight at a time");
            IsBusy = true;

            try
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                return Task.Run(
                    async () =>
                    {
                        var items = await LoadMoreItemsAsync();
                        var itemsCount = items.Count();

                        await dispatcher.RunAsync(
                            CoreDispatcherPriority.High,
                            () =>
                            {
                                if (Ascendant)
                                {
                                    int i = Count;
                                    foreach (var item in items)
                                        Insert(i, item);
                                }
                                else
                                {
                                    foreach (var item in items)
                                        Add(item);
                                }
                            });

                        return new LoadMoreItemsResult { Count = (uint)itemsCount };
                    }).AsAsyncOperation();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async virtual Task AddItemAsync(T item)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Insert(0, item);

                // Need to notify UI (chat messages) to get a correct UI (ex : grouped messages)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        protected abstract Task<IEnumerable<T>> LoadMoreItemsAsync();

        public async void Reset()
        {
            Clear();
            Page = 0;
            HasMoreItems = true;
            
            await LoadMoreItemsAsync((uint)ItemsPerPage);
        }

        #endregion
    }
}
