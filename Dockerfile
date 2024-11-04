# Usa la imagen base del SDK de .NET para el paso de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Instalar las herramientas necesarias para la instalación de fuentes
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    fontconfig \
    wget \
    && apt-get clean

# Descargar e instalar Calibri
RUN wget -O /usr/share/fonts/truetype/calibri.ttf https://example.com/path/to/calibri.ttf && \
    fc-cache -f -v

COPY Fonts /app/Fonts

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

