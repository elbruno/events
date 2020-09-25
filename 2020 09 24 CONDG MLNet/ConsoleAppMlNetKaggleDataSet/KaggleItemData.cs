using Microsoft.ML.Data;

namespace ConsoleAppMlNetKaggleDataSet
{
    public class KaggleItemData
    {
        [LoadColumn(0)]
        public string itemIndex;

        [LoadColumn(3)]
        public float itemValue;
    }
}
