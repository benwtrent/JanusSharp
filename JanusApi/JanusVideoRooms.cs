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
    private readonly object video_room_lock_obj = new object();
    public long JanusVideoRoomPluginHandle { get; private set; }
    /// <summary>
    /// This is part of the synchronous responses that the video room plugin provides
    /// </summary>
    /// <returns>A class containing the list of video rooms</returns>
    public JanusVideoRoomListResponse ListRooms()
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
          return resp;
        }
        if (!IsVideoRoomInitialized())
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
          return resp;
        }
        dynamic obj = new ExpandoObject();
        obj.request = "list";
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        JanusVideoRoomListResponse response = Execute<JanusVideoRoomListResponse>(msg, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
        return response;
    }

    /// <summary>
    /// Returns true if the given room id exists and is created already in the janus gateway 
    /// video server plugin.
    /// </summary>
    /// <param name="room_id">The room id to with which to check existance</param>
    /// <returns>True if exists, false otherwise</returns>
    public JanusVideoRoomExistsResponse RoomExists(int room_id)
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
          return resp;
        }
        if (!IsVideoRoomInitialized())
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
          return resp;
        }
        dynamic obj = new ExpandoObject();
        obj.request = "exists";
        obj.room = room_id;
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        JanusVideoRoomExistsResponse response = Execute<JanusVideoRoomExistsResponse>(msg, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
        return response;
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
          return resp;
        }
        if (!IsVideoRoomInitialized())
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
          return resp;
        }
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
        JanusVideoRoomResponse response = Execute<JanusVideoRoomResponse>(msg, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
        return response;
    }

    public JanusVideoRoomResponse RequestStream(int roomid, string host, int port_base)
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
          return resp;
        }
        if (!IsVideoRoomInitialized())
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
          return resp;
        }
        dynamic obj = new ExpandoObject();
        obj.request = "anonymous_listen";
        obj.room = roomid;
        obj.host = host;
        obj.port = port_base;
        dynamic msg = new ExpandoObject();
        if (api_secret.HasValue()) msg.apisecret = api_secret;
        msg.janus = "message";
        msg.transaction = GetNewRandomTransaction();
        msg.body = obj;
        JanusVideoRoomResponse response = Execute<JanusVideoRoomResponse>(msg, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
        return response;
    }


    /// <summary>
    /// This will try and remove a given room by the id. If no room is found, the response will contain an error code.
    /// </summary>
    /// <param name="room_id">The room id to remove</param>
    /// <returns>Verify success by checking error code</returns>
    public JanusVideoRoomResponse RemoveRoom(int room_id, string _secret = null)
    {
      if (IsVideoRoomInitialized())
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
            JanusVideoRoomResponse response = Execute<JanusVideoRoomResponse>(msg, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
            return response;
      }
       return JanusRoomSessionShuttingDownError();
    }

    public bool IsVideoRoomInitialized()
    {
      lock (video_room_lock_obj)
      {
        return IsRestClientInitialized() && JanusVideoRoomPluginHandle > 0;
      }
    }

    /// <summary>
    /// This initializes the connection to the video room plugin.
    /// This must be called first so that the plugin can be attached to the janus server session already created.
    /// </summary>
    /// <returns></returns>
    public bool InitializeVideoRoomConnection()
    {
      if (IsRestClientInitialized())
      {
        bool retVal = true;
        lock (video_room_lock_obj)
        {
          if (JanusVideoRoomPluginHandle == 0)
          {
            string transaction = GetNewRandomTransaction();
            dynamic msg = new ExpandoObject();
            msg.janus = "attach";
            msg.plugin = "janus.plugin.videoroom";
            msg.transaction = GetNewRandomTransaction();
            if (api_secret.HasValue()) msg.apisecret = api_secret;
            JanusBaseResponse resp = Execute<JanusBaseResponse>(msg, JanusRequestType.Attach, JanusPluginType.JanusVideoRoom);

            if (resp == (null) || resp.janus == "error")
            {
              retVal = false;
            }
            else
            {
              JanusVideoRoomPluginHandle = resp.data.id;
              retVal = true;
            }
          }
        }
        return retVal;
      }
      return false;
    }

    public void DeinitializeVideoRoomConnection()
    {
      //wait for all the other synchronous calls to finish if we are trying to send them
      lock (video_room_lock_obj)
      {
        if (IsRestClientInitialized() && JanusVideoRoomPluginHandle > 0)
        {
          dynamic msg = new ExpandoObject();
          msg.janus = "detach";
          msg.transaction = GetNewRandomTransaction();
          if (api_secret.HasValue()) msg.apisecret = api_secret;
          Execute<JanusBaseResponse>(msg, JanusRequestType.Detach, JanusPluginType.JanusVideoRoom);
          JanusVideoRoomPluginHandle = 0;
        }
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
