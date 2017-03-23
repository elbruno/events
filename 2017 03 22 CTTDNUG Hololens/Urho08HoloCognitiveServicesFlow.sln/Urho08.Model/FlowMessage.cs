using System;

namespace Urho08.Model
{
    public class FlowMessage
    {
        public FlowMessage()
        {
            HoloTimeStamp = DateTime.Now.ToString("O");
            HoloDescription = string.Empty;
            HoloImage = string.Empty;
            HoloReadText = string.Empty;
            FaceCount = "0";
            CategoriesCount = "0";
            Categories = "0";
            Faces = string.Empty;
            Tags = string.Empty;
            Exception = string.Empty;
        }
        public string HoloDescription { get; set; }
        public string HoloImage { get; set; }
        public string HoloTimeStamp { get; set; }
        public string HoloReadText { get; set; }
        public string FaceCount { get; set; }
        public string CategoriesCount { get; set; }
        public string TagCount { get; set; }
        public string Categories { get; set; }
        public string Faces { get; set; }
        public string Tags { get; set; }
        public string Exception { get; set; }
    }
}
