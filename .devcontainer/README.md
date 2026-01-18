# Dev Container Setup

## Dockerfile (To-do)
This Dockerfile uses a multi-stage build to allow for multiple types of builds without duplicating code and ensuring maximum consistency.

## Docker Compose (To-do)
This compose file includes the [] service(s), in addition to the [Application] dev container service. When running the Dev Container (i.e. via the "Clone Repository in Container" feature in VSCode), this compose file will be used to start the necessary services before starting the [Application] Dev Container itself.