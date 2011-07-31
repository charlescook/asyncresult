using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;


class Program
{
  static void Main(string[] args)
  {
    try
    {
      var client = new SampleWebClient();
      IAsyncResult iasr = client.BeginGet("http://localhost",
          ar =>
          {
            Console.WriteLine();
          }, 
          null);
      string page = client.EndGet(iasr);
      Console.WriteLine(page);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }
  }
}

