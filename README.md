# Sistema de Gestión de Comprobantes Electrónicos

Sistema completo para la gestión de comprobantes electrónicos (Facturas y Boletas) según normativa SUNAT, desarrollado con .NET 8 y React 18.

## 🚀 Tecnologías Utilizadas

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core 8** - ORM
- **PostgreSQL 18** - Base de datos
- **MediatR** - Patrón CQRS
- **FluentValidation** - Validaciones
- **Serilog** - Logging estructurado
- **Swagger/OpenAPI** - Documentación de API

### Frontend
- **React 18** - Framework UI
- **Vite** - Build tool
- **Axios** - Cliente HTTP
- **Lucide React** - Iconos
- **CSS3** - Estilos

## 📋 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL 18](https://www.postgresql.org/download/) (puerto 5432)
- Git

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/reto-fullstack-contasiscorp.git
cd reto-fullstack-contasiscorp
```

### 2. Configurar la Base de Datos

Asegúrate de que PostgreSQL esté corriendo en el puerto 5432 con:
- **Usuario:** postgres
- **Contraseña:** 123
- **Base de datos:** ComprobantesDb (se crea automáticamente)

### 3. Configurar y Ejecutar el Backend
```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar las migraciones (automático al iniciar)
cd Comprobantes.Api
dotnet run
```

La API estará disponible en:
- **HTTPS:** https://localhost:7151
- **Swagger:** https://localhost:7151/swagger

### 4. Configurar y Ejecutar el Frontend
```bash
# Ir a la carpeta del frontend
cd web

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev
```

El frontend estará disponible en:
- **Local:** http://localhost:3000


Servicios disponibles:
- **API:** http://localhost:5000
- **PostgreSQL:** localhost:5432
- **Frontend:** http://localhost:3000

## 📁 Estructura del Proyecto
```
reto-fullstack-contasiscorp/
├── Comprobantes.Api/              # API Web
│   ├── Controllers/               # Controladores REST
│   ├── Middleware/                # Middleware de errores
│   └── Program.cs                 # Configuración principal
├── Comprobantes.Application/      # Lógica de aplicación
│   ├── Commands/                  # Comandos CQRS
│   ├── Queries/                   # Consultas CQRS
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Validators/                # Validaciones FluentValidation
│   └── Behaviors/                 # Behaviors de MediatR
├── Comprobantes.Domain/           # Dominio
│   ├── Entities/                  # Entidades del dominio
│   ├── Enums/                     # Enumeraciones
│   └── Exceptions/                # Excepciones personalizadas
├── Comprobantes.Infrastructure/   # Infraestructura
│   ├── Data/                      # DbContext y configuraciones
│   └── Repositories/              # Implementación de repositorios
├── Comprobantes.Tests/            # Tests unitarios
│   ├── Domain/                    # Tests del dominio
│   └── Validators/                # Tests de validaciones
├── web/                           # Frontend React
│   ├── src/
│   │   ├── components/            # Componentes React
│   │   ├── services/              # Servicios API
│   │   ├── styles/                # Estilos CSS
│   │   └── utils/                 # Utilidades
│   └── package.json
├── docker-compose.yml             # Configuración Docker
└── README.md                      # Este archivo
```

## 🔌 API Endpoints

### Comprobantes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/comprobantes` | Listar comprobantes con filtros y paginación |
| `GET` | `/api/comprobantes/{id}` | Obtener detalle de un comprobante |
| `POST` | `/api/comprobantes` | Crear un nuevo comprobante |
| `PUT` | `/api/comprobantes/{id}/anular` | Anular un comprobante |

### Filtros disponibles (GET)
- `page`: Número de página (default: 1)
- `pageSize`: Tamaño de página (default: 10, máximo: 50)
- `tipo`: Factura | Boleta
- `estado`: Emitido | Anulado
- `fechaDesde`: Fecha desde (formato: yyyy-MM-dd)
- `fechaHasta`: Fecha hasta (formato: yyyy-MM-dd)
- `rucReceptor`: RUC del receptor (11 dígitos)

## 📝 Ejemplos de Uso

