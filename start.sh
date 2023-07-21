docker-compose \
    -f ./local-dev/docker-compose.yaml \
    -f ./local-dev/traefik/docker-compose.traefik.yaml \
    -f ./local-dev/exampleservice/docker-compose.exampleservice.yaml \
    -f ./local-dev/authserver/docker-compose.authserver.yaml \
    up --build