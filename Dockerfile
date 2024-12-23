# Usa la imagen base del SDK de .NET para el paso de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

RUN sed -i 's/^Components: main$/& contrib/' /etc/apt/sources.list.d/debian.sources
RUN apt-get update
RUN apt-get install -y ttf-mscorefonts-installer fontconfig
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


