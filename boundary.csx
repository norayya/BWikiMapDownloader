#load "public.csx"

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

var zoomLevel = 8;

async Task<int> FindBoundary(HttpClient client, bool isX, bool isPos, int n)
{
    int index = n;
    var uri = new Func<int, string>(i => isX switch {
        true => $"https://wiki-dev-patch-oss.oss-cn-hangzhou.aliyuncs.com/res/ys/map-{MAP_VERSION}/{zoomLevel}/tile-{(isPos ? i : -i)}_0.png",
        false => $"https://wiki-dev-patch-oss.oss-cn-hangzhou.aliyuncs.com/res/ys/map-{MAP_VERSION}/{zoomLevel}/tile-0_{(isPos ? i : -i)}.png"
    });

    var k = 0; 
    var j = 0; 
    var c = 1; 
    while(true) {
        var request = new HttpRequestMessage(HttpMethod.Head, uri(index));
        var response = await client.SendAsync(request);
        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Found tile at {index}");
            k = index;
            
        } else {
            Console.WriteLine($"Not found tile at {index}");
            j = index;
            break;
        }
        
        index = (int)Math.Floor(1 + Math.Pow(2, c));
        j=index;
        c++;
        await Task.Delay(200);
    } 

    while(true){
        if(j - k == 1 || j== k)
        {
            return k;
        }
        c = (int)Math.Floor((double)(j-k)/2);
        var f = j;
        j = j - c;

        var request = new HttpRequestMessage(HttpMethod.Head, uri(j));
        var response = await client.SendAsync(request);
        if(response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Found tile at {j}");
            k=j;
            j=f;
            
        } else {
            Console.WriteLine($"Not found tile at {j}");

        }

        await Task.Delay(200);
    }


}

var httpClient = new HttpClient();

var x_pos_max = await FindBoundary(httpClient,true,true,1);
var x_neg_max = await FindBoundary(httpClient,true,false,1);
var y_pos_max = await FindBoundary(httpClient,false,true,1);
var y_neg_max = await FindBoundary(httpClient,false,false,1);

Console.WriteLine($"x_positive_max: {x_pos_max}");
Console.WriteLine($"x_negative_max: {-x_neg_max}");
Console.WriteLine($"y_positive_max: {y_pos_max}");
Console.WriteLine($"y_negative_max: {-y_neg_max}");

Console.WriteLine("Done");

httpClient.Dispose();

File.WriteAllText("mapboundary.json", JsonSerializer.Serialize(new MapBoundary() { ZoomLevel = zoomLevel, PositiveX = x_pos_max, NegativeX = -x_neg_max, PositiveY = y_pos_max, NegativeY = -y_neg_max}));
