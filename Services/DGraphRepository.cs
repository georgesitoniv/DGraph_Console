using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Api;
using Grpc.Net.Client;
using Microsoft.Extensions.FileProviders;
using static Api.Dgraph;

namespace dgraph_console {
  public class DGraphRepository {
    private readonly Dgraph.IDgraphClient _client;
    private readonly IFileProvider _fileProvider;

    public DGraphRepository () {
      _client = new DgraphClient(GrpcChannel.ForAddress("http://127.0.0.1:9080")) as Dgraph.IDgraphClient;
      _fileProvider = new EmbeddedFileProvider(Assembly.GetAssembly(typeof(DGraphRepository)), "dgraph_console.Schema");
    }

    public async Task LoadSchema() {
      var result = await _client.Alter(new Operation { Schema =  ReadEmbeddedFile("deltaVEntities.schema") });
      if (result.IsFailed) {
        Console.WriteLine("Failed to load schema.");
      }
    }

    private string ReadEmbeddedFile(string filename) {
      using(var stream = _fileProvider.GetFileInfo(filename).CreateReadStream()) {
          using(var reader = new StreamReader(stream, Encoding.UTF8)) {
              return reader.ReadToEnd();
          }
      }
    }
  }
}