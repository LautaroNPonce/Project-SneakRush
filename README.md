<div align="center">

# 🏪 SneakRush

**ERP de escritorio para la venta minorista de zapatillas y productos deportivos.**
Stock, Compras y Punto de Venta en un solo lugar, con control de acceso por rol e integridad de datos verificada.

![C#](https://img.shields.io/badge/C%23-.NET%20Framework%204.8-512bd4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%20%2F%20LocalDB-cc2927?style=flat-square)
![Plataforma](https://img.shields.io/badge/Windows-10%20%2F%2011%20(64--bit)-0078d6?style=flat-square)
![Arquitectura](https://img.shields.io/badge/arquitectura-5%20capas-e23b3b?style=flat-square)

</div>

<!--
  ↓ CAPTURA DE PANTALLA ↓
  Cuando tengas una captura linda:
    1. Creá la carpeta  docs/  en la raíz.
    2. Guardá la imagen como  captura.png
    3. Borrá este comentario para que se vea.

<div align="center">
  <img src="docs/captura.png" alt="Menú principal de SneakRush" width="380">
</div>
-->

---

## Qué es

SneakRush es un sistema de gestión (ERP) hecho a medida para una tienda de zapatillas y artículos deportivos. Reemplaza las tareas manuales y las planillas sueltas por una única aplicación de escritorio que centraliza **inventario, compras y punto de venta**, con datos sincronizados y validaciones estrictas.

Lo que lo distingue no es solo el ABM: es el **control de acceso granular** (cada botón y cada opción del menú se habilita por permiso) y un sistema de **dígitos verificadores** que detecta si alguien tocó la base de datos por fuera de la aplicación.

### Funciona

| | |
|---|---|
| 🔐 **Login seguro** | Contraseñas con SHA-256 y datos sensibles con AES-256. Bloqueo por intentos fallidos y cambio de contraseña forzado. |
| 🧩 **Permisos por rol (Composite)** | Perfiles → Familias → Patentes. Se habilitan menús *y botones* uno por uno según el rol del usuario. |
| 🛡️ **Integridad verificada (DV)** | Cada tabla lleva un dígito verificador. Si la base se modifica por fuera del sistema, el login lo detecta y ofrece reparación. |
| 📋 **Bitácora de eventos** | Auditoría de acciones con nivel de criticidad, consultable y filtrable. |
| 🌐 **Multi-idioma en caliente** | Español, Inglés y Portugués vía patrón Observer + archivos JSON. El idioma se conserva entre reinicios. |
| 💾 **Respaldos** | Backup y restauración de la base de datos desde la propia aplicación. |

---

## Stack y decisiones

**C# · .NET Framework 4.8 · SQL Server 2019 / LocalDB · Windows Forms (MDI).**

Aplicación de escritorio nativa pensada para correr en red LAN (TCP/IP), con una separación estricta en capas y patrones de diseño clásicos. El estándar de calidad de referencia es **ISO/IEC 25010**.

<details>
<summary><b>Por qué</b> (clic para abrir)</summary>

<br>

- **Arquitectura en 5 capas** → GUI, BLL, DAL, Services y BE se comunican en un solo sentido. La GUI nunca llama a DAL; Services no llama a BLL ni a DAL; los errores viajan como `out string`, no como `MessageBox` en las capas bajas.
- **Patrón Composite para permisos** → una *Patente* (permiso) es la hoja, una *Familia* agrupa patentes, y un *Perfil* arma el rol final. Permite anidar y reutilizar sin duplicar reglas.
- **Patrón Singleton para la sesión** → `SessionManager` garantiza una única sesión activa y un solo punto de verdad para "quién está logueado".
- **Patrón Observer para el idioma** → `LanguageManager` notifica a cada formulario cuando cambia el idioma, sin recargar la app.
- **Dígitos verificadores (DV) propios** → en vez de confiar solo en la base, cada tabla guarda un hash por fila (DVH) y por columna (DVV). Es la línea de defensa contra manipulación directa del SQL.

Las patentes se definen **read-only por script SQL** a propósito: ni el administrador puede crearlas desde la interfaz, para que el catálogo de permisos del sistema no se corrompa desde el uso diario.

</details>

---

## Estructura

La solución está dividida en proyectos, uno por capa. El sufijo `486LP` identifica al alumno y evita choques de nombres.

```
Sistema SneakRush486LP/         solución completa (Visual Studio)
│
├── BE486LP/                     entidades base del dominio
│
├── Services486LP/              soporte transversal
│   ├── Encriptacion486LP        SHA-256 + AES-256
│   ├── SessionManager486LP      Singleton de sesión
│   ├── Composite486LP           patrón Composite (Permiso / Familia)
│   ├── DV486LP                  dígito verificador
│   └── LanguageManager486LP     patrón Observer (i18n)
│
├── DAL486LP/                    acceso a datos (ADO.NET → SQL Server)
│
├── BLL486LP/                    lógica de negocio y validaciones
│
├── Sistema_SneakRush (GUI)/     formularios Windows Forms (MDI)
│   └── Idiomas/                 es.json · en.json · pt.json
│
└── BaseDeDatos/                 script .sql para crear SneakRushDB
```

> **Nota:** el flujo de referencias es unidireccional — `GUI → BLL → DAL`, todos apoyados en `BE` y `Services`. Services no depende de BLL ni de DAL.

---

## Desarrollo

**Requisitos:** Visual Studio (con soporte .NET Framework 4.8), SQL Server LocalDB o Express, y SSMS para correr el script.

1. Cloná el repositorio y abrí la solución (`.sln`) en Visual Studio.
2. Abrí el script de `BaseDeDatos/` en SSMS y ejecutalo (F5) → crea la base `SneakRushDB`.
3. Compilá en modo **Release** y ejecutá. Ingresá con el usuario administrador inicial que crea el script.

| Para | Hacé |
|---|---|
| Compilar todo | `Compilación → Recompilar solución` (0 errores esperados) |
| Cambiar el idioma en runtime | Menú → Usuario → Cambiar idioma |
| Ver la auditoría | Menú → Administrador → Bitácora de eventos |

### ⚠️ Advertencia importante del flujo

El nombre del **Rol** en la tabla `Usuarios` tiene que coincidir *exactamente* (mayúsculas incluidas) con el nombre del **Perfil** en la tabla `Perfil`. Si no coincide, el menú aparece vacío porque no encuentra permisos.

```sql
-- Usuarios.Rol  debe ser idéntico a  Perfil.Nombre
-- 'Vendedor' ≠ 'vendedor' ≠ 'VENDEDOR'
```

---

## Publicar

El proyecto se empaqueta con **Inno Setup**:

1. Compilá la solución en **Release**.
2. Recompilá el script `SneakRush_Instalador.iss` en Inno Setup → genera `Salida/Instalador_SneakRush.exe` (~7 MB).
3. En la PC destino: ejecutá el instalador, corré el `.sql` incluido en SSMS y listo.

> Regla de oro: el instalador va **siempre último**. Cada cambio de código lo deja viejo hasta recompilar Release *y* regenerar el `.iss`.

---

## Datos / Configuración

La conexión vive en `Conexion486LP.cs` (capa DAL). Con **LocalDB** (`(localdb)\MSSQLLocalDB`) no hace falta tocar nada al mover el proyecto de máquina: el nombre de la instancia es igual en todas. Los textos de la interfaz viven en los tres JSON de la carpeta `Idiomas/`.

> ### 🚨 Lo que NUNCA hay que tocar
> Las **patentes** (tabla `Permiso`) son read-only y se cargan por script SQL. Si las borrás o renombrás desde la base, el control de acceso de todo el sistema se rompe, porque los menús y botones se habilitan por el nombre exacto de cada patente.

<details>
<summary>Detalle técnico plegado</summary>

<br>

```
Perfil   →  agrupa Familias y/o Patentes sueltas   (lo crea el Admin)
Familia  →  agrupa Patentes                        (lo crea el Admin)
Patente  →  permiso indivisible                     (read-only, por SQL)
```

Catálogo total: **50 patentes** (19 de menú + 31 de botón). El perfil Administrador recibe las 19 patentes de menú por script como caso especial (problema del huevo y la gallina); el resto de los perfiles los arma el Admin desde la aplicación.

Cada `INSERT`, `UPDATE` o `DELETE` sobre una tabla protegida dispara `RecalcularDV()` para mantener el dígito verificador al día.

</details>

---

## Mantenimiento

<details>
<summary><b>Backup y restauración de la base</b></summary>

<br>

Desde la app: `Administrador → Gestión de respaldos`.

- **Backup** = una foto de la base en ese momento.
- **Restore** = reemplaza *toda* la base con la foto y reinicia la aplicación (la sesión en memoria puede no existir en la base restaurada).

> ⚠️ La cuenta de servicio de SQL necesita permisos de escritura sobre la carpeta destino del backup (por ejemplo `C:\Backups`).

</details>

<details>
<summary><b>Agregar un idioma nuevo</b></summary>

<br>

1. Duplicá uno de los JSON de `Idiomas/` (ej: `es.json` → `it.json`) y traducí los valores.
2. Marcá el archivo como *Contenido* → *Copiar siempre* en sus propiedades.
3. Agregá el idioma a la tabla `Idioma` de la base.

> ⚠️ El `LanguageManager` busca cada texto por el nodo del formulario. Si una clave queda en el nodo equivocado, sale el texto crudo en vez del traducido.

</details>

---

## Roadmap

- [ ] Formularios de negocio: Ventas (POS), Compras, Maestros y Reportes
- [ ] Auditoría completa del DV, tabla por tabla
- [ ] Registrar los eventos de Bitácora desde la BLL en lugar de la GUI
- [ ] Composite anidado (agregar una familia dentro de otra familia)
- [ ] Pulido visual general de los formularios

---

## Créditos

Proyecto académico de **Ingeniería de Software** — Universidad Abierta Interamericana (UAI), Facultad de Tecnología Informática, Ingeniería en Sistemas · 2026.

Desarrollado por **Lautaro Nahuel Ponce**.

<div align="center">
<br>
<sub>SneakRush — porque el stock desactualizado no debería costarte una venta.</sub>
</div>
