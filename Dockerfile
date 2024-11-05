# Usa la imagen base del SDK de .NET para el paso de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

RUN apt-get -y install fontconfig
COPY /fonts ~/.fonts
COPY /fonts /usr/shared/fonts
COPY /fonts /usr/share/fonts/truetype
# refresh system font cache
RUN fc-cache -f -v

# Actualizar la caché de fuentes para que el sistema reconozca Calibri
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


