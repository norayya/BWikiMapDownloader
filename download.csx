#load "public.csx"

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Threading;

static async Task DownloadTile(HttpClient client, string map_version, int x, int y, int zoomLevel, SemaphoreSlim semaphore)
{
    var uri = $"https://wiki-dev-patch-oss.oss-cn-hangzhou.aliyuncs.com/res/ys/map-{map_version}/{zoomLevel}/tile-{x}_{y}.png";
    var response = await client.GetAsync(uri);
    if(response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync($"Map/{x}_{y}.png", content);
        Console.WriteLine($"Downloaded {x}_{y}.png");
    } else {
        Console.WriteLine($"Failed to download {x}_{y}.png");
    }

    semaphore.Release();

}

var httpClient = new HttpClient(new HttpClientHandler(){
    MaxConnectionsPerServer = 20
});
static SemaphoreSlim semaphore = new SemaphoreSlim(20);

if(!File.Exists("mapboundary.json"))
{
    throw new Exception("Please run boundary.csx first to generate mapboundary.json");
}

if(!Directory.Exists("Map"))
{
    Directory.CreateDirectory("Map");
}

string boundaryJson = File.ReadAllText("mapboundary.json");
var boundary = JsonSerializer.Deserialize<MapBoundary>(boundaryJson);

var downloadTasks = new List<Task>();

for(int x = boundary.NegativeX; x <= boundary.PositiveX; x++)
{
    for(int y = boundary.NegativeY; y <= boundary.PositiveY; y++)
    {
        await semaphore.WaitAsync();
        downloadTasks.Add(DownloadTile(httpClient, MAP_VERSION, x, y, boundary.ZoomLevel, semaphore));
    }
}

await Task.WhenAll(downloadTasks);

Console.WriteLine("download completed");

httpClient.Dispose();