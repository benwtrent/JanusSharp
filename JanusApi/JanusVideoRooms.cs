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
    public JanusVideoRoomListObject ListRooms()
    {
      JanusVideoRoomListObject obj = new JanusVideoRoomListObject();
      obj.body = new JanusVideoRoomBody();
      obj.body.request = "list";
      obj.janus = JanusRequestType.Message;
      if (api_secret.HasValue()) obj.apisecret = api_secret;
      obj.transaction = GetNewRandomTransaction();
      JanusVideoRoomListObject response = _client.Execute(obj, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
      return response;
    }

    /// <summary>
    /// Returns true if the given room id exists and is created already in the janus gateway 
    /// video server plugin.
    /// </summary>
    /// <param name="room_id">The room id to with which to check existance</param>
    /// <returns>True if exists, false otherwise</returns>
    public JanusVideoRoomExistsObject RoomExists(int room_id)
    {
      JanusVideoRoomExistsObject obj = new JanusVideoRoomExistsObject();
      obj.body = new JanusVideoRoomBody();
      obj.body.request = "exists";
      obj.body.room = room_id;
      if (api_secret.HasValue()) obj.apisecret = api_secret;
      obj.janus = JanusRequestType.Message;
      obj.transaction = GetNewRandomTransaction();
      JanusVideoRoomExistsObject response = _client.Execute(obj, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
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
    public JanusVideoRoomCreationObject CreateRoom(int roomid, string _description = null, string _secret = null, int _bitrate = 0, string _publishers = null, bool _record = false, string _rec_dir = null, int _fir_freq = 0, bool _private = false)
    {
      JanusVideoRoomCreationObject obj = new JanusVideoRoomCreationObject();
      obj.body = new JanusVideoRoomCreationBody();
      obj.body.request = "create";
      obj.body.room = roomid;
      obj.body.record = _record;
      obj.body.is_private = _private;
      if (_bitrate > 0) obj.body.bitrate = _bitrate;
      if (_fir_freq > 0) obj.body.fir_freq = _fir_freq;
      if (_description.HasValue()) obj.body.description = _description;
      if (_secret.HasValue()) obj.body.secret = _secret;
      if (_rec_dir.HasValue()) obj.body.rec_dir = _rec_dir;
      if (api_secret.HasValue()) obj.apisecret = api_secret;
      obj.janus = JanusRequestType.Message;
      obj.transaction = GetNewRandomTransaction();
      JanusVideoRoomCreationObject response = _client.Execute(obj, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
      return response;
    }

    public JanusVideoRoomStreamRequestObject RequestStream(int roomid, string host, int port_base)
    {
      JanusVideoRoomStreamRequestObject obj = new JanusVideoRoomStreamRequestObject();
      obj.body = new JanusVideoRoomStreamRequestBody();
      obj.body.room = roomid;
      obj.body.host = host;
      obj.body.port = port_base;
      obj.janus = JanusRequestType.Message;
      obj.transaction = GetNewRandomTransaction();
      if (api_secret.HasValue()) obj.apisecret = api_secret;
      JanusVideoRoomStreamRequestObject response = _client.Execute(obj, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
      return response;
    }


    /// <summary>
    /// This will try and remove a given room by the id. If no room is found, the response will contain an error code.
    /// </summary>
    /// <param name="room_id">The room id to remove</param>
    /// <returns>Verify success by checking error code</returns>
    public JanusVideoRoomDestroyObject RemoveRoom(int room_id, string _secret = null)
    {
      JanusVideoRoomDestroyObject obj = new JanusVideoRoomDestroyObject();
      obj.body = new JanusVideoRoomBody();
      obj.body.request = "destroy";
      obj.body.room = room_id;
      if (_secret.HasValue()) obj.body.secret = _secret;
      obj.janus = JanusRequestType.Message;
      obj.transaction = GetNewRandomTransaction();
      if (api_secret.HasValue()) obj.apisecret = api_secret;
      JanusVideoRoomDestroyObject response = _client.Execute(obj, JanusRequestType.Message, JanusPluginType.JanusVideoRoom);
      return response;
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
      JanusBaseObject msg = new JanusBaseObject();
      string transaction = GetNewRandomTransaction();
      msg.janus = JanusRequestType.Attach;
      msg.plugin = JanusPluginType.JanusVideoRoom;
      msg.transaction = GetNewRandomTransaction();
      if (api_secret.HasValue()) msg.apisecret = api_secret;
      JanusBaseObject resp = _client.Execute(msg, JanusRequestType.Attach, JanusPluginType.JanusVideoRoom);
      return resp.janus == JanusRequestType.Success;
    }

    public void DeinitializeVideoRoomConnection()
    {
      JanusBaseObject msg = new JanusBaseObject();
      msg.janus = JanusRequestType.Detach;
      msg.transaction = GetNewRandomTransaction();
      if (api_secret.HasValue()) msg.apisecret = api_secret;
      _client.Execute(msg, JanusRequestType.Detach, JanusPluginType.JanusVideoRoom);
      JanusVideoRoomPluginHandle = 0;
    }
    //TODO determine if we want to support the participant function calls...
  }
}
