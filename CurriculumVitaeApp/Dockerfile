# Imagen base para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos los archivos del proyecto
COPY *.csproj .
RUN dotnet restore

# Copiamos todo el código
COPY . .
RUN dotnet publish -c Release -o out

# Imagen final para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Exponemos el puerto
EXPOSE 8080

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "CurriculumVitaeApp.dll"]
