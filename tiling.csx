#r "nuget: OpenCvSharp4, 4.10.0.20241108"
#r "nuget: OpenCvSharp4.runtime.win, 4.10.0.20241108"
#r "nuget: OpenCvSharp4.Extensions, 4.10.0.20241108"
#load "public.csx"

using System.Text.Json;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Extensions;

var width = 256;
var height = 256;

if(!File.Exists("mapboundary.json"))
{
    throw new Exception("Please run boundary.csx first to generate mapboundary.json");
}


string boundaryJson = File.ReadAllText("mapboundary.json");
var boundary = JsonSerializer.Deserialize<MapBoundary>(boundaryJson);

var totalWidth = (boundary.PositiveX + Math.Abs(boundary.NegativeX) + 1) * width;
var totalHeight = (boundary.PositiveY + Math.Abs(boundary.NegativeY) + 1) * height;

Console.WriteLine($"Total width: {totalWidth}, Total height: {totalHeight}");

var bigmap = new Mat(totalHeight, totalWidth, MatType.CV_8UC3, Scalar.Black);

for(int x = boundary.NegativeX; x <= boundary.PositiveX; x++)
{
    for(int y = boundary.NegativeY; y <= boundary.PositiveY; y++)
    {
        var tile = new Mat($"Map/{x}_{y}.png");
        Console.WriteLine($"Processing {x}_{y}.png");
        var roi = bigmap[new Rect((x + Math.Abs(boundary.NegativeX)) * width, bigmap.Rows - (y + Math.Abs(boundary.NegativeY)+1) * height, width, height)];
        tile.CopyTo(roi);
        tile.Dispose();
    }

}

Cv2.ImWrite($"bigmap-{MAP_VERSION}.png", bigmap);

bigmap.Dispose();

Console.WriteLine("Done");