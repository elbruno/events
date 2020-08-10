public class Prediction
{
    public BoundingBox boundingBox { get; set; }
    public double probability { get; set; }
    public int tagId { get; set; }
    public string tagName { get; set; }
}