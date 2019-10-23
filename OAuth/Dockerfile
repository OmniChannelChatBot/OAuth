FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["OAuth/OAuth.csproj", "OAuth/"]
RUN dotnet restore "OAuth/OAuth.csproj"
COPY . .
WORKDIR "/src/OAuth"
RUN dotnet build "OAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OAuth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OAuth.dll"]