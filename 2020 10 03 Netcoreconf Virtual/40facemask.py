import os
import cv2
import time
from facemaskanalyzer import face_mask_analyzer

# init camera
camera = cv2.VideoCapture(1)
time.sleep(1)

# init Analyzer with confidence 50%
fma = face_mask_analyzer(0.5) 

while True:

    # Init and FPS process
    start_time = time.time()

    # Grab a single frame of video
    ret, frame = camera.read()

    frame = fma.ImageProcess(frame)

    # calculate FPS >> FPS = 1 / time to process loop
    fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) 
    print(fpsInfo)
    cv2.putText(frame, fpsInfo, (10, 10), cv2.FONT_HERSHEY_SIMPLEX, 0.4, (255, 255, 255), 1)

    # Display the resulting image
    cv2.imshow('DNC Face Mask !', frame)

    # Hit 'q' on the keyboard to quit!
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release handle to the webcam
camera.release()
cv2.destroyAllWindows()