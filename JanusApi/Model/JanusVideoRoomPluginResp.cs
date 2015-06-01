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

namespace JanusApi.Model
{
  public class JanusVideoRoomBody : JanusPluginBody
  {
    public string request { get; set; }
    public long? room { get; set; }
    public string secret { get; set; }
  }
  
  public class JanusVideoRoom
  {
    /// <summary>
    /// Room id, if 0 a random one will be generated
    /// </summary>
    public int room { get; set; }
    /// <summary>
    /// the description of the room
    /// </summary>
    public string description { get; set; }
    /// <summary>
    /// Room secret if it exists
    /// </summary>
    public string secret { get; set; }
    /// <summary>
    /// Room bitrate
    /// </summary>
    public int bitrate { get; set; }
    /// <summary>
    /// Frequency if fir rtcp requests.
    /// </summary>
    public int fir_freq { get; set; }
    /// <summary>
    /// the number of publishers allowed in the room
    /// </summary>
    public int publishers { get; set; }
    /// <summary>
    /// whether to record the room or not
    /// </summary>
    public bool record { get; set; }
    /// <summary>
    /// The recording directory for the recorded rtp dumps
    /// </summary>
    public string rec_dir { get; set; }

    /// <summary>
    /// The participants in the room.
    /// </summary>
    public List<JanusVideoRoomParticipant> participants { get; set; }

    /// <summary>
    /// The number of participants...this is for when the gateway does not return
    /// an actual list of participant information
    /// </summary>
    public int num_participants { get; set; }

    /// <summary>
    /// How many listeners there are in the room. 
    /// Listeners are anonymous unless they become full participants
    /// </summary>
    public int listeners { get; set; }

    /// <summary>
    /// Whether or not the room is private
    /// </summary>
    public bool is_private { get; set; }

  }

  public class JanusVideoRoomParticipant
  {
    public UInt64 user_id { get; set; }
    public string display { get; set; }
    public bool audio_active { get; set; }
    public bool video_active { get; set; }
    public bool firefox { get; set; }
    public UInt64 bitrate { get; set; }
    public bool recording_active { get; set; }
    public string recording_base { get; set; }
  }

  public enum JanusVideoRoomParticipantType
  {
    janus_videoroom_p_type_none = 0,
    janus_videoroom_p_type_subscriber,			/* Generic listener/subscriber */
    janus_videoroom_p_type_subscriber_muxed,	/* Multiplexed listener/subscriber */
    janus_videoroom_p_type_publisher	
  }

  public class JanusVideoRoomCreationBody
  {
    public string request { get { return "create"; } set { return; } }
    public long room { get; set; }
    public bool record { get; set; }
    public bool is_private { get; set; }
    public int bitrate { get; set; }
    public int fir_freq { get; set; }
    public string description { get; set; }
    public string secret { get; set; }
    public string rec_dir { get; set; }
  }

  public class JanusVideoRoomCreationObject : JanusPluginObject
  {
    public JanusVideoRoomCreationBody body { get; set; }
    public JanusVideoRoomPluginData plugindata { get; set; }
  }

  public class JanusVideoRoomDestroyObject : JanusPluginObject
  {
    public JanusVideoRoomBody body { get; set; }
    public JanusVideoRoomPluginData data { get; set; }
  }

  public class JanusVideoRoomStreamRequestBody
  {
    public string request { get { return "anonymous_listen"; } set { return; } }
    public long room { get; set; }
    public string host { get; set; }
    public int port { get; set; }
  }
  public class JanusVideoStreamRequestDataInternal
  {

  }

  public class JanusVideoStreamRequestData : JanusPluginData
  {
    JanusVideoStreamRequestDataInternal data { get; set; }
  }

  public class JanusVideoRoomStreamRequestObject : JanusPluginObject
  {
    public JanusVideoRoomStreamRequestBody body { get; set; }
    public JanusVideoStreamRequestData plugindata { get; set; }
  }

  public class JanusVideoRoomInfoObject : JanusPluginObject
  {
    public JanusVideoRoomInfoPluginData plugindata { get; set; }
  }

  public class JanusVideoRoomListObject : JanusPluginObject
  {
    public JanusVideoRoomBody body { get; set; }
    public JanusVideoRoomListPluginData plugindata { get; set; }
  }

  public class JanusVideoRoomExistsObject : JanusPluginObject
  {
    public JanusVideoRoomBody body { get; set; }
    public JanusVideoRoomExistsPluginData plugindata { get; set; }
  }

  public class JanusVideoRoomPluginData : JanusPluginData
  {
    public JanusVideoRoomPluginDataInternal data { get; set; }
  }

  public class JanusVideoRoomExistsPluginData : JanusPluginData
  {
    public JanusVideoRoomExistsPluginDataInternal data { get; set; }
  }

  public class JanusVideoRoomListPluginData : JanusPluginData
  {
    public JanusVideoRoomListPluginDataInternal data { get; set; }
  }

  public class JanusVideoRoomInfoPluginData : JanusPluginData
  {
    public JanusVideoRoomInfoPluginDataInternal data { get; set; }
  }

  public class JanusVideoRoomPluginDataInternal
  {
    public string videoroom { get; set; }
    public int room { get; set; }
    public int error_code { get; set; }
    public string error { get; set; }
  }

  public class JanusVideoRoomExistsPluginDataInternal : JanusVideoRoomPluginDataInternal
  {
    public bool exists { get; set; }
  }

  public class JanusVideoRoomInfoPluginDataInternal
  {
    public string videoroom { get; set; }
    public JanusVideoRoom room { get; set; }
    public int error_code { get; set; }
    public string error { get; set; }
  }

  public class JanusVideoRoomListPluginDataInternal
  {
    public string videoroom { get; set; }
    public List<JanusVideoRoom> list { get; set; }
    public int error_code { get; set; }
    public string error { get; set; }
  }

  public enum JanusRoomErrorCodes
  {
    JANUS_VIDEOROOM_ERROR_UNKNOWN_ERROR = 499,
    JANUS_VIDEOROOM_ERROR_NO_MESSAGE = 421,
    JANUS_VIDEOROOM_ERROR_INVALID_JSON = 422,
    JANUS_VIDEOROOM_ERROR_INVALID_REQUEST = 423,
    JANUS_VIDEOROOM_ERROR_JOIN_FIRST = 424,
    JANUS_VIDEOROOM_ERROR_ALREADY_JOINED = 425,
    JANUS_VIDEOROOM_ERROR_NO_SUCH_ROOM = 426,
    JANUS_VIDEOROOM_ERROR_ROOM_EXISTS = 427,
    JANUS_VIDEOROOM_ERROR_NO_SUCH_FEED = 428,
    JANUS_VIDEOROOM_ERROR_MISSING_ELEMENT = 429,
    JANUS_VIDEOROOM_ERROR_INVALID_ELEMENT = 430,
    JANUS_VIDEOROOM_ERROR_INVALID_SDP_TYPE = 431,
    JANUS_VIDEOROOM_ERROR_PUBLISHERS_FULL = 432,
    JANUS_VIDEOROOM_ERROR_UNAUTHORIZED = 433,
    JANUS_VIDEOROOM_ERROR_ALREADY_PUBLISHED = 434,
    JANUS_VIDEOROOM_ERROR_NOT_PUBLISHED = 435,
    JANUS_VIDEOROOM_ERROR_ID_EXISTS = 436,
    JANUS_VIDEOROOM_ERROR_NO_SESSION = 437,
    JANUS_VIDEOROOM_ERROR_NONE = 0
  }
}
