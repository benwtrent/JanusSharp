using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Serializers;
using Newtonsoft.Json;
namespace JanusApi
{
  class JanusSerializer: ISerializer
  {

    public JanusSerializer()
    {
      ContentType = "appliction/json";
    }

    public string Serialize(object obj)
    {
      return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }

    public static string StaticSerialize(object obj)
    {
      return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }
    public string ContentType { get; set; }
    public string DateFormat { get; set; }
    public string Namespace { get; set; }
    public string RootElement { get; set; }
  }
}
