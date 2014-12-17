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

using RestSharp;
using RestSharp.Extensions;
using RestSharp.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Dynamic;
using JanusApi.Model;
namespace JanusApi
{
  public partial class JanusRestClient
  {
    public long JanusVideoRoomPluginHandle { get; private set; }
    private static readonly object video_room_lock_obj = new object();
    private MTSafeRefCounter janus_video_plugin_ref = new MTSafeRefCounter();
    /// <summary>
    /// This is part of the synchronous responses that the video room plugin provides
    /// </summary>
    /// <returns>A class containing the list of video rooms</returns>
    public JanusVideoRoomListResponse ListRooms()
    {
      if (janus_video_plugin_ref.IncRef())
      {
        if (!IsRestClientInitialized())
        {
          var resp = new JanusVideoRoomListResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomListPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomListPluginDataInternal
              {
                videoroom = "none",
                list = null,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Initialize the API client first"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        if (!InitializeVideoRoomConnection())
        {
          var resp = new JanusVideoRoomListResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomListPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomListPluginDataInternal
              {
                videoroom = "none",
                list = null,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Could not attach the plugin"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        var room_request = new RestRequest(Method.POST);
        room_request.Resource = "{SessionToken}/" + JanusVideoRoomPluginHandle;
        room_request.RequestFormat = DataFormat.Json;
        dynamic obj = new ExpandoObject();
        obj.request = "list";
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        room_request.AddBody(msg);
        JanusVideoRoomListResponse response = Execute<JanusVideoRoomListResponse>(room_request);
        if (response != null)
          delay_timeout.ResetDelay(29);
        janus_video_plugin_ref.DecRef();
        return response;
      }
      return null;
    }

    /// <summary>
    /// Returns true if the given room id exists and is created already in the janus gateway 
    /// video server plugin.
    /// </summary>
    /// <param name="room_id">The room id to with which to check existance</param>
    /// <returns>True if exists, false otherwise</returns>
    public JanusVideoRoomExistsResponse RoomExists(int room_id)
    {
      if (janus_video_plugin_ref.IncRef())
      {
        if (!IsRestClientInitialized())
        {
          var resp = new JanusVideoRoomExistsResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomExistsPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomExistsPluginDataInternal
              {
                videoroom = "none",
                exists = false,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Initialize the API client first"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        if (!InitializeVideoRoomConnection())
        {
          var resp = new JanusVideoRoomExistsResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomExistsPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomExistsPluginDataInternal
              {
                videoroom = "none",
                exists = false,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Could not attach the plugin"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        var room_request = new RestRequest(Method.POST);
        room_request.Resource = "{SessionToken}/" + JanusVideoRoomPluginHandle;
        room_request.RequestFormat = DataFormat.Json;
        dynamic obj = new ExpandoObject();
        obj.request = "exists";
        obj.room = room_id;
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        room_request.AddBody(msg);
        JanusVideoRoomExistsResponse response = Execute<JanusVideoRoomExistsResponse>(room_request);
        janus_video_plugin_ref.DecRef();
        return response;
      }
      return null;
    }


    /// <summary>
    /// Creates a room against the janus video room plugin.
    /// Will automatically attach the plugin if it is not already attached.
    /// However, it will not initialize the default session
    /// </summary>
    /// <param name="roomid">Mandatory unique room id to create</param>
    /// <param name="_description">Optional, room description</param>
    /// <param name="_secret">Optional, room secret for room access/modifications</param>
    /// <param name="_bitrate">Optional, bitrate what the streams will be</param>
    /// <param name="_publishers">Optional, the default number of publishers allowed</param>
    /// <param name="_record">Optional, whether to record the room or not</param>
    /// <param name="_rec_dir">Optional, the recording directory</param>
    /// <param name="_fir_freq">Optional, the frequency of FIR requests for the room...also sends PLI requests at the same time</param>
    /// <param name="_private">Optional, whether the room is private or not...defaults to not</param>
    /// <returns>Janus room response object. Will contain errors if not successful</returns>
    public JanusVideoRoomResponse CreateRoom(int roomid, string _description = null, string _secret = null, int _bitrate = 0, string _publishers = null, bool _record = false, string _rec_dir = null, int _fir_freq = 0, bool _private = false)
    {
      if (janus_video_plugin_ref.IncRef())
      {
        if (!IsRestClientInitialized())
        {
          var resp = new JanusVideoRoomResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomPluginDataInternal
              {
                videoroom = "none",
                room = 0,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Initialize the API client first"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        if (!InitializeVideoRoomConnection())
        {
          var resp = new JanusVideoRoomResponse
          {
            janus = "failure",
            plugindata = new JanusVideoRoomPluginData
            {
              plugin = "janus.plugin.videoroom",
              data = new JanusVideoRoomPluginDataInternal
              {
                videoroom = "none",
                room = 0,
                error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_NO_SESSION,
                error = "Could not attach the plugin"
              }
            }
          };
          janus_video_plugin_ref.DecRef();
          return resp;
        }
        var room_request = new RestRequest(Method.POST);
        room_request.Resource = "{SessionToken}/" + JanusVideoRoomPluginHandle;
        room_request.RequestFormat = DataFormat.Json;
        dynamic obj = new ExpandoObject();
        obj.request = "create";
        obj.room = roomid;
        obj.record = _record;
        obj.is_private = _private;
        if (_bitrate > 0) obj.bitrate = _bitrate;
        if (_fir_freq > 0) obj.fir_freq = _fir_freq;
        if (_description.HasValue()) obj.description = _description;
        if (_secret.HasValue()) obj.secret = _secret;
        if (_rec_dir.HasValue()) obj.rec_dir = _rec_dir;
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        room_request.AddBody(msg);
        JanusVideoRoomResponse response = Execute<JanusVideoRoomResponse>(room_request);
        if (response != null && response.plugindata.data.room > 0)
          delay_timeout.ResetDelay(29);
        janus_video_plugin_ref.DecRef();
        return response;
      }
      return JanusRoomSessionShuttingDownError();
    }

    /// <summary>
    /// This will try and remove a given room by the id. If no room is found, the response will contain an error code.
    /// </summary>
    /// <param name="room_id">The room id to remove</param>
    /// <returns>Verify success by checking error code</returns>
    public JanusVideoRoomResponse RemoveRoom(int room_id, string _secret = null)
    {
      if (janus_video_plugin_ref.IncRef())
      {
        if (InitializeVideoRoomConnection())
        {
          if (JanusVideoRoomPluginHandle > 0)
          {
            dynamic obj = new ExpandoObject();
            obj.request = "destroy";
            obj.room = room_id;
            if (_secret.HasValue()) obj.secret = _secret;
            dynamic msg = new ExpandoObject();
            msg.janus = "message";
            msg.transaction = GetNewRandomTransaction();
            if (api_secret.HasValue()) msg.apisecret = api_secret;
            msg.body = obj;
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.Resource = "{SessionToken}/" + JanusVideoRoomPluginHandle;
            request.AddBody(msg);
            JanusVideoRoomResponse response = Execute<JanusVideoRoomResponse>(request);
            if (response != null && response.plugindata.data.room > 0)
              delay_timeout.ResetDelay(29);
            janus_video_plugin_ref.DecRef();
            return response;
          }
        }
        else
        {
          janus_video_plugin_ref.DecRef();
        }
      }
      return JanusRoomSessionShuttingDownError();
    }

    /// <summary>
    /// This initializes the connection to the video room plugin.
    /// This must be called first so that the plugin can be attached to the janus server session already created.
    /// </summary>
    /// <returns></returns>
    public bool InitializeVideoRoomConnection()
    {
      if (IsRestClientInitialized() && janus_video_plugin_ref.IncRef())
      {
        bool retVal = true;
        lock (video_room_lock_obj)
        {
          if (JanusVideoRoomPluginHandle == 0)
          {
            RestRequest request = new RestRequest(Method.POST);
            request.Resource = "{SessionToken}";
            string transaction = GetNewRandomTransaction();
            request.RequestFormat = DataFormat.Json;
            dynamic msg = new ExpandoObject();
            msg.janus = "attach";
            msg.plugin = "janus.plugin.videoroom";
            msg.transaction = GetNewRandomTransaction();
            if (api_secret.HasValue()) msg.apisecret = api_secret;
            request.AddBody(msg);
            JanusBaseResponse resp = Execute<JanusBaseResponse>(request);

            if (resp == (null) || resp.janus == "error")
            {
              retVal = false;
            }
            else
            {
              JanusVideoRoomPluginHandle = resp.data.id;
              delay_timeout.ResetDelay(29);
              retVal = true;
            }
          }
        }
        janus_video_plugin_ref.DecRef();
        return retVal;
      }
      return false;
    }

    public void DeinitializeVideoRoomConnection()
    {
      janus_video_plugin_ref.BlockIncrease();
      //wait for all the other synchronous calls to finish if we are trying to send them
      while (janus_video_plugin_ref.ReferenceCount > 0)
        Thread.Sleep(250);
      lock (video_room_lock_obj)
      {
        if (IsRestClientInitialized() && JanusVideoRoomPluginHandle > 0)
        {
          RestRequest request = new RestRequest(Method.POST);
          request.Resource = "{SessionToken}/" + JanusVideoRoomPluginHandle;
          request.RequestFormat = DataFormat.Json;
          dynamic msg = new ExpandoObject();
          msg.janus = "detach";
          msg.transaction = GetNewRandomTransaction();
          if (api_secret.HasValue()) msg.apisecret = api_secret;
          request.AddBody(msg);
          JanusVideoRoomPluginHandle = 0;
          Execute<JanusBaseResponse>(request);
        }
        janus_video_plugin_ref.UnblockIncrease();
      }
    }

    private JanusVideoRoomResponse JanusRoomSessionShuttingDownError()
    {
      var error_resp = new JanusVideoRoomResponse
      {
        janus = "failure",
        plugindata = new JanusVideoRoomPluginData
        {
          plugin = "janus.plugin.videoroom",
          data = new JanusVideoRoomPluginDataInternal
          {
            videoroom = "none",
            room = 0,
            error_code = (int)JanusRoomErrorCodes.JANUS_VIDEOROOM_ERROR_UNKNOWN_ERROR,
            error = "We seem to be in the middle of shutting down"
          }
        }
      };
      return error_resp;
    }

    //TODO determine if we want to support the participant function calls...
  }
}
