# Usa la imagen base del SDK de .NET para el paso de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Instalar fontconfig para manejar las fuentes
RUN apt-get update && apt-get install -y fontconfig && apt-get clean

# Copiar las fuentes al contenedor en las rutas estándar de Linux
COPY ./fonts/*.ttf /usr/share/fonts/truetype/
COPY ./fonts/*.ttf /usr/local/share/fonts/

# Actualizar la caché de fuentes para que el sistema reconozca las fuentes copiadas
RUN fc-cache -f -v

# Copia el archivo .csproj y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto de los archivos del proyecto
COPY . ./

# Publica el proyecto
RUN dotnet publish -c Release -o /app/publish

# Usa la imagen base del ASP.NET para el runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expone el puerto 80
EXPOSE 80
ENTRYPOINT ["dotnet", "PescaSystem.dll"]


