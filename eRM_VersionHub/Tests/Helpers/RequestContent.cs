using eRM_VersionHub.Services;

namespace eRM_VersionHub_Tester.Helpers
{
    public static class RequestContent
    {
        public static async Task<Func<T?>> GetRequestContent<T>(this HttpResponseMessage httpResponseMessage)
        {
            string json = await httpResponseMessage.Content.ReadAsStringAsync();
            return delegate() {
                return JsonManager.Deserialize<T>(json);
            };
        }
    }
}