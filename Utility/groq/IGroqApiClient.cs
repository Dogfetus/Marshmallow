using Newtonsoft.Json.Linq;
using System.Threading.Tasks;  // For Task<T>


namespace GroqApiLibrary
{
    public interface IGroqApiClient
    {
        Task<JObject> CreateChatCompletionAsync(JObject request);
    }
}