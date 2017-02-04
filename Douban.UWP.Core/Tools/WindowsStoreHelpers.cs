using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace Douban.UWP.Core.Tools {
    public static class WindowsStoreHelpers {

        public static async Task<bool> GetProductInfoAsync(StoreContext context, string id, string filter = "Durable") {
            if (context == null)
                context = StoreContext.GetDefault();

            var filterList = new string[] { filter };
            var storeIds = new string[] { id };

            var queryResult = await context.GetStoreProductsAsync(filterList, storeIds);

            if (queryResult.ExtendedError != null) {
                System.Diagnostics.Debug.WriteLine($"ExtendedError: {queryResult.ExtendedError.Message}");
                return false;
            }

            bool have = false;
            queryResult.Products.Values.ToList().ForEach(item => have = item.IsInUserCollection);

            return have;
        }


        public static async Task<PurchasAddOnReturn> PurchaseAddOnAsync(StoreContext context, string storeId) {
            if (context == null)
                context = StoreContext.GetDefault();

            StorePurchaseResult result = await context.RequestPurchaseAsync(storeId);

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null) 
                extendedError = result.ExtendedError.Message;

            PurchasAddOnReturn resultToReturn = default(PurchasAddOnReturn);

            switch (result.Status) {
                case StorePurchaseStatus.AlreadyPurchased:
                    System.Diagnostics.Debug.WriteLine("The user has already purchased the product.");
                    resultToReturn = PurchasAddOnReturn.Failed;
                    break;

                case StorePurchaseStatus.Succeeded:
                    System.Diagnostics.Debug.WriteLine("The purchase was successful.");
                    resultToReturn = PurchasAddOnReturn.Successful;
                    break;

                case StorePurchaseStatus.NotPurchased:
                    System.Diagnostics.Debug.WriteLine("The purchase did not complete. " + "The user may have cancelled the purchase. ExtendedError: " + extendedError);
                    break;

                case StorePurchaseStatus.NetworkError:
                    System.Diagnostics.Debug.WriteLine("The purchase was unsuccessful due to a network error. " + "ExtendedError: " + extendedError);
                    resultToReturn = PurchasAddOnReturn.Failed;
                    break;

                case StorePurchaseStatus.ServerError:
                    System.Diagnostics.Debug.WriteLine("The purchase was unsuccessful due to a server error. " + "ExtendedError: " + extendedError);
                    resultToReturn = PurchasAddOnReturn.Failed;
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("The purchase was unsuccessful due to an unknown error. " + "ExtendedError: " + extendedError);
                    resultToReturn = PurchasAddOnReturn.Failed;
                    break;
            }

            return resultToReturn;

        }
    }

    public enum PurchasAddOnReturn {
        Unknown,
        Successful,
        Failed
    }

}
