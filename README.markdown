asyncresult
===

asyncresult provides a couple of classes to implement IAsyncResult. It is mostly derived
from the code presented in 
[How to implement the IAsyncResult design pattern][howtoimplement]
by Niko Schuessler, which in turn was based on the code in the MSDN article
[ Implementing the CLR Asynchronous Programming Model][msdnarticle]

Example
---

```c#
using System;
using System.IO;
using System.Net;
using CookComputing.AsynchronousProgrammingModel;

public class SampleWebClient
{
  public IAsyncResult BeginGet(
      string uri,
      AsyncCallback asyncCallback,
      object state)
  {
    var result = new SampleAsyncResult(new Uri(uri), asyncCallback, 
      state, this, "get");
    result.Process();
    return result;
  }

  public string EndGet(IAsyncResult result)
  {
    return AsyncResult<string>.End(result, this, "get");
  }
}

class SampleAsyncResult : AsyncResult<string>
{
  Uri _uri;
  WebRequest _webRequest;
  WebResponse _webResponse;
  Stream _responseStream;
  byte[] _buffer = new byte[4096];
  Stream _outputStream;

  public SampleAsyncResult(Uri uri,
    AsyncCallback asyncCallback, object state, object owner, string id)
    : base(asyncCallback, state, owner, id)
  {
    _uri = uri;
  }

  protected override void ProcessImpl()
  {
    _webRequest = WebRequest.Create(_uri);
    _webRequest.BeginGetResponse(Try(GetResponseCallback), null);
  }

  void GetResponseCallback(IAsyncResult asyncResult)
  {
    _outputStream = new MemoryStream();
    _webResponse = _webRequest.GetResponse();
    _responseStream = _webResponse.GetResponseStream();
    _responseStream.BeginRead(_buffer, 0, _buffer.Length, 
      Try(ReadCallback), null);
  }

  void ReadCallback(IAsyncResult asyncResult)
  {
    int count = _responseStream.EndRead(asyncResult);
    if (count > 0)
      _outputStream.Write(_buffer, 0, count);
    if (count == 0)
    {
      _outputStream.Position = 0;
      SetResult(new StreamReader(_outputStream).ReadToEnd());
      Complete(null, false);
      return;
    }
    _responseStream.BeginRead(_buffer, 0, _buffer.Length, 
      Try(ReadCallback), null);
  }

  protected override void Completing(Exception ex, 
    bool completedSynchronously)
  {
    if (_responseStream != null)
      _responseStream.Close();
    if (_webResponse != null)
      _webResponse.Close();
  }
}
```

[howtoimplement]: http://blogs.msdn.com/b/nikos/archive/2011/03/14/how-to-implement-iasyncresult-in-another-way.aspx
[msdnarticle]: http://msdn.microsoft.com/en-us/magazine/cc163467.aspx
