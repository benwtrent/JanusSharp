using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JanusApi.Model;
using WebSocket4Net;
using log4net;
namespace JanusApi
{
  public class JanusWebSocketClient : JanusClient
  {

      private WebSocket _wsclient;
      private string _receivedMessage;
      private readonly object messageSync = new object();
    public JanusWebSocketClient(string url) : base() {

        _wsclient = new WebSocket(url, "janus-protocol");
        _wsclient.Open();
        _wsclient.MessageReceived += _wsclient_MessageReceived;
        _wsclient.Closed += new EventHandler(_wsclient_Closed);
    }

    void _wsclient_Closed(object sender, EventArgs e)
    {
      ClearConnectionInfo();
    }

    public override void Close()
    {
      if (_wsclient.State == WebSocketState.Open || _wsclient.State != WebSocketState.Connecting)
        _wsclient.Close();
    }

    void _wsclient_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        lock (messageSync)
        {
            _receivedMessage = e.Message;
            Monitor.PulseAll(messageSync);
        }
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
      Request.session_id = session_handle;
      Request.handle_id = plugin_handles[plugin];

      var response = Execute<T>(Request);
      T returnVal = transformAndErrorCheck<T>(response, Request.transaction);
      return returnVal;     
    }
    
    public override JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type)
    {
      if (type != JanusRequestType.Create)
      {
        if (session_handle <= 0)
        {
          return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_SESSION_NOT_FOUND, "No session exists", Request.transaction);
        }
        Request.session_id = session_handle;
      }
      
      var resp = Execute<JanusBaseObject>(Request);
      JanusBaseObject returnVal = transformAndErrorCheck<JanusBaseObject>(resp, Request.transaction);

      if (returnVal.error != null)
      {
        return returnVal;
      }
      else if (type == JanusRequestType.Create)
      {
        session_handle = returnVal.data.id;
      }
      else if (type == JanusRequestType.Destroy)
      {
        session_handle = 0;
        plugin_handles.Clear();
      }
      return returnVal;
    }

    public override JanusBaseObject Execute(JanusBaseObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      if (type == JanusRequestType.Create || type == JanusRequestType.Destroy)
        return this.Execute(Request, type);

      if (session_handle <= 0)
      {
        return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_SESSION_NOT_FOUND, "No session found", Request.transaction);
      }
      if (!plugin_handles.ContainsKey(plugin) && type != JanusRequestType.Attach)
      {
        return getNewError<JanusBaseObject>(JanusBaseErrorCodes.JANUS_ERROR_PLUGIN_NOT_FOUND, "Plugin: " + plugin.Type + " not attached", Request.transaction);
      }
      Request.session_id = session_handle;
      if (type != JanusRequestType.Attach)
      {
        Request.handle_id = plugin_handles[plugin];
      }

      var resp = Execute<JanusBaseObject>(Request);
      JanusBaseObject returnVal = transformAndErrorCheck<JanusBaseObject>(resp, Request.transaction);
      if (returnVal.error != null)
        return returnVal;

      if (type == JanusRequestType.Attach)
        plugin_handles[plugin] = returnVal.data.id;
      if (type == JanusRequestType.Detach)
      {
        long temp_id;
        plugin_handles.TryRemove(plugin, out temp_id);
      }
      return returnVal;
    }

    public override JanusVideoRoomExistsObject Execute(JanusVideoRoomExistsObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return Execute<JanusVideoRoomExistsObject>(Request, type, plugin);
    }

    public override JanusVideoRoomInfoObject Execute(JanusVideoRoomInfoObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return Execute<JanusVideoRoomInfoObject>(Request, type, plugin);
    }

    public override JanusVideoRoomListObject Execute(JanusVideoRoomListObject Request, JanusRequestType type, JanusPluginType plugin)
    {
      return Execute<JanusVideoRoomListObject>(Request, type, plugin);
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
    }

    private string SerializeObject(dynamic obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    private T DeserializeString<T>(string str)
    {
        return JsonConvert.DeserializeObject<T>(str);
    }

    private string SendRecvOnSocketSync(string toSend)
    {
        string returnVal = "";
        lock (messageSync)
        {
#if DEBUG
          Console.WriteLine("Send: {0}", toSend);
#endif
          log.DebugFormat("Send: {0}", toSend);
            _receivedMessage = String.Empty;
            _wsclient.Send(toSend);
            Monitor.Wait(messageSync, 30000);
            returnVal = _receivedMessage;
#if DEBUG
            Console.WriteLine("Recv: {0}", returnVal);
#endif
            log.DebugFormat("Recv: {0}", returnVal);
            
        }
        return returnVal; 
    }

    private T Execute<T>(dynamic obj) where T: JanusBaseObject, new()
    {
      return JsonConvert.DeserializeObject<T>(SendRecvOnSocketSync(JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })));
    }
  }
}
