# 🚗 CarManager WPF

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D7?style=for-the-badge&logo=windows&logoColor=white)

> Una aplicación de escritorio moderna y rápida para la gestión de inventario de vehículos de alta gama.

---

## 📋 Descripción

Este proyecto es una aplicación desarrollada en **WPF (Windows Presentation Foundation)** que permite administrar una base de datos de coches. Implementa el patrón **MVVM** y utiliza `ObservableCollection` para asegurar que la interfaz de usuario responda en tiempo real a los cambios en los datos.

El objetivo principal es permitir el control total (CRUD) sobre el stock de vehículos, gestionando datos técnicos como caballos de fuerza, aceleración y precio.

## ✨ Funcionalidades Principales

* **⚡ Visualización en Tiempo Real:** Listado de vehículos con actualización automática de la UI.
* **✏️ Edición Completa:** Modificación de detalles como Marca, Modelo, Año, Caballos (HP) y Precio.
* **🏎️ Datos Técnicos:** Soporte para métricas de rendimiento (Tiempo 0-60 mph).
* **💻 Interfaz Limpia:** Diseño basado en XAML con DataGrids interactivos.
* **🔄 Conexión Asíncrona:** Operaciones de actualización (`PUT`) implementadas con `Task` y `async/await` para no congelar la interfaz.

## 🛠️ Tecnologías Usadas

* **Lenguaje:** C#
* **Framework:** .NET / WPF
* **Arquitectura:** MVVM (Model-View-ViewModel)
* **IDE:** Visual Studio 2022

## 📸 Capturas de Pantalla

*(Aquí puedes poner una captura de tu DataGrid con los coches)*
![Screenshot de la App](https://via.placeholder.com/800x400?text=Captura+de+la+Aplicacion)

## 🚀 Instalación y Uso

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/TU_USUARIO/TU_PROYECTO.git](https://github.com/TU_USUARIO/TU_PROYECTO.git)
    ```
2.  **Abrir en Visual Studio:**
    Abre el archivo `.sln` con Visual Studio 2022.
3.  **Compilar y Ejecutar:**
    Presiona `F5` para iniciar la aplicación.

## 📄 Estructura del Modelo

El sistema gestiona objetos `Coche` con las siguientes propiedades:
* `Id`
* `Marca` / `Modelo`
* `Anio`
* `Caballos`
* `Tiempo0a60`
* `Precio`

---
Hecho con ❤️ y C#
