using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using JanusApi.Model;
namespace JanusApi
{
  public abstract class JanusClient
  {
    protected long session_handle;
    protected ConcurrentDictionary<JanusPluginType, long> plugin_handles;

    public JanusClient()
    {
        plugin_handles = new ConcurrentDictionary<JanusPluginType, long>();
    }
    /// <summary>
    /// This will send the given request to the janus server. And respond with an object of type T.
    /// </summary>
    /// <typeparam name="T">Response object type</typeparam>
    /// <param name="Request">The request object to be serialized and sent</param>
    /// <param name="type">The request type</param>
    /// <returns>An object of type T containing the response information</returns>
    public virtual T Execute<T>(dynamic Request, JanusRequestType type) where T : new()
    {
      return default(T); 
    }

    /// <summary>
    /// This will send the given request to the janus server in pertaining to the specified plugin
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Request"></param>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public virtual T Execute<T>(dynamic Request, JanusRequestType type, JanusPluginType plugin) where T : new()
    {
      return default(T);
    }

    /// <summary>
    /// This removes any stored connection information such as session handles and plugin handles. 
    /// </summary>
    public abstract void ClearConnectionInfo();

    /// <summary>
    /// Gets the current Janus session handle
    /// </summary>
    /// <returns></returns>
    public long GetSessionHandle()
    {
      return session_handle; 
    }

    protected void SetSessionHandleFromResponse(dynamic obj)
    {
        JanusBaseResponse resp = obj as JanusBaseResponse;
        if(resp != null)
        {
            session_handle = resp.data.id;
        }
        else
        {
            session_handle = 0;
        }
    }

    protected void AddPluginHandleFromResponse(dynamic obj, JanusPluginType type)
    {
        JanusBaseResponse resp = obj as JanusBaseResponse;
        if(resp != null)
        {
            plugin_handles[type] = resp.data.id;
        }
        else
        {
            plugin_handles[type] = 0;
        }
    }
    /// <summary>
    /// Gets the current list of attached plugins
    /// </summary>
    /// <returns></returns>
    public List<JanusPluginType> GetAttachedPlugins()
    {
      return plugin_handles.Keys.ToList();
    }
  }
}
