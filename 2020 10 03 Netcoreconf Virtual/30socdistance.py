import os
import cv2
import time
from yoloanalyzer import yoloV3_analyzer
from socialdistanceanalyzer import social_distance_analyzer

# init camera
camera = cv2.VideoCapture("video2 640x360.mp4")
time.sleep(1)

# init Analyzer with confidence 50%
sda = social_distance_analyzer()

while True:

    # Init and FPS process
    start_time = time.time()

    # Grab a single frame of video
    ret, frame = camera.read()

    frame, socialDistanceOk = sda.ImageProcess(frame)

    # calculate FPS >> FPS = 1 / time to process loop
    info = str(f"Social Distance: {socialDistanceOk} - FPS: " + str(1.0 / (time.time() - start_time)))
    print(info)

    # Display the resulting image
    cv2.imshow('Soc Distance demo !', frame)

    # Hit 'q' on the keyboard to quit!
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release handle to the webcam
camera.release()
cv2.destroyAllWindows()