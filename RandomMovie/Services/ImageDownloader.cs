namespace RandomMovie.Services
{
    public class ImageDownloader : IDisposable
    {
        private bool m_disposed;
        private readonly HttpClient m_httpClient;


        public ImageDownloader(HttpClient httpClient = null)
        {
            m_httpClient = httpClient ?? new HttpClient();
        }

        public async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri, Movie movie)
        {
            if (m_disposed) 
            { 
                throw new ObjectDisposedException(GetType().FullName); 
            }

            // Get the file extension
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            // Create file path and ensure directory exists
            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);
            try
            {
                // Download the image and write to the file
                var imageBytes = await m_httpClient.GetByteArrayAsync(uri);
                await File.WriteAllBytesAsync(path, imageBytes);
                movie.PosterImageSource = ImageSource.FromFile(path);
                movie.PosterNotAvailable = false;
            }
            catch
            {
                movie.PosterImageSource = null;
                movie.PosterNotAvailable = true;
            }
            
        }

        public void Dispose()
        {
            if (m_disposed) { return; }
            m_httpClient.Dispose();
            GC.SuppressFinalize(this);
            m_disposed = true;
        }
    }
}
