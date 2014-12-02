### C#.Net Client Side API for the Janus Gateway ###

* Created for .Net 4.0 and includes the RestSharp.dll for that .Net version.
* [Janus Gateway API Docs](http://janus.conf.meetecho.com/docs/rest.html)
* [Janus Gateway Source Code](https://github.com/meetecho/janus-gateway)

### Designed Use ###

* This client side API is for server logic and not for client media exchange 
* Right now should only support synchronous calls to the Janus API

### Example Code ###
```
#!c#
    static void Main(string[] args)
    {
      JanusRestClient client = new JanusRestClient("http://192.168.0.195:8088/janus");
      client.InitializeConnection();
      client.InitializeVideoRoomConnection();
      client.CreateRoom(12233);

      client.RemoveRoom(12233);

      client.CleanUp();
    }
```
