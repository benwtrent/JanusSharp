using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using JanusApi.Model;
using log4net;
namespace JanusApi
{
  public abstract class JanusClient
  {
    protected static readonly ILog log = Logger.Create();
    protected long session_handle;
    protected ConcurrentDictionary<JanusPluginType, long> plugin_handles;

    public JanusClient()
    {
        plugin_handles = new ConcurrentDictionary<JanusPluginType, long>();
    }

    /// <summary>
    /// Close all the underlying connections if they are still open
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// This will send the given request to the janus server. And respond with an object of type T.
    /// </summary>
    /// <typeparam name="T">Response object type</typeparam>
    /// <param name="Request">The request object to be serialized and sent</param>
    /// <param name="type">The request type</param>
    /// <returns>An object of type T containing the response information</returns>
    public abstract JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type);

    /// <summary>
    /// This will send the given request to the janus server in pertaining to the specified plugin
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Request"></param>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public abstract JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomCreationObject Execute(JanusVideoRoomCreationObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomDestroyObject Execute(JanusVideoRoomDestroyObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomInfoObject Execute(JanusVideoRoomInfoObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomListObject Execute(JanusVideoRoomListObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomExistsObject Execute(JanusVideoRoomExistsObject Request, JanusRequestType type, JanusPluginType plugin);

    public abstract JanusVideoRoomStreamRequestObject Execute(JanusVideoRoomStreamRequestObject Request, JanusRequestType type, JanusPluginType plugin);

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

    /// <summary>
    /// Gets the current list of attached plugins
    /// </summary>
    /// <returns></returns>
    public List<JanusPluginType> GetAttachedPlugins()
    {
      return plugin_handles.Keys.ToList();
    }

    protected T getNewError<T>(JanusBaseErrorCodes Code, String Reason, String Trans) where T : JanusBaseObject
    {
      var error = new JanusBaseObject
      {
        janus = JanusRequestType.Error,
        transaction = Trans,
        error = new JanusBaseError
        {
          code = Code,
          reason = Reason
        }
      };
      return error as T;
    }

    protected T transformAndErrorCheck<T>(dynamic obj, String Trans) where T: JanusBaseObject
    {
      T myObj = obj as T;
      if (obj == null)
      {
        log.ErrorFormat("Unexpected return from HTTP Rest request");
        return getNewError<T>(JanusBaseErrorCodes.JANUS_ERROR_UNKNOWN, "Unknown response from janus", Trans);
      }
      if (myObj.error != null)
      {
        log.ErrorFormat("Received ErrorCode: {0}\n\t, \n\tErrorMessage: {1}\n", myObj.error.code, myObj.error.reason);
      }
      return myObj;
    }
  }
}
