FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["James/James.csproj", "James/"]
RUN dotnet restore "James/James.csproj"
COPY . .
WORKDIR "/src/James"
RUN dotnet build "James.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "James.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "James.dll"]

ENV Discord__Bot__Token=""
