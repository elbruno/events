import json

jsonStr = '{"created":"2020-04-06T22:39:49.979443","id":"","iteration":"","predictions":[{"boundingBox":{"height":0.27972369,"left":0.20783035,"top":0.40126584,"width":0.27212879},"probability":0.91361207,"tagId":0,"tagName":"MVP"},{"boundingBox":{"height":0.31118299,"left":0.59710174,"top":0.38750677,"width":0.2969852},"probability":0.66253799,"tagId":0,"tagName":"MVP"},{"boundingBox":{"height":0.28672652,"left":0.58867771,"top":-0.02662121,"width":0.32969777},"probability":0.2496209,"tagId":0,"tagName":"MVP"},{"boundingBox":{"height":0.11953794,"left":0.67877732,"top":0.47256263,"width":0.12360345},"probability":0.10147022,"tagId":0,"tagName":"MVP"}],"project":""}'

jsonObj = json.loads(jsonStr)
preds = jsonObj['predictions']
sorted_preds = sorted(preds, key=lambda x: x['probability'], reverse=True)
strSortedPreds = ""
resultFound = False
if (sorted_preds):
    for pred in sorted_preds:
        # tag name and prob * 100
        tagName     = str(pred['tagName'])
        probability = pred['probability'] * 100
        # apply threshold
        if (probability >= 50):
            bb = pred['boundingBox']
            height = bb['height'] * 100
            left = bb['left'] * 100
            top = bb['top'] * 100
            width = bb['width'] * 100

            # draw bounding boxes
            print(f'height = {height} - left {left} - top {top} - width {width}')
        
