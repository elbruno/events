using System;
using Windows.ApplicationModel.Core;
using Urho;

namespace Urho08.HoloCognitiveServicesFlow
{
    internal class Program
    {
        [MTAThread]
        static void Main() => CoreApplication.Run(new UrhoAppViewSource<HoloCsFlowApp>());
    }
}