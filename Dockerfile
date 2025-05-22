# syntax=docker/dockerfile:1
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

# Copy the solution file first
COPY *.sln .

# Create the necessary directory structure
RUN mkdir -p src/MerchStore.Domain \
    src/MerchStore.Application \
    src/MerchStore.Infrastructure \
    src/MerchStore.WebUI \
    infra/ReviewApiFunction \
    infra/ReviewApiClient \
    MerchStoreTest/Infrastructure/MerchStore.IntegrationTests

# Copy all project files
COPY src/MerchStore.Domain/*.csproj ./src/MerchStore.Domain/
COPY src/MerchStore.Application/*.csproj ./src/MerchStore.Application/
COPY src/MerchStore.Infrastructure/*.csproj ./src/MerchStore.Infrastructure/
COPY src/MerchStore.WebUI/*.csproj ./src/MerchStore.WebUI/

# Copy the infra project files if they exist
COPY Infra/ReviewApiFunction/*.csproj ./Infra/ReviewApiFunction/
COPY Infra/ReviewApiClient/*.csproj ./Infra/ReviewApiClient/

# Copy test project files if they exist
COPY IntegrationTestFolder/MerchStore.IntegrationTests/IntegrationTests.csproj/ ./IntegrationTestFolder/MerchStore.IntegrationTests/

# Restore NuGet packages
ARG TARGETARCH
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore -a ${TARGETARCH/amd64/x64} || \
    # If restore fails, try with only the main projects that we know exist
    (dotnet new sln --name TempSolution && \
     dotnet sln TempSolution.sln add $(find src -name "*.csproj") && \
     dotnet restore -a ${TARGETARCH/amd64/x64} TempSolution.sln)

# Copy the remaining source code
COPY . .

# Build and publish the WebUI application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish src/MerchStore.WebUI -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["dotnet", "MerchStore.WebUI.dll"]