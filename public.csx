public class MapBoundary {
    public int ZoomLevel { get; set; }
    public int PositiveX { get; set; }
    public int NegativeX { get; set; }
    public int PositiveY { get; set; }
    public int NegativeY { get; set; }

    public override string ToString() {
        return $"ZoomLevel: {ZoomLevel}, PositiveX: {PositiveX}, NegativeX: {NegativeX}, PositiveY: {PositiveY}, NegativeY: {NegativeY}";
    }
}

public string MAP_VERSION = "5.2-3";