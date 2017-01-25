using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace Wallace.UWP.Helpers.Tools {
    public class ImageHelpers {

        public static async Task<byte[]> GetThumbImageFromUriToBytesAsync(string url) {
            using (HttpClient client = new HttpClient()) {
                var resp = await client.GetAsync(new Uri(url));
                resp.EnsureSuccessStatusCode();
                using (var inputStream = await resp.Content.ReadAsInputStreamAsync()) {
                    return await ReadBytesFromSoftwareBitmapAsync(await CreateSoftwareBitmapFromInputStreamAsync(inputStream));
                }
            }
        }

        public static async Task<SoftwareBitmap> CreateSoftwareBitmapFromInputStreamAsync(IInputStream inputStream) {
            var memStream = new InMemoryRandomAccessStream();
            await RandomAccessStream.CopyAsync(inputStream, memStream);
            var decoder = await BitmapDecoder.CreateAsync(memStream);
            SoftwareBitmap softBmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            return softBmp;
        }

        public static IRandomAccessStreamReference CreateThumbnailFromUri(Uri uri) {
            return RandomAccessStreamReference.CreateFromUri(uri);
        }

        public static async Task<byte[]> ReadBytesFromSoftwareBitmapAsync(SoftwareBitmap softBmp) {
            using (IRandomAccessStream rstream = new InMemoryRandomAccessStream()) {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, rstream);
                encoder.SetSoftwareBitmap(softBmp);
                encoder.BitmapTransform.ScaledWidth = 100;
                encoder.BitmapTransform.ScaledHeight = 100;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                encoder.IsThumbnailGenerated = true;
                await encoder.FlushAsync();
                var bytes = new byte[rstream.Size];
                await rstream.AsStream().ReadAsync(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static async Task<StorageFile> GetSoftwareBitmapFromUriStringAsync(string url) {
            using (HttpClient client = new HttpClient()) {
                var resp = await client.GetAsync(new Uri(url));
                resp.EnsureSuccessStatusCode();
                using (var inputStream = await resp.Content.ReadAsInputStreamAsync()) {
                    return await GetTempImageFileFromSoftwareBitmapAsync(await CreateSoftwareBitmapFromInputStreamAsync(inputStream));
                }
            }
        }

        public static async Task<StorageFile> GetTempImageFileFromSoftwareBitmapAsync(SoftwareBitmap softwareBitmap) {
            string fileName = "SHARE" + "-" +
                Guid.NewGuid().ToString() + "-" +
                DateTime.Now.ToString("yyyy-MM--dd-hh-mm-ss") + ".jpg";

            if (softwareBitmap != null) {
                StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(softwareBitmap);
                    await encoder.FlushAsync();
                    return file;
                }
            }

            return null;
        }

    }
}
