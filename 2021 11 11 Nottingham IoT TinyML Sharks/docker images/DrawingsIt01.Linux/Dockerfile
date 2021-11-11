FROM python:3.7-slim

RUN pip install -U pip
RUN pip install --no-cache-dir numpy~=1.17.5 tensorflow~=2.0.2 flask~=1.1.2 pillow~=7.2.0
RUN pip install --no-cache-dir mscviplib==2.200731.16

COPY app /app

# Expose the port
EXPOSE 80

# Set the working directory
WORKDIR /app

# Run the flask server for the endpoints
CMD python -u app.py