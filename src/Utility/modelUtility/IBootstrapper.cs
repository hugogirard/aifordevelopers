namespace modelUtility
{
    public interface IBootstrapper
    {
        Task CreateModelDocument(string modelId, string description);

        Task CreateIndexingResources();
    }
}