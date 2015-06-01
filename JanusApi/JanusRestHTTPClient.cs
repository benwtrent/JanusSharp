using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using JanusApi.Model;
using Newtonsoft.Json;
using log4net;
namespace JanusApi
{
  public class JanusRestHTTPClient : JanusClient
  {
    private RestClient _client;

    public JanusRestHTTPClient(String url) :base() {
      _client = new RestClient();
      _client.BaseUrl = url;
      _client.AddHandler("application/json", new JanusDeserializer());
      _client.Timeout = 30000;
    }

    public class JanusRestRequest : RestRequest
    {
      public JanusRestRequest()
        : base(Method.POST)
      {
        OnBeforeDeserialization = (resp) =>
        {
#if DEBUG
          Console.WriteLine("Recv: {0}, StatusCode {1}, StatusDesc {2}", resp.Content, resp.StatusCode, resp.StatusDescription);
#endif
          log.DebugFormat("Recv: {0}, StatusCode {1}, StatusDesc {2}", resp.Content, resp.StatusCode, resp.StatusDescription);
        };
        RequestFormat = DataFormat.Json;
        JanusSerializer serializer = new JanusSerializer();
        JsonSerializer = serializer;       
      }
      public void MyAddBody(object obj, long session_handle)
      {
#if DEBUG
        Console.WriteLine("Sending: \n{0}\n To url {1} with SessionID {2}", JanusSerializer.StaticSerialize(obj), Resource, session_handle);
#endif
        log.DebugFormat("Sending: \n{0}\n To url {1} with SessionID {2}", JanusSerializer.StaticSerialize(obj), Resource, session_handle);
        AddBody(obj);
      }
    }

    public override void Close()
    {
      //todo don't think we need to do anything here. 
    }

    public override JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type)
    {
      JanusRestRequest rest_request = new JanusRestRequest();
      if (session_handle > 0)
      {
        rest_request.Resource = "{SessionToken}";
      }
      else if (session_handle == 0 && type != JanusRequestType.Create)
      {
        if (session_handle <= 0)
        {
          return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_SESSION_NOT_FOUND, "No session exists", Request.transaction);
        }
      }
      rest_request.MyAddBody(Request, session_handle);

      var response = _client.Execute<JanusBaseObject>(rest_request);
      if (response.ErrorException != null)
      {
#if DEBUG
        Console.WriteLine("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
#endif
        log.ErrorFormat("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
      }
      JanusBaseObject returnVal = transformAndErrorCheck<JanusBaseObject>(response.Data, Request.transaction);
      if (returnVal.error != null)
        return returnVal;
      if (type == JanusRequestType.Create)
      {
        session_handle = returnVal.data.id;
        if (session_handle > 0)
          _client.AddDefaultUrlSegment("SessionToken", session_handle.ToString());
      }
      else if (type == JanusRequestType.Destroy)
      {
        session_handle = 0;
        _client.RemoveDefaultParameter("SessionToken");
        plugin_handles.Clear();
      }
      return returnVal;
    }

    public override JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      if(type == JanusRequestType.Create || type == JanusRequestType.Destroy)
        return this.Execute(Request, type);
      
      JanusRestRequest rest_request = new JanusRestRequest();

      if (session_handle <= 0)
      {
        return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_SESSION_NOT_FOUND, "No session found", Request.transaction);
      }
      if (!plugin_handles.ContainsKey(plugin) && type != JanusRequestType.Attach)
      {
        return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_PLUGIN_NOT_FOUND, "Plugin: " + plugin.Type + " not attached", Request.transaction);
      }

      rest_request.Resource = "{SessionToken}";
      rest_request.MyAddBody(Request, session_handle);

      var response = _client.Execute<JanusBaseObject>(rest_request);
      if (response.ErrorException != null)
      {
#if DEBUG
        Console.WriteLine("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
#endif
        log.ErrorFormat("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
      }
      JanusBaseObject returnVal = transformAndErrorCheck<JanusBaseObject>(response.Data, Request.transaction);
      if (returnVal.error != null)
        return returnVal;
      if (type == JanusRequestType.Attach)
      {
        plugin_handles[plugin] = returnVal.data.id;
      }
      if (type == JanusRequestType.Detach)
      {
        long id;
        plugin_handles.TryRemove(plugin, out id);
      }
      return returnVal;
    }

    private T Execute<T>(T Request, JanusRequestType type, JanusPluginType plugin) where T : JanusBaseObject, new()
    {
      if (session_handle <= 0)
      {
        return getNewError<T>(JanusBaseErrorCodes.JANUS_ERROR_SESSION_NOT_FOUND, "No session found", Request.transaction);
      }
      if (!plugin_handles.ContainsKey(plugin))
      {
        return getNewError<T>(JanusBaseErrorCodes.JANUS_ERROR_PLUGIN_NOT_FOUND, "Plugin: " + plugin.Type + " not attached", Request.transaction);
      }
      JanusRestRequest rest_request = new JanusRestRequest();
      rest_request.Resource = String.Join(String.Empty, "{SessionToken}", "/", plugin_handles[plugin].ToString());
      rest_request.MyAddBody(Request, session_handle);
      var response = _client.Execute<T>(rest_request);
      if (response.ErrorException != null)
      {
#if DEBUG
        Console.WriteLine("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
#endif
        log.ErrorFormat("Error message: {0}, \n\tError Exception: {1}", response.ErrorMessage, response.ErrorException.Message);
      }
      T returnVal = transformAndErrorCheck<T>(response.Data, Request.transaction);
      return returnVal;     
    }

    public override JanusVideoRoomExistsObject Execute(JanusVideoRoomExistsObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomExistsObject>(Request, type, plugin);
    }

    public override JanusVideoRoomInfoObject Execute(JanusVideoRoomInfoObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomInfoObject>(Request, type, plugin);
    }

    public override JanusVideoRoomListObject Execute(JanusVideoRoomListObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomListObject>(Request, type, plugin);
    }

    public override JanusVideoRoomCreationObject Execute(JanusVideoRoomCreationObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomCreationObject>(Request, type, plugin);
    }

    public override JanusVideoRoomDestroyObject Execute(JanusVideoRoomDestroyObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomDestroyObject>(Request, type, plugin);
    }

    public override JanusVideoRoomStreamRequestObject Execute(JanusVideoRoomStreamRequestObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return this.Execute<JanusVideoRoomStreamRequestObject>(Request, type, plugin);
    }

    public override void ClearConnectionInfo()
    {
      plugin_handles.Clear();
      session_handle = 0;
      _client.RemoveDefaultParameter("SessionToken");
    }
  }
}
