🗄️ Instalación de la base de datos

Sigue estos pasos para crear y configurar la base de datos requerida por la aplicación:

1. Crear la base de datos

Crea manualmente una base de datos llamada CurriculumBD en tu servidor SQL Server (puedes hacerlo desde SQL Server Management Studio o cualquier cliente SQL).

2. Configurar la cadena de conexión

En el archivo appsettings.json, edita la sección ConnectionStrings y reemplaza los valores de servidor, usuario y contraseña por los tuyos:

"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=CurriculumBD;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
}

3. Abrir la Consola de Administración de Paquetes

En Visual Studio, ve a:

Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes

4. Ejecutar las migraciones

En la consola, ejecuta los siguientes comandos:

Add-Migration NombreDeLaMigracion
Update-Database

Esto creará las tablas en la base de datos CurriculumBD según tu modelo.