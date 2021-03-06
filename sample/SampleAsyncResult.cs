﻿using System;
using System.IO;
using System.Net;
using CookComputing.AsynchronousProgrammingModel;

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