### Crear una Factura
```bash
POST https://localhost:7151/api/comprobantes
Content-Type: application/json

{
  "tipo": "Factura",
  "serie": "F001",
  "rucEmisor": "20123456789",
  "razonSocialEmisor": "Mi Empresa S.A.C.",
  "rucReceptor": "20987654321",
  "razonSocialReceptor": "Cliente S.A.",
  "items": [
    {
      "descripcion": "Servicio de consultoría",
      "cantidad": 1,
      "precioUnitario": 1000.00
    },
    {
      "descripcion": "Horas adicionales",
      "cantidad": 2.5,
      "precioUnitario": 150.00
    }
  ]
}
```

### Crear una Boleta (sin receptor)
```bash
POST https://localhost:7151/api/comprobantes
Content-Type: application/json

{
  "tipo": "Boleta",
  "serie": "B001",
  "rucEmisor": "20123456789",
  "razonSocialEmisor": "Mi Tienda SAC",
  "items": [
    {
      "descripcion": "Producto A",
      "cantidad": 3,
      "precioUnitario": 25.50
    }
  ]
}
```

## ✅ Tests
```bash
# Ejecutar todos los tests
dotnet test

```

### Tests Implementados
- ✅ Validación de RUC (11 dígitos numéricos)
- ✅ Validación de formato de serie (F### para Facturas, B### para Boletas)
- ✅ Cálculo de IGV (18% del subtotal)
- ✅ Cálculo de totales
- ✅ Anulación de comprobantes
- ✅ Validación de items (cantidad y precio > 0)

## 🎯 Funcionalidades Principales

### Backend
- ✅ CRUD completo de comprobantes
- ✅ Validaciones de negocio según SUNAT
- ✅ Cálculo automático de subtotales, IGV y total
- ✅ Numeración automática por serie
- ✅ Paginación y filtros avanzados
- ✅ Manejo de errores con ProblemDetails (RFC 7807)
- ✅ Logging estructurado con Serilog
- ✅ Documentación Swagger/OpenAPI
- ✅ Arquitectura limpia (Clean Architecture)

### Frontend
- ✅ Lista de comprobantes con filtros
- ✅ Creación de comprobantes (Facturas y Boletas)
- ✅ Visualización de detalle
- ✅ Anulación de comprobantes
- ✅ Cálculo automático de totales en tiempo real
- ✅ Manejo dinámico de items
- ✅ Interfaz responsive
- ✅ Validaciones en el cliente

## 🔒 Reglas de Negocio

1. **RUC:** Debe tener exactamente 11 dígitos numéricos
2. **IGV:** Se calcula como el 18% del subtotal
3. **Total:** SubTotal + IGV
4. **SubTotal:** Suma de todos los `Items.Subtotal`
5. **Serie:** 
   - Facturas: Formato `F###` (ej: F001, F002)
   - Boletas: Formato `B###` (ej: B001, B002)
6. **Anulación:** No se puede anular un comprobante ya anulado
7. **Número:** Autoincremental por serie, persistido en BD
8. **Receptor en Boletas:** Opcional (consumidor final)
9. **Receptor en Facturas:** Obligatorio

## 🐛 Solución de Problemas

### La API no se conecta a PostgreSQL
```bash
# Verificar que PostgreSQL está corriendo
sudo systemctl status postgresql

# Verificar la conexión
psql -h localhost -p 5432 -U postgres
```

### El frontend no se conecta a la API
- Verificar que el backend esté corriendo en https://localhost:7151
- Revisar la configuración de proxy en `web/vite.config.js`
- Verificar CORS en el backend

### Error de migraciones
```bash
# Eliminar migraciones existentes
dotnet ef migrations remove --project Comprobantes.Infrastructure --startup-project Comprobantes.Api

# Crear nuevas migraciones
dotnet ef migrations add InitialCreate --project Comprobantes.Infrastructure --startup-project Comprobantes.Api
```

## 📚 Documentación Adicional

- **Swagger UI:** https://localhost:7151/swagger
- **Logs:** `Comprobantes.Api/logs/`

## 👨‍💻 Autor

Jordy Ortiz Arce

## 📄 Licencia

Este proyecto es de uso  para evaluación técnica.