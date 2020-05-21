# Bruno Capuano 2020
# display the camera feed using OpenCV
# add a bottom image overlay, using a background image

import time
import cv2

dsize = (640, 480)

# load bottom img
background = cv2.imread('Bottom03.png')
background = cv2.resize(background, dsize)

video_capture = cv2.VideoCapture(0)
time.sleep(2.0)

while True:
    ret, frameOrig = video_capture.read()
    frame = cv2.resize(frameOrig, dsize)

    img = cv2.addWeighted(background, 1, frame, 1, 0)
   
    cv2.imshow('Video', img)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

video_capture.release()
cv2.destroyAllWindows()