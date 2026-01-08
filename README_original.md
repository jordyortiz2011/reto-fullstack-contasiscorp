# Reto Técnico: Sistema de Gestión de Comprobantes

## Información General

| Campo | Detalle |
|-------|---------|
| **Fecha** | Jueves 9 de enero de 2025 |
| **Duración** | 7 horas (9:00 AM - 4:00 PM, hora Perú UTC-5) |
| **Modalidad** | Desarrollo + Defensa de código (Loom) |
| **Stack** | C# .NET 8, PostgreSQL, React o Blazor |
| **Herramientas** | Puedes usar cualquier recurso (IA, documentación, librerías, etc.) |

---

## Contexto del Negocio

Eres parte del equipo de desarrollo de una empresa de software contable en Perú. Necesitas construir un microservicio para gestionar comprobantes electrónicos (facturas y boletas) que cumpla con las regulaciones de SUNAT.

---

## Requerimientos

### Backend - API REST

Construir una API con los siguientes endpoints:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/comprobantes` | Crear un nuevo comprobante |
| `GET` | `/api/comprobantes` | Listar comprobantes con paginación y filtros |
| `GET` | `/api/comprobantes/{id}` | Obtener detalle de un comprobante |
| `PUT` | `/api/comprobantes/{id}/anular` | Anular un comprobante |

### Modelo de Datos

```
Comprobante
├── Id (GUID)
├── Tipo (Factura | Boleta)
├── Serie (string, 4 caracteres)
├── Numero (int, autoincremental por serie, persistente en BD)
├── FechaEmision (DateTime, generada automáticamente al crear)
├── RucEmisor (string, 11 dígitos) [Obligatorio]
├── RazonSocialEmisor (string) [Obligatorio]
├── RucReceptor (string, 11 dígitos) [Obligatorio solo para Facturas, opcional para Boletas]
├── RazonSocialReceptor (string) [Obligatorio solo para Facturas]
├── SubTotal (decimal, calculado = suma de Items.Subtotal)
├── IGV (decimal, calculado = SubTotal * 0.18)
├── Total (decimal, calculado = SubTotal + IGV)
├── Estado (Emitido | Anulado, inicial siempre "Emitido")
└── Items[] [Mínimo 1 item requerido]
    ├── Descripcion (string) [Obligatorio]
    ├── Cantidad (decimal, permite fracciones ej: 0.5, 2.5)
    ├── PrecioUnitario (decimal) [Obligatorio]
    └── Subtotal (decimal, calculado = Cantidad * PrecioUnitario)
```

**Nota**: Los campos marcados como "calculado" no se reciben en el request, se calculan automáticamente.

### Reglas de Negocio

1. **RUC**: Debe tener exactamente 11 dígitos numéricos
2. **IGV**: Se calcula como el 18% del subtotal
3. **Total**: SubTotal + IGV
4. **SubTotal**: Suma de todos los `Items.Subtotal`
5. **Serie**: 
   - Facturas: Formato `F###` (ej: F001, F002)
   - Boletas: Formato `B###` (ej: B001, B002)
6. **Anulación**: No se puede anular un comprobante ya anulado
7. **Número**: Autoincremental por serie, debe persistir en base de datos (si existen F001-1, F001-2, el siguiente debe ser F001-3)
8. **Receptor en Boletas**: RucReceptor y RazonSocialReceptor son opcionales para Boletas (consumidor final)

### Ejemplos de Request/Response

**POST /api/comprobantes** - Crear comprobante:

```json
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

**Response 201 Created**:

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tipo": "Factura",
  "serie": "F001",
  "numero": 1,
  "fechaEmision": "2025-01-09T10:30:00Z",
  "rucEmisor": "20123456789",
  "razonSocialEmisor": "Mi Empresa S.A.C.",
  "rucReceptor": "20987654321",
  "razonSocialReceptor": "Cliente S.A.",
  "subTotal": 1375.00,
  "igv": 247.50,
  "total": 1622.50,
  "estado": "Emitido",
  "items": [
    {
      "descripcion": "Servicio de consultoría",
      "cantidad": 1,
      "precioUnitario": 1000.00,
      "subtotal": 1000.00
    },
    {
      "descripcion": "Horas adicionales",
      "cantidad": 2.5,
      "precioUnitario": 150.00,
      "subtotal": 375.00
    }
  ]
}
```

