# Shop API Testing Pack 🧪

Este proyecto es un **mini‑backend en ASP.NET Core (.NET 9)** que he desarrollado con un objetivo muy concreto: **demostrar testing moderno y de calidad en .NET**, más allá de simples tests unitarios aislados.

La idea principal es mostrar cómo testear **APIs reales**, con **HTTP real**, **base de datos real en memoria** y flujos completos, tal y como se haría en un entorno profesional.

---

## 🎯 Objetivo del proyecto

Con este proyecto quería consolidar y demostrar:

- Cómo escribir **tests unitarios** claros y rápidos  
- Cómo escribir **tests de integración reales** usando `WebApplicationFactory`  
- Cómo testear **Minimal APIs** sin levantar servidores externos  
- Cómo usar **SQLite in‑memory** como base de datos real para tests  
- Cómo validar **flujos completos** (crear → persistir → recuperar)  
- Cómo comprobar **errores y validaciones**  
- Uso de **FluentAssertions** para tests legibles y expresivos  

Este repositorio está pensado como un **“testing pack”**, no como una aplicación de negocio grande.

---

## 🏗️ Arquitectura

```
ShopApiTestingPack
│
├── ShopApi
│   ├── Contracts        → DTOs de entrada y salida
│   ├── Data             → DbContext y entidades (EF Core)
│   ├── Middleware       → Manejo centralizado de errores de validación
│   ├── Validation       → Validadores con FluentValidation
│   └── Program.cs       → Minimal API
│
├── ShopApi.Tests
│   ├── Integration      → Tests de integración (HTTP real)
│   ├── Unit             → Tests unitarios (validadores)
│   └── Infrastructure   → WebApplicationFactory + SQLite in‑memory
│
└── ShopApiTestingPack.sln
```

---

## 🧪 Qué se testea

### Tests unitarios
- Validadores de productos y pedidos
- Casos inválidos y mensajes de error

### Tests de integración
- Creación de productos
- Creación de pedidos
- Recuperación de pedidos
- Validación de totales y errores

Todos los tests de integración usan:
- **HTTP real**
- **DbContext real**
- **SQLite in‑memory**
- Sin mocks de repositorios

---

## 🚀 Cómo ejecutar el proyecto

### Requisitos
- .NET SDK **9.x**
- No requiere Docker

### Pasos
```bash
git clone https://github.com/marcosdev97/ShopApiTestingPack.git
cd ShopApiTestingPack
dotnet restore
dotnet test
```

---

## 🧠 Decisiones técnicas

- Minimal APIs para reducir ruido
- EF Core con SQLite in‑memory
- WebApplicationFactory para integración real
- FluentValidation para validaciones
- FluentAssertions para tests claros

---

## 👤 Autor

**Marcos Pérez**  
.NET Backend Developer

---

Este proyecto forma parte de una serie de mini‑proyectos orientados a consolidar fundamentos sólidos de backend en .NET.
