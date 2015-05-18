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
    }

    void _wsclient_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        lock (messageSync)
        {
            _receivedMessage = e.Message;
            Monitor.PulseAll(messageSync);
        }
    }

    public override T Execute<T>(dynamic Request, JanusRequestType type)
    {
        if(type != JanusRequestType.Create)
        {
            if (session_handle <= 0)
                throw new Exception("Session with janus has not been appropriately initialized");
            Request.session_id = session_handle;
        }
        var resp = Execute<T>(Request);
        if(type == JanusRequestType.Create)
        {
            SetSessionHandleFromResponse(resp);
        }
        else if(type == JanusRequestType.Destroy)
        {
            session_handle = 0;
        }
        return resp;
    }

    public override T Execute<T>(dynamic Request, JanusRequestType type, JanusPluginType plugin)
    {
        if (type == JanusRequestType.Create || type == JanusRequestType.Destroy)
            return this.Execute<T>(Request, type);
        if(session_handle == 0)
        {
            throw new Exception("Janus session is not initialized");
        }
        Request.session_id = session_handle;
        if (!plugin_handles.Keys.Contains(plugin) && type != JanusRequestType.Attach)
        {
            throw new Exception("The desired plugin must be attached before action can be taken against it");
        }
        if(type != JanusRequestType.Attach)
        {
            Request.handle_id = plugin_handles[plugin];
        }
       var resp = Execute<T>(Request); 
        
        if(type == JanusRequestType.Attach)
        {
            AddPluginHandleFromResponse(resp, plugin);
        }
        if(type == JanusRequestType.Detach)
        {
            long id;
            plugin_handles.TryRemove(plugin, out id);
        }
        return resp;
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
            _receivedMessage = String.Empty;
            Console.WriteLine(toSend);
            _wsclient.Send(toSend);
            Monitor.Wait(messageSync, 30000);
            returnVal = _receivedMessage;
        }
        return returnVal; 
    }

    private T Execute<T>(dynamic obj)
    {
        return JsonConvert.DeserializeObject<T>(SendRecvOnSocketSync(JsonConvert.SerializeObject(obj)));
    }
  }
}
