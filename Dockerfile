FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
COPY ["*.csproj", "/app/jumbotravel/"]
WORKDIR /app/jumbotravel
RUN dotnet restore

COPY . ./
RUN dotnet publish -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
ENV ASPNETCORE_URLS http://+:5000
WORKDIR /app
COPY --from=build /app/jumbotravel/out .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["./JumboTravel.Api"]