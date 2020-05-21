# Bruno Capuano 2020
# display the camera feed using OpenCV

import time
import cv2

dsize = (640, 480)

video_capture = cv2.VideoCapture(0)
time.sleep(2.0)

while True:
    ret, frameOrig = video_capture.read()
    frame = cv2.resize(frameOrig, dsize)
   
    cv2.imshow('Video', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

video_capture.release()
cv2.destroyAllWindows()