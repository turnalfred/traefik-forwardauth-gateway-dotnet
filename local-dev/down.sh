docker-compose \
    -f ./docker-compose.yaml \
    -f ./traefik/docker-compose.traefik.yaml \
    -f ./redis/docker-compose.redis.yaml \
    -f ./exampleservice/docker-compose.exampleservice.yaml \
    -f ./exampleapp/docker-compose.exampleapp.yaml \
    -f ./authserver/docker-compose.authserver.yaml \
    down 