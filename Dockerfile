FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Thang.IDP/Thang.IDP.csproj", "Thang.IDP/"]
RUN dotnet restore "Thang.IDP/Thang.IDP.csproj"
COPY . .
WORKDIR "/src/Thang.IDP"
RUN dotnet build "Thang.IDP.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Thang.IDP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add the following lines to copy the certificate
RUN mkdir -p /app/certificates
COPY Thang.IDP/certificates/aspnetapp.pfx /app/certificates

ENTRYPOINT ["dotnet", "Thang.IDP.dll"]