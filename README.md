# BoschSmartHomeSharp
C# wrapper class for the Bosch SmartHome local REST API (https://github.com/BoschSmartHome/bosch-shc-api-docs)

(work in progress - nuget package available: https://www.nuget.org/packages/BoschSmartHomeSharp/)

requires self-signed client certificate to talk with the Bosch smart home controller. When using the default "ApiClient" constructor, the library expects the PFX to be provided as file "client_pfx.PFX" (which should be present in the executables directory) and protected with a password (the ApiClient constructor expects '12345' by default). There are also ohter ApiClient constructors available if you want to pass in your own custom certificate file path and/or password. The certificate as string, and the password of your smart home controller encoded as base64, are both needed in order to register to the controller as new client app (and to create trust to your client cert).

register new app:
```C#
BoschSmartHomeSharp.ApiClient apiclient;
(...)
string certFile = Path.Join(Directory.GetCurrentDirectory(), "client_pfx.pfx");
apiclient = new BoschSmartHomeSharp.ApiClient("192.168.0.10", certFile, "myCertPwd123");
(...)
private void registerClient() // register new client
{
    string clientCert = "\"-----BEGIN CERTIFICATE-----\\rMIID(...)A==\\r-----END CERTIFICATE-----\"";
    bool result = apiclient.registerDevice("c29tZXRoaW5nMTIzIQ==", clientCert, "myNewBoschClientApp");
    if (!result)
        Debug.WriteLine("registration not successful");
    else
        Debug.WriteLine("registration successful - yay!! :)");
}



```

get devices:
```C#
List<device> allDevices = apiclient.getDevices();
```

subscribe to messages:
```C#
string pollId = apiclient.subscribeLongPoll();
IEnumerable<JToken> messages = apiclient.longPoll(pollId);
foreach (JToken msg in messages)
                {
                    Debug.WriteLine(msg.ToString());
                }
client.unsubscribeLongPoll(pollId);
```

