namespace Sales.API.Helpers.Interfaces
{

    /// <summary>
    /// The interface IFileStorage
    /// </summary>

    public interface IFileStorage
    {

        #region Methods

        Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

        Task RemoveFileAsync(string path, string nombreContenedor);

        async Task<string> EditFileAsync(byte[] content, string extention, string containerName, string path)
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