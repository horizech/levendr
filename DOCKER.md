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
- Set up environment variables by using .env file or mentioning in Dockerfile
- Run docker container using docker-compose
  ```
  docker-compose up -d
  ```
