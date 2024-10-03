using update_stash_api.Services.DTOs;

namespace update_stash_api.Services
{
    public interface IStashGraphQlService
    {
        Task<JobStatus> FindJob(string jobId);
        Task<FindScenesList> GetAllScenes(string fragmentQuery = "");
        Task<string> QueueMetadataIdentifyJob(string path);
        Task<string> QueueMetadataScanJob(string path);
        Task<Scene> SceneUpdate(SceneUpdateInput scene);
        Task<Scene> ScrapeSceneId(string javId);
        Task<Scene> ScrapeSceneUrl(string url);
        Task<Scene> UpdateScene(Scene updatedScene);
    }
}