---
page_type: sample
languages:
- csharp
products:
- azure
description: "This sample application demonstrates how to take a model exported from the Custom Vision Service in the ONNX format and add it to an application for real-time image classification."
urlFragment: cognitive-services-onnx12-customvision-sample
---

# Sample application for ONNX1.2 models exported from Custom Vision Service
This sample application demonstrates how to take a model exported from the [Custom Vision Service](https://www.customvision.ai) in the ONNX format and add it to an application for real-time image classification. 

## Getting Started

### Prerequisites
- [Windows SDK - Build 17738+ (latest)](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK)
- [Visual Studio 17](https://www.visualstudio.com/vs/preview/)
- [Windows 10 Insider Preview 17738+](https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewiso)
- An account at [Custom Vision Service](https://www.customvision.ai) 

### Quickstart
1. Clone the repository and open the project in Visual Studio
2. Build and run the sample Application
3. Application comes with two models already included along with sample images to test.

### Adding your own sample model of your own classifier
The models provided with the sample recognizes Hemlock/Japanese Cherry images. To add  your own model exported from the [Custom Vision Service](https://www.customvision.ai) do the following, and then build and launch the application:
  1. [Create and train](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/getting-started-build-a-classifier) a classifer with the Custom Vision Service. You must choose a "compact" domain such as **General (compact)** to be able to export your classifier. If you have an existing classifier you want to export instead, convert the domain in "settings" by clicking on the gear icon at the top right. In setting, choose a "compact" model, Save, and Train your project.  
  2. [Export your model](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/export-your-model) by going to the Performance tab. Select an iteration trained with a compact domain, an "Export" button will appear. Click on *Export* then *ONNX* then *ONNX1.2* then *Export.* Click the *Download* button when it appears. A *.onnx file will download.
  3. Drop your *model.onnx file into your project's Assets folder. 
  4. Under Solutions Explorer/ Assets Folder add model file to project by selecting Add Existing Item.
  5. Change properties of model just added: "Build Action" -> "Content"  and  "Copy to Output Directory" -> "Copy if newer"
  6. Add to list variable "onnxFileNames" name of model just added along with number of lables model contains.
  7. Build and run.
  8. Click button to select image to evaluate.

### Things to note
- The test image should larger than 227 x 227


## Resources
- Link to [ONNX](https://onnx.ai/)
- Link to [ONNX on GitHub](https://github.com/onnx/onnx)
- Link to [Get started with Windows Machine Learning](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/get-started)
- Link to [More Information about ONNX version and Windows Machine Learning](https://github.com/Microsoft/Windows-Machine-Learning)
- Link to [Custom Vision Service Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/home)