**Response 400 Bad Request** (ProblemDetails):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "detail": "Uno o más errores de validación ocurrieron.",
  "errors": {
    "rucEmisor": ["El RUC debe tener exactamente 11 dígitos numéricos"],
    "serie": ["El formato de serie para Factura debe ser F### (ej: F001)"]
  }
}
```

### Filtros para el Listado

- `fechaDesde` y `fechaHasta`: Rango de fechas de emisión
- `tipo`: Filtrar por Factura o Boleta
- `rucReceptor`: Búsqueda por RUC del cliente
- `estado`: Filtrar por Emitido o Anulado
- `page` y `pageSize`: Paginación (default: page=1, pageSize=10, máximo pageSize=50)

---

### Frontend

Construir una interfaz simple que **consuma la API REST** desarrollada:

1. **Listado de comprobantes**
   - Tabla con los datos principales
   - Filtros básicos (al menos por tipo y estado)
   - Paginación

2. **Formulario de creación**
   - Campos del comprobante
   - Agregar/eliminar items dinámicamente
   - Cálculo automático de subtotales, IGV y total

3. **Acción de anulación**
   - Botón en cada fila o en el detalle
   - Confirmación antes de anular

**Nota**: El frontend debe estar integrado con la API, no usar datos mock.

---

### Requerimientos Adicionales

- **Tests unitarios**: Mínimo un test por cada regla de negocio (validación RUC, cálculo IGV, formato serie, etc.)
- **Documentación Swagger/OpenAPI**: Todos los endpoints documentados con ejemplos de request/response
- **Manejo de errores con ProblemDetails (RFC 7807)**: Respuestas de error estructuradas y consistentes
- **Logging estructurado**: Loggear creación, anulación y errores de comprobantes (usar Serilog o similar)
- **Docker Compose funcional**: Debe levantar la API y PostgreSQL con un solo comando `docker-compose up`

---

## Configuración

1. **Crea un repositorio en tu cuenta personal de GitHub**
   - Nombre sugerido: `reto-fullstack-contasiscorp`
   - Puede ser público o privado
   - **Monorepo**: Backend y Frontend deben estar en el mismo repositorio

2. **Agrega como colaborador a**: `alejandro.xux`
   - Ve a Settings → Collaborators → Add people
   - Esto es **obligatorio** para que podamos revisar tu código

3. **Configura tu entorno de desarrollo como prefieras**
   - .NET 8 SDK
   - PostgreSQL 15+
   - Node.js 18+ y React 18+ (si usas React) o Blazor WebAssembly/.NET 8 (si usas Blazor)

4. **Librerías permitidas**: Puedes usar cualquier librería del ecosistema .NET (MediatR, AutoMapper, FluentValidation, etc.) y de React/Blazor

5. **Haz commits frecuentes** con mensajes descriptivos

---

## Estructura Sugerida del Repositorio

```
tu-repositorio/
├── README.md                   # Instrucciones para ejecutar tu solución
├── docker-compose.yml          # PostgreSQL y servicios necesarios
├── src/
│   ├── Api/
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   ├── Middleware/         # Manejo de errores (ProblemDetails)
│   │   └── Program.cs          # Configuración de Swagger y logging
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   └── Validators/
│   ├── Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Exceptions/
│   ├── Infrastructure/
│   │   ├── Data/
│   │   └── Repositories/
│   └── Web/
│       └── (React o Blazor)
└── tests/
    └── UnitTests/              # Tests de validaciones de negocio
```

---

## Criterios de Evaluación

| Criterio | Peso | Descripción |
|----------|------|-------------|
| **Funcionalidad** | 20% | Los endpoints funcionan correctamente |
| **Arquitectura** | 20% | Separación de responsabilidades, Clean Architecture |
| **Código C#** | 15% | Uso idiomático del lenguaje, async/await, manejo de errores |
| **Frontend** | 15% | Interfaz funcional y usable |
| **Requerimientos Adicionales** | 15% | Tests, Swagger, ProblemDetails, Logging, Docker Compose |
| **Video de defensa** | 15% | Claridad al explicar, dominio del código, respuestas coherentes |

---

## Importante: Defensa del Código (Video Loom)

Después de completar el desarrollo, debes grabar un video explicando tu solución.

### Instrucciones del Video

1. **Crea una cuenta gratuita en [Loom](https://www.loom.com/signup)** y graba tu pantalla
2. **Duración máxima: 30 minutos**
3. **Formato**: Pantalla completa + cámara (tu rostro visible)
4. **Envía el link del video** junto con el link de tu repositorio

### Contenido del Video

Tu video debe cubrir:

1. **Demo funcional** (~5 min)
   - Muestra la aplicación funcionando
   - Crea un comprobante, lista, filtra y anula

2. **Explicación de arquitectura** (~10 min)
   - Recorre la estructura de tu proyecto
   - Explica la separación de capas y responsabilidades
   - Justifica tus decisiones técnicas

3. **Code walkthrough** (~10 min)
   - Muestra el flujo de una request (controller → service → repository)
   - Explica las validaciones de negocio
   - Muestra cómo configuraste EF Core

4. **Responde estas preguntas en el video** (~5 min)
   - ¿Por qué elegiste esta estructura de carpetas/capas?
   - ¿Qué mejorarías si tuvieras más tiempo?
   - ¿Cómo manejarías concurrencia si dos usuarios anulan el mismo comprobante simultáneamente?

---

## Agenda

| Hora | Actividad |
|------|-----------|
| 9:00 - 13:00 | **Desarrollo** |
| 13:00 - 14:00 | Almuerzo |
| 14:00 - 16:00 | **Grabar video en Loom** y enviar enlaces |

### Entregables al finalizar (máximo 4:00 PM)

Enviar a:
- **m.cuzcano@contasiscorp.com**
- **a.mayta@finacontcorp.com**

| Entregable | Requisito |
|------------|-----------|
| **Repositorio GitHub** | Colaborador `alejandro.xux` agregado |
| **Video Loom** | Máximo 30 minutos, cubriendo los puntos indicados |

**Sin video = evaluación incompleta**

---

## Tips

1. **Prioriza funcionalidad sobre perfección**: Es mejor tener algo funcionando que código perfecto incompleto
2. **Commits frecuentes**: Haz commits pequeños con mensajes descriptivos
3. **Entiende lo que escribes**: Vas a tener que explicarlo en el video
4. **No hay soporte durante el reto**: Este documento contiene toda la información necesaria

---

Welcome to the matrix

Alejandro mayta
CTO Contasiscorp
