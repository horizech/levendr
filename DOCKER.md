## Docker

- To build Docker image, first make sure to run buid ClientApp because right now, we are not building ClientApp in DockerFile
  ```
  cd ClientApp/
  npm run build
  ```
- Build docker image
  ```
  docker build -t horizech/levendr .
  ```
- Add tags to docker image
  ```
  docker image tag horizech/levendr horizech/levendr:latest
  docker image tag horizech/levendr horizech/levendr:0.1.3
  ```
- Push docker image with all tags to docker hub
  ```
  docker push horizech/levendr -a
  ```
- Set up environment variables by using .env file or mentioning in Dockerfile
- Run docker container using docker-compose
  ```
  docker-compose up -d
  ```
