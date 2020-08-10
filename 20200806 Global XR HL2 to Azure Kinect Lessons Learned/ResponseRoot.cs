using System;
using System.Collections.Generic;

public class ResponseRoot
{
    public DateTime created { get; set; }
    public string id { get; set; }
    public string iteration { get; set; }
    public List<Prediction> predictions { get; set; }
    public string project { get; set; }
}
