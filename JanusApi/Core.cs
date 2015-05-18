#region License
/** Janus Sharp
 * Copyright (C) <2014> Benjamin Trent <ben.w.trent@gmail.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Library Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Library Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Dynamic;
using JanusApi.Model;


namespace JanusApi
{
  public partial class JanusRestClient : IDisposable
  {
    public long SessionToken { get; private set; }
    private static Random random = new Random();
    public string BaseUrl { get; private set; }
    private JanusClient _client;
    private bool keep_alive;
    private string api_secret;
    private static readonly object janus_core_lock_obj = new object();
    private static readonly object thread_monitor = new object();
    private DynamicDelayExecute delay_timeout;
    private const int timeout_time = 20;
   
    /// <summary>
    /// Constructs the Rest client and prepares it to start the connection.
    /// You must initialize the connection if you want to start it.
    /// By default, it will not keepAlive and will de-initialize the connection after 30 seconds if no command has been sent for that time.
    /// If it is supposed to keep alive, it will send keepAlive requests to the gateway every 30 seconds if no command has been sent in that time period.
    /// </summary>
    /// <param name="baseUrl">The base url for the Janus gateway, ex: http://192.168.0.123:8088/janus </param>
    /// <param name="keepAlive">Whether to keep the connection alive or not. If no command is sent through the janus base or to any attached plugin for 30 seconds, then it will close</param>
    public JanusRestClient(string baseUrl, bool keepAlive = false, string apiSecret = null)
    {
      BaseUrl = baseUrl;
      keep_alive = keepAlive;
      _client = JanusInterfaceFactory.GetNewJanusClient(baseUrl);
      delay_timeout = new DynamicDelayExecute(timeout_time);
      delay_timeout.OnDelayExhausted += new EventHandler(OnTimeOutFired);
      api_secret = apiSecret;
    }

    public void Dispose()
    {
      keep_alive = false;
      delay_timeout.Immediate();
    }

    private void OnTimeOutFired(object obj, EventArgs e)
    {
      lock (janus_core_lock_obj)
      {
        if (keep_alive)
        {
          _client.Execute<JanusBaseResponse>(new { janus = "keepalive", transaction = GetNewRandomTransaction() }, JanusRequestType.KeepAlive);
          delay_timeout.ResetDelay(timeout_time);
        }
        else
        {
          InternalCleanUp();
        }
      }
    }

    /// <summary>
    /// Determines if the client is initialized or not. We need to have a sessiontoken from the janus gateway root to do anything.
    /// </summary>
    /// <returns>true if it is initialized, false otherwise</returns>
    public bool IsRestClientInitialized()
    {
      lock (janus_core_lock_obj)
      {
        return SessionToken > 0;
      }
    }

    /// <summary>
    /// Tries to initializes the connection with the gateway.
    /// We need a session token given from the gateway for us to send any commands to it.
    /// </summary>
    /// <returns>True on success, false on failure</returns>
    public bool InitializeConnection()
    {
      bool retVal = false;
      lock (janus_core_lock_obj)
      {
        if (SessionToken == 0)
        {
          dynamic obj = new ExpandoObject();
          if (!String.IsNullOrWhiteSpace(api_secret)) obj.apisecret = api_secret;
          obj.janus = "create";
          obj.transaction = GetNewRandomTransaction();
          JanusBaseResponse resp = _client.Execute<JanusBaseResponse>(obj, JanusRequestType.Create);
          if (resp == null || resp.janus != "success")
          {
            retVal = false;
          }
          else
          {
            SessionToken = _client.GetSessionHandle();
            delay_timeout.Start();
            retVal = true;
          }
        }
      }
      return retVal;
    }

    /// <summary>
    /// This will lazily release all our connections to the janus gateway.
    /// And will disconnect from the janus gateway all together.
    /// </summary>
    public void CleanUp()
    {
      keep_alive = false;
      delay_timeout.Immediate();
    }

    private void InternalCleanUp()
    {
      Console.WriteLine("Starting Internal Cleanup");

      DeinitializeConnection();
      Console.WriteLine("Finished Internal Clean up, should now be reinitialized");
    }

    private void DeinitializeConnection()
    {
      dynamic msg = new ExpandoObject();
      msg.janus = "destroy";
      msg.transaction = GetNewRandomTransaction();
      if (!String.IsNullOrWhiteSpace(api_secret)) msg.apisecret = api_secret;
      _client.Execute<JanusBaseResponse>(msg, JanusRequestType.Destroy);
      _client.ClearConnectionInfo();
      SessionToken = 0;
    }

    /// <summary>
    /// Creates a new random alphanumeric string.
    /// This is to keep track of rest requests that are asynchronous, if any are made.
    /// Generally, most of the requests made in this format will be synchronous.
    /// </summary>
    /// <returns>alphanumeric string</returns>
    public string GetNewRandomTransaction()
    {
      var chars = "abcdefghijklmnopqrstuvwxyz1234567890";
      var result = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
      return result;
    }

    /// <summary>
    /// Execute a REST request against the gateway
    /// </summary>
    /// <typeparam name="T">The type of object to create and return with the response data</typeparam>
    /// <param name="request">The RestRequest to make against the gateway(assumes that the connection is intialized</param>
    /// <returns>The populated response</returns>
    public T Execute<T>(dynamic request, JanusRequestType type) where T : new()
    {
      if(type != JanusRequestType.Destroy)
        delay_timeout.ResetDelay(timeout_time);
      var response = _client.Execute<T>(request, type);
      return response.Data;
    }

    public T Execute<T>(dynamic request, JanusRequestType type, JanusPluginType plugin) where T : new()
    {
      delay_timeout.ResetDelay(timeout_time);
      var response = _client.Execute<T>(request, type, plugin);
      return response;
    }
  }
}
