# Etapa 1: Compilación
# Usamos la imagen del SDK de .NET 8 para tener todas las herramientas de construcción.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Proyect_api.csproj", "."]
RUN dotnet restore "./Proyect_api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Proyect_api.csproj" -c Release -o /app/build

# Etapa 2: Publicación
# Tomamos los archivos compilados y los preparamos para producción.
FROM build AS publish
RUN dotnet publish "Proyect_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Imagen Final y Ligera
# Usamos una imagen más pequeña que solo tiene lo necesario para ejecutar la API.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Proyect_api.dll"]