using RestSharp.Deserializers;
using Newtonsoft.Json;
using System.Globalization;
using RestSharp;
namespace JanusApi
{
  class JanusDeserializer : IDeserializer
  {
    public string RootElement { get; set; }
    public string Namespace { get; set; }
    public string DateFormat { get; set; }
    public CultureInfo Culture { get; set; }

    public JanusDeserializer()
    {
      Culture = CultureInfo.InvariantCulture;
    }

    public T Deserialize<T>(IRestResponse response)
    {
      return JsonConvert.DeserializeObject<T>(response.Content);
    }
  }
}
