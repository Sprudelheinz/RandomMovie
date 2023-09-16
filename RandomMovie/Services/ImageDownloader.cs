﻿namespace RandomMovie.Services
{
    public class ImageDownloader : IDisposable
    {
        private bool _disposed;
        private readonly HttpClient _httpClient;


        public ImageDownloader(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Downloads an image asynchronously from the <paramref name="uri"/> and places it in the specified <paramref name="directoryPath"/> with the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directoryPath">The relative or absolute path to the directory to place the image in.</param>
        /// <param name="fileName">The name of the file without the file extension.</param>
        /// <param name="uri">The URI for the image to download.</param>
        public async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri, Movie movie)
        {
            if (_disposed) { throw new ObjectDisposedException(GetType().FullName); }

            // Get the file extension
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            // Create file path and ensure directory exists
            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            // Download the image and write to the file
            var imageBytes = await _httpClient.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(path, imageBytes);
            movie.PosterImageSource = ImageSource.FromFile(path);
        }

        public void Dispose()
        {
            if (_disposed) { return; }
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}
