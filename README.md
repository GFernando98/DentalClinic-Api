# 🦷 DentalClinic - Sistema de Gestión para Clínicas Odontológicas

## Arquitectura
- **Clean Architecture** con separación en 4 capas
- **CQRS** con MediatR para Auth (extensible a todos los módulos)
- **Repository + Unit of Work** pattern
- **JWT + Refresh Token** con cierre por inactividad
- **Soft Delete** global vía query filters

## Stack Tecnológico
- .NET 8 Web API
- Entity Framework Core 8 + SQL Server
- ASP.NET Identity
- MediatR + FluentValidation
- MailKit (email) + WhatsApp Business API
- Serilog (logging)
- Swagger/OpenAPI

## Estructura del Proyecto

```
DentalClinic/
├── src/
│   ├── DentalClinic.Domain/          # Entidades, Enums, Interfaces
│   ├── DentalClinic.Application/     # CQRS, DTOs, Validators, Interfaces
│   ├── DentalClinic.Infrastructure/  # EF Core, Identity, Servicios externos
│   └── DentalClinic.API/             # Controllers, Middleware, Program.cs
└── DentalClinic.sln
```

## Módulos Implementados

### ✅ Autenticación y Autorización
- Login con JWT + Refresh Token
- Cierre de sesión por inactividad (configurable)
- Registro de usuarios (solo Admin)
- Cambio de contraseña
- 4 roles: Admin, Doctor, Receptionist, Assistant
- Políticas de autorización por rol

### ✅ Gestión de Pacientes
- CRUD completo
- Búsqueda por nombre/identidad
- Datos médicos, alergias, medicamentos
- Contacto de emergencia
- Soft delete

### ✅ Odontograma (Historial Dental)
- Creación de odontogramas (adulto/pediátrico)
- Notación FDI (ISO 3950) - estándar internacional
- Estado por diente (sano, cariado, obturado, extraído, etc.)
- Estado por superficie (mesial, distal, bucal, lingual, oclusal)
- Registro de tratamientos por diente
- Historial completo de tratamientos

### ✅ Control de Citas
- Agenda con filtros por fecha/doctor
- Detección de conflictos de horario
- Estados: Programada, Confirmada, En progreso, Completada, Cancelada, No show
- Vista de citas del día

### ✅ Catálogo de Tratamientos
- 22 tratamientos pre-cargados (preventivos, restauradores, endodoncia, cirugía, etc.)
- Precios por defecto configurables
- Duración estimada

### ✅ Notificaciones (estructura lista)
- Entidad de notificaciones (WhatsApp + Email)
- Servicio de WhatsApp Business API
- Servicio de Email con MailKit/SMTP

## Setup Rápido

### 1. Prerequisitos
- .NET 8 SDK
- SQL Server (local o Docker)
- Visual Studio 2022 / VS Code / Rider

### 2. Configurar Base de Datos
Editar `appsettings.json` con tu connection string:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DentalClinicDB;Trusted_Connection=true;TrustServerCertificate=true"
}
```

### 3. Configurar JWT Secret
**IMPORTANTE:** Cambiar el secret en `appsettings.json`:
```json
"Jwt": {
    "Secret": "TU-SECRET-KEY-MUY-LARGA-DE-AL-MENOS-64-CARACTERES-AQUI"
}
```

### 4. Crear Migración Inicial
```bash
cd src/DentalClinic.API
dotnet ef migrations add InitialCreate --project ../DentalClinic.Infrastructure
```

### 5. Ejecutar
```bash
dotnet run --project src/DentalClinic.API
```

La API arrancará y automáticamente:
- Aplicará migraciones pendientes
- Creará los roles (Admin, Doctor, Receptionist, Assistant)
- Creará el usuario admin: `admin@dentalclinic.com` / `Admin@123!`
- Sembrará el catálogo de tratamientos

### 6. Swagger
Abrir en el navegador: `https://localhost:5001` (o el puerto que configure)

## Endpoints Principales

### Auth
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/auth/login | Iniciar sesión |
| POST | /api/auth/refresh-token | Renovar token |
| POST | /api/auth/logout | Cerrar sesión |
| POST | /api/auth/register | Registrar usuario (Admin) |
| POST | /api/auth/change-password | Cambiar contraseña |
| GET | /api/auth/me | Obtener usuario actual |

### Pacientes
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/patients | Listar pacientes |
| GET | /api/patients/{id} | Obtener paciente |
| GET | /api/patients/search?name=X | Buscar pacientes |
| POST | /api/patients | Crear paciente |
| PUT | /api/patients/{id} | Actualizar paciente |
| DELETE | /api/patients/{id} | Eliminar (soft) |

### Odontograma
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/odontogram/patient/{patientId} | Historial de odontogramas |
| GET | /api/odontogram/{id} | Obtener odontograma |
| POST | /api/odontogram | Crear odontograma |
| PUT | /api/odontogram/tooth/{id} | Actualizar diente |
| POST | /api/odontogram/tooth/{id}/surface | Registrar superficie |
| POST | /api/odontogram/tooth/{id}/treatment | Registrar tratamiento |
| GET | /api/odontogram/tooth/{id}/treatments | Historial de tratamientos |

### Citas
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/appointments?from=X&to=Y | Listar citas |
| GET | /api/appointments/today | Citas de hoy |
| GET | /api/appointments/patient/{id} | Citas de paciente |
| POST | /api/appointments | Crear cita |
| PUT | /api/appointments/{id} | Actualizar cita |
| PUT | /api/appointments/{id}/status | Cambiar estado |

## Configuración de Notificaciones

### Email (Gmail)
1. Habilitar "App Passwords" en tu cuenta Google
2. Configurar en `appsettings.json` la sección `Email`

### WhatsApp
1. Crear cuenta en [Meta Business](https://business.facebook.com)
2. Configurar WhatsApp Business API
3. Obtener PhoneNumberId y ApiToken
4. Configurar en `appsettings.json` la sección `WhatsApp`

## Próximos Pasos (Frontend)
El frontend se desarrollará con React + TypeScript incluyendo:
- Dashboard administrativo
- Odontograma interactivo (SVG)
- Calendario de citas
- Panel de pacientes
- Sistema de notificaciones
