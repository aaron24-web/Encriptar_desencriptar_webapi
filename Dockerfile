# Reemplaza 'NOMBRE_DEL_PROYECTO' en las siguientes líneas con el nombre real de tu archivo .csproj

# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Proyect_api.csproj", "."]
RUN dotnet restore "./Proyect_api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Proyect_api.csproj" -c Release -o /app/build

# Etapa 2: Publicación
FROM build AS publish
RUN dotnet publish "Proyect_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Imagen Final
# Reemplaza 'NOMBRE_DEL_PROYECTO.dll' con el nombre de tu dll de salida (usualmente el mismo que el .csproj)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Proyect_api.dll"]