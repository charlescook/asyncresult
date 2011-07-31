using System;
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

