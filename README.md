# A Traefik ForwardAuth AuthServer implementation in .NET

## Running Locally

### Generate certs

Add .pem and .key files to `./local-dev/traefik/certs`. (everything in the folder will be gitignored)

If you don't have a cert, you can generate one using .NET with the following command:

```bash
dotnet dev-certs https --trust --export-path ./local-dev/traefik/certs/dev-cert.pem --verbose --format PEM
```

> Note - This will also trust the cert on the local machine
