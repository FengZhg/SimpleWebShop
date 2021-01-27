#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
RUN apt-get update
RUN apt-get install -y --no-install-recommends libgdiplus libc6-dev 
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["backEnd/backEnd.csproj", "backEnd/"]
RUN dotnet restore "backEnd/backEnd.csproj"
COPY . .
WORKDIR "/src/backEnd"
RUN dotnet build "backEnd.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "backEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "backEnd.dll"]