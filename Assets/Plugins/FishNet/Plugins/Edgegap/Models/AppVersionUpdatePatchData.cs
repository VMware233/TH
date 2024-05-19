using Newtonsoft.Json;

namespace Edgegap
{
    //[Obsolete("Use UpdateAppVersionRequest")] // MIRROR CHANGE: commented this out to avoid import warnings
    public struct AppVersionUpdatePatchData
    {

        [JsonProperty("docker_repository")]
        public string DockerRegistry;

        [JsonProperty("docker_image")]
        public string DockerImage;

        [JsonProperty("docker_tag")]
        public string DockerTag;

    }
}
