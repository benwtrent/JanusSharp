using System;
using System.Text;
using Newtonsoft.Json;
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

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
  }
  [JsonConverter(typeof(TypeSafeEnumJsonConverter))]
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
    public JanusPluginType() : base("none")
    {  }
    public static JanusPluginType getFromString(String type)
    {
      type = type.ToLower();
      if (JanusPluginType.None.Equals(type))
        return JanusPluginType.None;
      if (JanusPluginType.JanusAudioBridge.Equals(type))
        return JanusPluginType.JanusAudioBridge;
      if (JanusPluginType.JanusEchoTest.Equals(type))
        return JanusPluginType.JanusEchoTest;
      if (JanusPluginType.JanusRecordPlay.Equals(type))
        return JanusPluginType.JanusRecordPlay;
      if (JanusPluginType.JanusSip.Equals(type))
        return JanusPluginType.JanusSip;
      if (JanusPluginType.JanusStreaming.Equals(type))
        return JanusPluginType.JanusStreaming;
      if (JanusPluginType.JanusVideoCall.Equals(type))
        return JanusPluginType.JanusVideoCall;
      if (JanusPluginType.JanusVideoRoom.Equals(type))
        return JanusPluginType.JanusVideoRoom;
      if (JanusPluginType.JanusVoiceMail.Equals(type))
        return JanusPluginType.JanusVoiceMail;
      throw new Exception("Invalid string for JanusPluginType" + type);
    }

    private JanusPluginType(String type) : base(type) { }

    public override bool Equals(object obj)
    {
        var myobj = obj as JanusPluginType;
        if (myobj != null)
            return myobj.Type == this.Type;
        var strObj = obj as String;
        if (strObj != null)
          return strObj == this.Type;
      return false;
    }
  }

  [JsonConverter(typeof(TypeSafeEnumJsonConverter))]
  public sealed class JanusRequestType : TypeSafeEnum
  {
    public static readonly JanusRequestType Message = new JanusRequestType("message");
    public static readonly JanusRequestType Trickle = new JanusRequestType("trickle");
    public static readonly JanusRequestType Detach = new JanusRequestType("detach");
    public static readonly JanusRequestType Destroy = new JanusRequestType("destroy");
    public static readonly JanusRequestType KeepAlive = new JanusRequestType("keepalive");
    public static readonly JanusRequestType Create = new JanusRequestType("create");
    public static readonly JanusRequestType Attach = new JanusRequestType("attach");
    public static readonly JanusRequestType Event = new JanusRequestType("event");
    public static readonly JanusRequestType Error = new JanusRequestType("error");
    public static readonly JanusRequestType Ack = new JanusRequestType("ack");
    public static readonly JanusRequestType WebRtcUp = new JanusRequestType("webrtcup");
    public static readonly JanusRequestType Success = new JanusRequestType("success");
    public static readonly JanusRequestType HangUp = new JanusRequestType("hangup");
    public static readonly JanusRequestType Detached = new JanusRequestType("detached");
    public static readonly JanusRequestType Media = new JanusRequestType("media");

    public JanusRequestType()
      : base("message")
    { }

    public static JanusRequestType getFromString(String type)
    {
      type = type.ToLower();
      if (JanusRequestType.Media.Equals(type))
        return JanusRequestType.Media;
      if (JanusRequestType.Ack.Equals(type))
        return JanusRequestType.Ack;
      if (JanusRequestType.Attach.Equals(type))
        return JanusRequestType.Attach;
      if (JanusRequestType.Create.Equals(type))
        return JanusRequestType.Create;
      if (JanusRequestType.Destroy.Equals(type))
        return JanusRequestType.Destroy;
      if (JanusRequestType.Detach.Equals(type))
        return JanusRequestType.Detach;
      if (JanusRequestType.Detached.Equals(type))
        return JanusRequestType.Detached;
      if (JanusRequestType.Error.Equals(type))
        return JanusRequestType.Error;
      if (JanusRequestType.Event.Equals(type))
        return JanusRequestType.Event;
      if (JanusRequestType.HangUp.Equals(type))
        return JanusRequestType.HangUp;
      if (JanusRequestType.KeepAlive.Equals(type))
        return JanusRequestType.KeepAlive;
      if (JanusRequestType.Message.Equals(type))
        return JanusRequestType.Message;
      if (JanusRequestType.Success.Equals(type))
        return JanusRequestType.Success;
      if (JanusRequestType.Trickle.Equals(type))
        return JanusRequestType.Trickle;
      if (JanusRequestType.WebRtcUp.Equals(type))
        return JanusRequestType.WebRtcUp;

      throw new Exception("Invalid string for JanusRequestType" + type);
    }

    private JanusRequestType(String type) : base(type) { }
    public override bool Equals(object obj)
    {
        var myobj = obj as JanusRequestType;
        if (myobj != null)
            return myobj.Type == this.Type;
        var strObj = obj as String;
        if (strObj != null)
          return strObj == this.Type;
        return false;
    }
  }

  class TypeSafeEnumJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return true;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(value.ToString());
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      string enumString = (string)reader.Value;
      if (objectType == typeof(JanusRequestType))
        return JanusRequestType.getFromString(enumString);
      else if (objectType == typeof(JanusPluginType))
        return JanusPluginType.getFromString(enumString);
      throw new JsonException("Could not parse typesafe enum of value: " + reader.Value.ToString());
    }

  }
}
