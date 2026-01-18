# Application-name

Application description.

## Navigation

- [Company homepage](https://google.com)
- [Documentation](https://google.com)


## Deployment

### Tech stack

- **Framework** powers the REST API and other web pages.

### Requirements

- **Framework** 12+

The repository includes deployment configurations for **Docker and docker-compose** as well as **other platforms**. For Helm charts, reference the [documentation](https://google.com).

## Development

### Docker

For production hosting and deployment with **Docker**, use [ref to the production Dockerfile and docker-compose file].

For local development, install and launch [Docker], and run:

```shell
docker compose -f .devcontainer/docker-compose.yaml up -d
```

### Dev Containers

Within IDEs that support the [Development Containers] specification, start the container from the editor. The necessary `docker compose` commands to build and setup the container should run automatically. For **Visual Studio Code** this requires installing the [Dev Container extension].

[Dev Container extension]: https://containers.dev/supporting#dev-containers
[Development Containers]: https://containers.dev/supporting
[Docker]: https://docs.docker.com