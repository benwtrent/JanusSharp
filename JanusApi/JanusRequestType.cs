using System;
using System.Text;

namespace JanusApi
{
  public class TypeSafeEnum
  {
    public String Type
    {
      get;
      private set;
    }

    protected TypeSafeEnum(String type)
    {
      Type = type;
    }

    public override string ToString()
    {
      return Type;
    }

    public override bool Equals(object obj)
    {
      var myobj = obj as JanusRequestType;
      if (myobj != null)
        return myobj.Type == this.Type;
      else
        return false;
    }
  }

  public sealed class JanusPluginType : TypeSafeEnum
  {
    public static readonly JanusPluginType None = new JanusPluginType("none");
    public static readonly JanusPluginType JanusAudioBridge = new JanusPluginType("janus.plugin.audiobridge");
    public static readonly JanusPluginType JanusEchoTest = new JanusPluginType("janus.plugin.echotest");
    public static readonly JanusPluginType JanusRecordPlay = new JanusPluginType("janus.plugin.recordplay");
    public static readonly JanusPluginType JanusSip = new JanusPluginType("janus.plugin.sip");
    public static readonly JanusPluginType JanusStreaming = new JanusPluginType("janus.plugin.streaming");
    public static readonly JanusPluginType JanusVideoCall = new JanusPluginType("janus.plugin.videocall");
    public static readonly JanusPluginType JanusVideoRoom = new JanusPluginType("janus.plugin.videoroom");
    public static readonly JanusPluginType JanusVoiceMail = new JanusPluginType("janus.plugin.voicemail");

    protected JanusPluginType(String type) : base(type) { }
  }

  public sealed class JanusRequestType : TypeSafeEnum
  {
    public static readonly JanusRequestType Create = new JanusRequestType("create");
    public static readonly JanusRequestType Attach = new JanusRequestType("attach");
    public static readonly JanusRequestType Destroy = new JanusRequestType("destroy");
    public static readonly JanusRequestType Detach = new JanusRequestType("detach");
    public static readonly JanusRequestType Message = new JanusRequestType("message");
    public static readonly JanusRequestType KeepAlive = new JanusRequestType("keepalive");
   
    protected JanusRequestType(String type) : base(type) { }
  }
}
