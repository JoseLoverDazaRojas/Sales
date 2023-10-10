namespace Sales.API.Helpers.Interfaces
{

    /// <summary>
    /// The interface IFileStorage
    /// </summary>

    public interface IFileStorage
    {

        #region Methods

        public Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

        public Task RemoveFileAsync(string path, string containerName);

        public async Task<string> EditFileAsync(byte[] content, string extention, string containerName, string path)
        {
            if (path is not null)
            {
                await RemoveFileAsync(path, containerName);
            }

            return await SaveFileAsync(content, extention, containerName);
        }

        #endregion Methods

    }
}