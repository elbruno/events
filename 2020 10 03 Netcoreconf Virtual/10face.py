import os
import cv2
import time

# init camera
camera = cv2.VideoCapture(1)
time.sleep(1)

face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml') 

while True:

    # Init and FPS process
    start_time = time.time()

    # Grab a single frame of video
    ret, frame = camera.read()

    gray  = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, 1.3, 5)

    for (top, right, bottom, left) in faces:
        # add frame for faces
        cv2.rectangle(frame,(top,right),(top+bottom,right+left),(0,0,255),2)

    # calculate FPS >> FPS = 1 / time to process loop
    fpsInfo = "FPS: " + str(1.0 / (time.time() - start_time)) 
    print(fpsInfo)
    cv2.putText(frame, fpsInfo, (10, 10), cv2.FONT_HERSHEY_SIMPLEX, 0.4, (255, 255, 255), 1)

    # Display the resulting image
    cv2.imshow('Deteccion de rostros', frame)

    # Hit 'q' on the keyboard to quit!
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release handle to the webcam
camera.release()
cv2.destroyAllWindows()