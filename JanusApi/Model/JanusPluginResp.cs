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
using System.Linq;
using System.Text;

namespace JanusApi.Model
{
  public class JanusPluginResp
  {
    /// <summary>
    /// Constructs a new plugin.
    /// </summary>
    /// <param name="_handle">The handle on which to send commands</param>
    /// <param name="_type">The plugin type</param>
    public JanusPluginResp(int _handle, JanusPluginType _type)
    {
      handle = _handle;
      type = _type;
    }
    /// <summary>
    /// The handle of the plugin. 
    /// To send messages to this plugin send REST commands to: baseURL/SessionToken/handle
    /// </summary>
    public int handle { get; private set; }

    /// <summary>
    /// The plugin type.
    /// </summary>
    public JanusPluginType type { get; private set; }

  }

  public class JanusPluginResponse : JanusBaseResponse
  {
    public long session_id { get; set; }
    public long sender { get; set; }
  }

  public class JanusPluginData
  {
    public string plugin { get; set; }
  }

  /// <summary>
  /// Currently supported plugin types. Add plugin type here if you are adding a supported plugin.
  /// </summary>
  public enum JanusPluginType
  {
    AUDIOBRIDGE = 0,
    ECHOTEST = 1,
    RECORDPLAY = 2,
    SIP = 3,
    STREAMING = 4,
    VIDEOCALL = 5,
    VIDEOROOM = 6,
    VOICEMAIL = 7
  }
}
