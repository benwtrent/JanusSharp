using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using JanusApi.Model;
namespace JanusApi
{
  public class JanusRestHTTPClient : IJanusClient
  {
    private ConcurrentDictionary<JanusPluginType, long> plugin_handles;
    private RestClient _client;
    private long session_handle;

    public JanusRestHTTPClient(String url) {
      _client = new RestClient();
      _client.BaseUrl = url;
      _client.Timeout = 30000;
    }

    public T Execute<T>(dynamic request, JanusRequestType type) where T: new()
    {
      RestRequest rest_request = new RestRequest(Method.POST);
      rest_request.RequestFormat = DataFormat.Json;
      if (session_handle > 0)
      {
        rest_request.Resource = "{SessionToken}";
      }
      else if(session_handle == 0 && type != JanusRequestType.Create)
      {
        throw new Exception("Session with janus has not been appropriately initialized");
      }
      rest_request.AddBody(request);

      var response = _client.Execute<T>(rest_request);

      if (type == JanusRequestType.Create)
      {
        JanusBaseResponse resp = response.Data as JanusBaseResponse;
        if (resp != null)
        {
          session_handle = resp.data.id;
          if (session_handle > 0)
            _client.AddDefaultUrlSegment("SessionToken", session_handle.ToString());
        }
      }
      else if (type == JanusRequestType.Destroy)
      {
        session_handle = 0;
        _client.RemoveDefaultParameter("SessionToken");
      }
      return response.Data;
    }

    public T Execute<T>(dynamic request, JanusRequestType type, JanusPluginType plugin) where T : new()
    {
      RestRequest rest_request = new RestRequest(Method.POST);
      rest_request.RequestFormat = DataFormat.Json;
      if(type == JanusRequestType.Create || type == JanusRequestType.Destroy)
        return Execute<T>(request, type);
      if (session_handle == 0)
      {
        throw new Exception("Janus session is not initialized");
      }
      if (!plugin_handles.Keys.Contains(plugin) && type != JanusRequestType.Attach)
      {
        throw new Exception("The desired plugin must be attached before action can be taken against it");
      }
      if (type != JanusRequestType.Attach)
      {
        rest_request.Resource = String.Join(String.Empty, "{SessionToken}", plugin_handles[plugin].ToString());
      }
      else
      {
        rest_request.Resource = "{SessionToken}";
      }
      rest_request.AddBody(request);
      var response = _client.Execute<T>(rest_request);
      if (type == JanusRequestType.Attach)
      {
        JanusBaseResponse resp = response.Data as JanusBaseResponse;
        if (resp != null)
        {
          long pluginhandle = resp.data.id;
          if (pluginhandle > 0)
          {
            plugin_handles[plugin] = pluginhandle;
          }
        }
      }
      if (type == JanusRequestType.Detach)
      {
        long id;
        plugin_handles.TryRemove(plugin, out id);
      }

      return response.Data;
    }

    public void ClearConnectionInfo()
    {
      plugin_handles.Clear();
      session_handle = 0;
      _client.RemoveDefaultParameter("SessionToken");
    }

    public long GetSessionHandle()
    {
      return session_handle;
    }

    public List<JanusPluginType> GetAttachedPlugins()
    {
      return plugin_handles.Keys.ToList();
    }
  }
}
