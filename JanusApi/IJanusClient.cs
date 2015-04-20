using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JanusApi
{
  public interface IJanusClient
  {
    /// <summary>
    /// This will send the given request to the janus server. And respond with an object of type T.
    /// </summary>
    /// <typeparam name="T">Response object type</typeparam>
    /// <param name="Request">The request object to be serialized and sent</param>
    /// <param name="type">The request type</param>
    /// <returns>An object of type T containing the response information</returns>
    T Execute<T>(dynamic Request, JanusRequestType type);

    /// <summary>
    /// This will send the given request to the janus server in pertaining to the specified plugin
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Request"></param>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    T Execute<T>(dynamic Request, JanusRequestType type, JanusPluginType plugin);

    /// <summary>
    /// This removes any stored connection information such as session handles and plugin handles. 
    /// </summary>
    void ClearConnectionInfo();

    /// <summary>
    /// Gets the current Janus session handle
    /// </summary>
    /// <returns></returns>
    long GetSessionHandle();

    /// <summary>
    /// Gets the current list of attached plugins
    /// </summary>
    /// <returns></returns>
    List<JanusPluginType> GetAttachedPlugins();
  }
}
